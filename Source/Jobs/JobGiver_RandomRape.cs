
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobGiver_RandomRape : ThinkNode_JobGiver {

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
		
		public static Pawn find_victim (Pawn rapist, Map m)
		{
			Pawn best_rapee = null;
			var best_fuckability = 0.20f; // Don't rape prisoners with <20% fuckability
            foreach (var target in m.mapPawns.AllPawns) {
                if (target != rapist && rapist.CanReserve( target, comfort_prisoners.max_rapists_per_prisoner, 0) && !target.Position.IsForbidden(rapist) && is_healthy_enough(target)) {
                    if (!xxx.is_animal(target) || (xxx.is_animal(target) && xxx.config.animals_enabled)) {
                        var fuc = xxx.would_fuck(rapist, target, true);
                        if ((fuc > best_fuckability) && (Rand.Value < 0.9 * fuc)) {
                            best_rapee = target;
                            best_fuckability = fuc;
                        }
                    }
                }
            }
			return best_rapee;
		}

		protected override Job TryGiveJob (Pawn p)
		{
            //Log.Message("[RJW] JobGiver_RandomRape::TryGiveJob( " + p.NameStringShort + " ) called");

			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null)) {
				
                // don't allow pawns marked as comfort prisoners to rape others
				if (p.health.capacities.CanBeAwake && xxx.can_fuck (p)) {
				
					var prisoner = find_victim(p, p.Map);


                    if (prisoner != null) {
                        //Log.Message("[RJW] JobGiver_RandomRape::TryGiveJob( " + p.NameStringShort + " ) - found victim " + prisoner.NameStringShort);
                        return new Job(DefDatabase<JobDef>.GetNamed("RandomRape"), prisoner);
                    } else {
                        //Log.Message("[RJW] JobGiver_RandomRape::TryGiveJob( " + p.NameStringShort + " ) - unable to find victim");
                        p.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
                    }
					
				}
			}
			
			return null;
		}
	}
}
