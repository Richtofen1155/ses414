
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class JobGiver_ViolateCorpse : ThinkNode_JobGiver {

         public static Thing find_corpse( Pawn rapist, Map m) {
            Log.Message("JobGiver_ViolateCorpse::find_corpse( " + rapist.NameStringShort + " ) called");
            Thing found = null;
            var best_distance = 1.0e6f;
            int best_freshness = 100;


            foreach (Thing thing in m.listerThings.ThingsOfDef(ThingDef.Named("Human_Corpse"))) {
                //Log.Message(rapist.NameStringShort + " found a corpse with id " + thing.Label);
                Corpse corpse = thing as Corpse;
                if (rapist.CanReserve(thing, 1)) {
                    int freshness = corpse.GetRotStage().ChangeType<int>();
                    var distance = rapist.Position.DistanceToSquared(thing.Position);
                    //Log.Message("   " + corpse.InnerPawn.NameStringShort + " =  " + freshness + "/" + distance + ",  best =  " + best_freshness + "/" + best_distance);
                    if (freshness < best_freshness || (freshness <= best_freshness && distance < best_distance)) {
                        found = thing;
                        best_freshness = freshness;
                        best_distance = distance;
                    }
                }
            }

            return found;
        }

		protected override Job TryGiveJob (Pawn p)
		{
            //Log.Message("[RJW] JobGiver_ViolateCorpse::TryGiveJob( " + p.NameStringShort + " ) called");
			if ((Find.TickManager.TicksGame >= p.mindState.canLovinTick) && (p.CurJob == null)) {

                if (p.health.capacities.CanBeAwake && xxx.can_fuck(p)) {
                    var target = find_corpse(p, p.Map);
                    //Log.Message("   target = " + target + ", distance = " + p.Position.DistanceToSquared(target.Position));
                    if (target != null) {
                        return new Job(DefDatabase<JobDef>.GetNamed("ViolateCorpse"), target);
                    }
                }
			}
			
			return null;
		}
	}
}
