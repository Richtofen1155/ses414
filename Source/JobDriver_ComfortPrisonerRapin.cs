
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobDriver_ComfortPrisonerRapin : JobDriver {
		
		private int duration;
		
		private int ticks_between_hearts;
		
		private int ticks_between_hits = 50;

		private int ticks_between_thrusts;
		
		protected TargetIndex iprisoner = TargetIndex.A;

        // Same as in JobDriver_Lovin
        private static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
        {
            new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};
		
		protected Pawn Prisoner
		{
			get
			{
				return (Pawn)(CurJob.GetTarget (iprisoner));
			}
		}
		
		public static void roll_to_hit (Pawn rapist, Pawn p)
		{
			if (!HugsLibInj.prisoner_beating)
			{
				return;
			}

			if ((Rand.Value < 0.50f) &&
				((Rand.Value < 0.33f) ||
				 (p.health.hediffSet.PainTotal < xxx.config.significant_pain_threshold) ||
			     (xxx.is_bloodlust (rapist) || xxx.is_psychopath (rapist)))) {
				Verb v;
				if (InteractionUtility.TryGetRandomVerbForSocialFight (rapist, out v))
					rapist.meleeVerbs.TryMeleeAttack (p, v);
			}
		}
		
		protected override IEnumerable<Toil> MakeNewToils ()
		{
			duration = (int)(2500.0f * Rand.Range (0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive (70, 130);
			ticks_between_hits = Rand.Range (500, 700);
			ticks_between_thrusts = 100;

			if (xxx.is_bloodlust (pawn))
				ticks_between_hits /= 2;
			if (xxx.is_brawler (pawn))
				ticks_between_hits /= 2;
			
			this.FailOnDespawnedNullOrForbidden (iprisoner);
			this.FailOn (() => (!Prisoner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated (Prisoner)));
            this.FailOn(() => !pawn.CanReserve(Prisoner, comfort_prisoners.max_rapists_per_prisoner)); // Fail if someone else reserves the prisoner before the pawn arrives
            //this.FailOn(() => !pawn.CanReserve(Prisoner, comfort_prisoners.max_rapists_per_prisoner, -1, null, true)); // ok if someone else reserves the prisoner before the pawn arrives
            yield return Toils_Goto.GotoThing (iprisoner, PathEndMode.OnCell);
            

			var rape = new Toil ();
			rape.initAction = delegate {
				pawn.Reserve (Prisoner, comfort_prisoners.max_rapists_per_prisoner);
				var dri = Prisoner.jobs.curDriver as JobDriver_GettinRaped;
				if (dri == null) {
					var gettin_raped = new Job (xxx.gettin_raped);
					Prisoner.jobs.StartJob (gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					(Prisoner.jobs.curDriver as JobDriver_GettinRaped).increase_time (duration);
				} else {
					dri.rapist_count += 1;
					dri.increase_time (duration);
				}
			};
			rape.tickAction = delegate {
				if (pawn.IsHashIntervalTick (ticks_between_hearts))
					MoteMaker.ThrowMetaIcon (pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick (ticks_between_thrusts))
					xxx.sexTick (pawn, Prisoner);
				if (pawn.IsHashIntervalTick (ticks_between_hits))
					roll_to_hit (pawn, Prisoner);
			};
			rape.AddFinishAction (delegate {
				if ((Prisoner.jobs != null) &&
			    	(Prisoner.jobs.curDriver != null) &&
			    	(Prisoner.jobs.curDriver as JobDriver_GettinRaped != null))
	               		(Prisoner.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
			});
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil {
				initAction = delegate {
                    xxx.aftersex (pawn, Prisoner, pawn);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (pawn);
					if (! Prisoner.Dead) {
						xxx.aftersex (Prisoner, pawn, pawn);
						Prisoner.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (Prisoner);
					}
                },
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
		
	}
}
