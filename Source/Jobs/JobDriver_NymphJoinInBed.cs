
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobDriver_NymphJoinInBed : JobDriver {
		
		private const int ticks_between_hearts = 100;
		
		private int ticks_left;
		
		private TargetIndex ipartner = TargetIndex.A;
		
		private TargetIndex ibed = TargetIndex.B;
		
		protected Pawn Partner
		{
			get
			{
				return (Pawn)(CurJob.GetTarget (ipartner));
			}
		}
		
		protected Building_Bed Bed
		{
			get
			{
				return (Building_Bed)(CurJob.GetTarget (ibed));
			}
		}
		
		protected override IEnumerable<Toil> MakeNewToils ()
		{
			this.FailOnDespawnedOrNull (ipartner);
			this.FailOnDespawnedOrNull (ibed);
			this.FailOn (() => ! Partner.health.capacities.CanBeAwake);
			this.FailOn (() => ! xxx.is_laying_down_alone (Partner));
			yield return Toils_Reserve.Reserve (ipartner, 1);
			yield return Toils_Goto.GotoThing (ipartner, PathEndMode.OnCell);
			yield return new Toil {
				initAction = delegate {
					ticks_left = (int)(2500.0f * Rand.Range (0.30f, 1.30f));
					var gettin_loved = new Job (xxx.gettin_loved, pawn, Bed);
					Partner.jobs.StartJob (gettin_loved, JobCondition.InterruptForced, null, false, true, null);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			var do_lovin = new Toil ();
			do_lovin.defaultCompleteMode = ToilCompleteMode.Never;
			do_lovin.FailOn (() => (Partner.CurJob == null) || (Partner.CurJob.def != xxx.gettin_loved));
			do_lovin.AddPreTickAction (delegate {
				--ticks_left;
			    if (ticks_left <= 0)
			    	ReadyForNextToil ();
			    else if (pawn.IsHashIntervalTick (ticks_between_hearts))
			    	MoteMaker.ThrowMetaIcon (pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
			});
			do_lovin.socialMode = RandomSocialMode.Off;
			yield return do_lovin;
			yield return new Toil {
				initAction = delegate {
					var sex_mem = (Thought_Memory)ThoughtMaker.MakeThought (ThoughtDefOf.GotSomeLovin);
					pawn.needs.mood.thoughts.memories.TryGainMemory (sex_mem, Partner);
					var sex_mem2 = (Thought_Memory)ThoughtMaker.MakeThought (ThoughtDefOf.GotSomeLovin); // Is this neccessary?
					Partner.needs.mood.thoughts.memories.TryGainMemory (sex_mem2, pawn);
					xxx.aftersex (pawn, Partner);
					xxx.aftersex (Partner, pawn);
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (pawn);
					Partner.mindState.canLovinTick = Find.TickManager.TicksGame + xxx.generate_min_ticks_to_next_lovin (Partner);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
		
	}
}
