
using System;
using System.Collections.Generic;
using System.Reflection;

using Verse;
using RimWorld;

namespace rjw {
	public class IncidentWorker_NymphJoins : IncidentWorker {

        
		protected override bool CanFireNowSub (IIncidentTarget target)
		{
            return true;
            
			if (HugsLibInj.nymphos) {
				var m = (Map)target;
				var colonist_count = 0;
				var nymph_count = 0;
				foreach (var p in m.mapPawns.FreeColonists) {
					++colonist_count;
					if (xxx.is_nympho (p))
						++nymph_count;
				}
				var nymph_fraction = (float)nymph_count / (float)colonist_count;
				return (colonist_count >= 1) && (nymph_fraction < xxx.config.max_nymph_fraction);
			} else
				return false;
		}
		
		public override bool TryExecute (IncidentParms parms)
		{
			if (!HugsLibInj.nymphos)
			{
				return false;
			}

			var m = (Map)parms.target;
			
			IntVec3 loc;
			if (! CellFinder.TryFindRandomEdgeCellWith (m.reachability.CanReachColony, m, 1.0f, out loc)) // TODO check this ROADCHANCE
				return false;

			var p = nymph_generator.spawn_new (loc, m);
			
			Find.LetterStack.ReceiveLetter ("Nymph Joins", "A wandering nymph has decided to join your colony.", LetterDefOf.Good, p);
			
			return true;
		}
		
	}
}
