
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ChancePerHour_Fappin : ThinkNode_ChancePerHour {

		public static float get_fappin_mtb_hours (Pawn p)
		{
			if (p.Dead)
				return -1.0f;
			
			if (DebugSettings.alwaysDoLovin)
				return 0.1f;
			
			if (p.needs.food.Starving)
				return -1.0f;
			
			if (p.health.hediffSet.BleedRateTotal > 0.0f)
				return -1.0f;
			
			return (xxx.is_nympho (p) ? 0.50f : 1.0f) * rjw_CORE_EXPOSED.LovePartnerRelationUtility.LovinMtbSinglePawnFactor (p);
		}

		protected override float MtbHours (Pawn p)
		{
			bool can_get_job =
				(p.CurJob != null) &&
				p.jobs.curDriver.layingDown != LayingDownState.NotLaying;

			if (p.jobs.curDriver is JobDriver_LayDown) {
				bool is_horny;
				{
					var need_sex = p.needs.TryGetNeed<Need_Sex> ();
					is_horny = (need_sex != null) && (need_sex.CurLevel <= need_sex.thresh_horny ());
				}

				// TODO Bed check?

				if (can_get_job && is_horny) {
					return get_fappin_mtb_hours (p);
				}
			}
			return -1.0f;
		}
	}
}
