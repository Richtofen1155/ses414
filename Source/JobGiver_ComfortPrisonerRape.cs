
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobGiver_ComfortPrisonerRape : ThinkNode_JobGiver {

		// Returns the designated pawn if the comfort prisoner designation is still valid
		public static Pawn check_cp_designation (Map m, Designation des)
		{
			var p = des.target.Thing as Pawn;
            //var log_msg = "check_cp_designation() - pawn.Name = " + p.Name;
            //Log.Message(log_msg);

			if ((p.Map == m) /*&& (p.IsPrisonerOfColony)*/)
				return p;
			else
				return null;
		}
		
		private static bool is_healthy_enough (Pawn p)
		{
			return (! p.Dead) && p.health.capacities.CanBeAwake && (p.health.hediffSet.BleedRateTotal <= 0.0f) && xxx.can_get_raped(p);
		}
		
		public static Pawn find_prisoner_to_rape (Pawn rapist, Map m)
		{
			List<Designation> invalid_designations = null;
			Pawn best_rapee = null;
			var best_fuckability = 0.20f; // Don't rape prisoners with <20% fuckability
            DesignationDef designation_def = xxx.config.rape_me_sticky_enabled ? comfort_prisoners.designation_def : comfort_prisoners.designation_def_no_sticky;
			foreach (var des in m.designationManager.SpawnedDesignationsOfDef (designation_def)) {
				var q = check_cp_designation (m, des);
				if (q != null) {
					if ((q != rapist) && rapist.CanReserve (q, comfort_prisoners.max_rapists_per_prisoner) && (!q.Position.IsForbidden (rapist)) && is_healthy_enough (q)) {
						var fuc = xxx.would_fuck(rapist, q, true);
                        //var log_msg = rapist.Name + " -> " + q.Name + " (" + fuc.ToString() + " / " + best_fuckability.ToString() + ")";
                        //Log.Message(log_msg);

						if ((fuc > best_fuckability) && (Rand.Value < 0.9*fuc)) {
							best_rapee = q;
							best_fuckability = fuc;
						}
					}
				} else {
					if (invalid_designations == null)
						invalid_designations = new List<Designation> ();
					invalid_designations.Add (des);
				}
			}
			if (!invalid_designations.NullOrEmpty<Designation> ())
				foreach (var invalid_des in invalid_designations)
					m.designationManager.RemoveDesignation (invalid_des);
			return best_rapee;
		}

		protected override Job TryGiveJob (Pawn p)
		{
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null)) {
				
                // don't allow pawns marked as comfort prisoners to rape others
				if (p.health.capacities.CanBeAwake && !comfort_prisoners.is_designated(p) && xxx.can_fuck (p)) {
				
					var prisoner = find_prisoner_to_rape(p, p.Map);

					if (prisoner != null)
						return new Job (xxx.comfort_prisoner_rapin, prisoner);
					else
						p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range (75, 150);
					
				}
			}
			
			return null;
		}
	}
}
