
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobDriver_GettinRaped : JobDriver {
		
		private int ticks_between_hearts;
		
		private int ticks_remaining = 10;
		
		public int rapist_count = 1; // Defaults to 1 so the first rapist doesn't have to add themself
		
		private bool was_laying_down;
		
		public void increase_time (int min_ticks_remaining)
		{
			if (min_ticks_remaining > ticks_remaining)
				ticks_remaining = min_ticks_remaining;
		}
		
		protected override IEnumerable<Toil> MakeNewToils ()
		{
			ticks_between_hearts = Rand.RangeInclusive (70, 130);
			was_laying_down = (pawn.jobs.curDriver != null) && pawn.jobs.curDriver.layingDown != Verse.AI.LayingDownState.NotLaying;
			


			var get_raped = new Toil ();
			get_raped.defaultCompleteMode = ToilCompleteMode.Never;
			get_raped.initAction = delegate {
				pawn.pather.StopDead ();
				pawn.jobs.curDriver.layingDown = Verse.AI.LayingDownState.NotLaying;
				pawn.jobs.curDriver.asleep = false;
				pawn.mindState.awokeVoluntarily = false;
                // drop clothing
                if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0) {
                    pawn.apparel.DropAll(pawn.Position, false);
                }
            };
			get_raped.tickAction = delegate {
				--ticks_remaining;
				if ((ticks_remaining <= 0) || (rapist_count <= 0))
					ReadyForNextToil ();
				if ((rapist_count > 0) && (pawn.IsHashIntervalTick (ticks_between_hearts / rapist_count)))
					MoteMaker.ThrowMetaIcon (pawn.Position, pawn.Map, xxx.mote_noheart);
			};
			get_raped.socialMode = RandomSocialMode.Off;
			yield return get_raped;
			
			var after_rape = new Toil ();
			after_rape.defaultCompleteMode = ToilCompleteMode.Delay;
			after_rape.defaultDuration = 150;
			after_rape.socialMode = RandomSocialMode.Off;
			after_rape.AddFinishAction (delegate {
                pawn.jobs.curDriver.layingDown = was_laying_down ? Verse.AI.LayingDownState.NotLaying : Verse.AI.LayingDownState.LayingInBed;
			});
			yield return after_rape;
		}
		
	}
}
