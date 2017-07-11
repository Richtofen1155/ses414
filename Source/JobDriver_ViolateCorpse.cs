
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

namespace rjw {
	public class JobDriver_ViolateCorpse : JobDriver {
		
		private int duration;
		
		private int ticks_between_hearts;
		
		private int ticks_between_hits = 50;

		private int ticks_between_thrusts;
		
		protected TargetIndex iprisoner = TargetIndex.A;

        private List<Apparel> worn_apparel;

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
		
		protected Thing Corpse
		{
			get
			{
				return (Thing)(CurJob.GetTarget (iprisoner));
			}
		}

        public static void sexTick(Pawn pawn, Thing corpse) {
            pawn.Drawer.rotator.Face(corpse.DrawPos);

            if (xxx.config.sounds_enabled) {
                SoundDef.Named("Sex").PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
            }

            pawn.Drawer.Notify_MeleeAttackOn(corpse);
            pawn.Drawer.rotator.FaceCell(corpse.Position);
        }

        public static void roll_to_hit (Pawn rapist, Pawn p)
		{
			if (!HugsLibInj.prisoner_beating)
			{
				return;
			}


            float rand_value = Rand.Value;
            float victim_pain = p.health.hediffSet.PainTotal;
            // bloodlust makes the aggressor more likely to hit the prisoner
            float beating_chance = xxx.config.base_chance_to_hit_prisoner * (xxx.is_bloodlust(rapist) ? 1.25f : 1.0f);
            // psychopath makes the aggressor more likely to hit the prisoner past the significant_pain_threshold
            float beating_threshold = xxx.is_psychopath(rapist) ? xxx.config.extreme_pain_threshold : xxx.config.significant_pain_threshold;

            //Log.Message("roll_to_hit:  rand = " + rand_value + ", beating_chance = " + beating_chance + ", victim_pain = " + victim_pain + ", beating_threshold = " + beating_threshold);
            if ((victim_pain < beating_threshold && rand_value < beating_chance) || (rand_value < (beating_chance / 2))) {
                //Log.Message("   done told her twice already...");
                Verb v;
                if (InteractionUtility.TryGetRandomVerbForSocialFight( rapist, out v)) {
                    rapist.meleeVerbs.TryMeleeAttack(p, v);
                }
            }

            /*
            //if (p.health.hediffSet.PainTotal < xxx.config.significant_pain_threshold)
			if ((Rand.Value < 0.50f) &&
				((Rand.Value < 0.33f) || (p.health.hediffSet.PainTotal < xxx.config.significant_pain_threshold) ||
			     (xxx.is_bloodlust (rapist) || xxx.is_psychopath (rapist)))) {
				Verb v;
				if (InteractionUtility.TryGetRandomVerbForSocialFight (rapist, out v))
					rapist.meleeVerbs.TryMeleeAttack (p, v);
			}
            */
		}
		
		protected override IEnumerable<Toil> MakeNewToils ()
		{
            Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() called");
			duration = (int)(2500.0f * Rand.Range (0.50f, 0.90f));
			ticks_between_hearts = Rand.RangeInclusive (70, 130);
			ticks_between_hits = Rand.Range (xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			ticks_between_thrusts = 100;

          
			if (xxx.is_bloodlust (pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
            if (xxx.is_brawler(pawn))
                ticks_between_hits = (int)(ticks_between_hits * 0.90);

            //this.FailOnDespawnedNullOrForbidden (iprisoner);
            //this.FailOn (() => (!Prisoner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated (Prisoner)));
            //this.FailOn(() => !pawn.CanReserve(Corpse, 1)); // comfort_prisoners.max_rapists_per_prisoner)); // Fail if someone else reserves the prisoner before the pawn arrives
            //this.FailOn(() => !pawn.CanReserve(Prisoner, comfort_prisoners.max_rapists_per_prisoner, -1, null, true)); // ok if someone else reserves the prisoner before the pawn arrives
            Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - moving towards corpse");
            yield return Toils_Goto.GotoThing (iprisoner, PathEndMode.OnCell);


            var rape = new Toil ();
			rape.initAction = delegate {
                Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - reserving corpse");
                pawn.Reserve(Corpse, 1); // comfort_prisoners.max_rapists_per_prisoner);

                //// Trying to add some interactions and social logs
                //InteractionDef intDef = DefDatabase<InteractionDef>.GetNamed("AnalRaped");
                //if (!xxx.is_animal(pawn) && !xxx.is_animal(Prisoner))
                //    pawn.interactions.TryInteractWith(Prisoner, intDef);

                /*
				var dri = Prisoner.jobs.curDriver as JobDriver_GettinRaped;
				if (dri == null) {
					var gettin_raped = new Job (xxx.gettin_raped);
					Prisoner.jobs.StartJob (gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					(Prisoner.jobs.curDriver as JobDriver_GettinRaped).increase_time (duration);
				} else {
					dri.rapist_count += 1;
					dri.increase_time (duration);
				}
                */

                // Try to take off the attacker's clothing
                Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - stripping corpse");
                worn_apparel = pawn.apparel.WornApparel.ListFullCopy<Apparel>();
                while (pawn.apparel != null && pawn.apparel.WornApparelCount > 0) {
                    Apparel apparel = pawn.apparel.WornApparel.RandomElement<Apparel>();
                    pawn.apparel.Remove(apparel);
                }
                //pawn.apparel.WornApparel.RemoveAll(null);

                //List<Apparel> worn = pawn.apparel.WornApparel;
                //while (pawn.apparel != null && pawn.apparel.WornApparelCount > 0) {
                //    Apparel apparel = pawn.apparel.WornApparel.RemoveAll(null);
                //    pawn.apparel.Remove(apparel);
                //}
 
			};
			rape.tickAction = delegate {
				if (pawn.IsHashIntervalTick (ticks_between_hearts))
					MoteMaker.ThrowMetaIcon (pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
				if (pawn.IsHashIntervalTick (ticks_between_thrusts))
					sexTick (pawn, Corpse);
                /*
				if (pawn.IsHashIntervalTick (ticks_between_hits))
					roll_to_hit (pawn, Corpse);
                    */
			};
			rape.AddFinishAction (delegate {
                Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - finished violating");
                /*
				if ((Prisoner.jobs != null) &&
			    	(Prisoner.jobs.curDriver != null) &&
			    	(Prisoner.jobs.curDriver as JobDriver_GettinRaped != null))
	               		(Prisoner.jobs.curDriver as JobDriver_GettinRaped).rapist_count -= 1;
                        */
            });
			rape.defaultCompleteMode = ToilCompleteMode.Delay;
			rape.defaultDuration = duration;
			yield return rape;

			yield return new Toil {
				initAction = delegate {
                    Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - creating aftersex toil");
                    //xxx.aftersex (pawn, Corpse, pawn);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (pawn);
					//if (!Corpse.Dead) {
					//	xxx.aftersex (Corpse, pawn, pawn);
                    //    Corpse.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (Corpse);
					//}

                    Log.Message("[RJW] JobDriver_ViolateCorpse::MakeNewToils() - putting clothes back on");
                    if (pawn.apparel != null) {
                        foreach (Apparel apparel in worn_apparel) {
                            pawn.apparel.Wear(apparel);//  WornApparel.Add(apparel);
                        }
                    }
                },
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
		
	}
}
