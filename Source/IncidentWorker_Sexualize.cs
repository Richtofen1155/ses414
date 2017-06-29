
using System;
using System.Collections.Generic;

using Verse;
using RimWorld;

namespace rjw {
	public class IncidentWorker_Sexualize : IncidentWorker {

		public override bool TryExecute (IncidentParms parms)
		{
			xxx.sexualize_everyone ();
			Find.LetterStack.ReceiveLetter ("Sexualization Complete", "All pawns have been sexualized.", LetterDefOf.Good, null);
			return true;
		}
		
	}
}
