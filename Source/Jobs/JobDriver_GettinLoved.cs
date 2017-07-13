
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobDriver_GettinLoved : JobDriver {
		
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
			this.FailOn (() => ! Partner.health.capacities.CanBeAwake);
			this.KeepLyingDown (ibed);
			yield return Toils_Reserve.Reserve (ipartner, 1, 0);
			yield return Toils_Reserve.Reserve (ibed, Bed.SleepingSlotsCount, 0);
			Toil get_loved = Toils_LayDown.LayDown (ibed, true, false, false, false);
			get_loved.FailOn (() => (Partner.CurJob == null) || (Partner.CurJob.def != xxx.nymph_rapin));
			get_loved.defaultCompleteMode = ToilCompleteMode.Never;
			get_loved.AddPreTickAction (delegate {
			                            	if (pawn.IsHashIntervalTick (100))
			                            		MoteMaker.ThrowMetaIcon (pawn.Position, pawn.Map, ThingDefOf.Mote_Heart);
			                            });
			get_loved.socialMode = RandomSocialMode.Off;
			yield return get_loved;
		}
		
	}
}
