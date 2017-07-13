
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobGiver_DoFappin : ThinkNode_JobGiver {

		protected override Job TryGiveJob (Pawn p)
		{
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) &&
				(p.CurJob != null)) {

				Building_Bed bed = null;

				if (p.jobs.curDriver is JobDriver_LayDown) {
					bed = ((JobDriver_LayDown)p.jobs.curDriver).Bed;
				}

				if (p.jobs.curDriver.layingDown != LayingDownState.NotLaying &&
					(bed != null) &&
					(!bed.Medical) &&
					p.health.capacities.CanBeAwake &&
					xxx.can_fuck (p)) {

					var no_partner = xxx.is_laying_down_alone (p);
					bool is_frustrated;
					{
						var need_sex = p.needs.TryGetNeed<Need_Sex> ();
						is_frustrated = (need_sex != null) && (need_sex.CurLevel <= need_sex.thresh_frustrated ());
					}

					if (no_partner || is_frustrated) {
						p.mindState.awokeVoluntarily = true;
						return new Job (DefDatabase<JobDef>.GetNamed ("Fappin"), bed);
					}
				}
			}

			return null;
		}
	}
}
