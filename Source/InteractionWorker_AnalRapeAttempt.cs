using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace rjw {
    class InteractionWorker_AnalRapeAttempt : InteractionWorker {
        
        public override float RandomSelectionWeight(Pawn rapist, Pawn victim) {
            // this interaction is triggered by the jobdriver
            if (rapist == null || victim == null) return 0.0f;
            return 0.0f; // base.RandomSelectionWeight(initiator, recipient);
        }
        public override void Interacted(Pawn rapist, Pawn victim, List<RulePackDef> extraSentencePacks) {
            if (rapist == null || victim == null) return;
            Log.Message("[RJW] InteractionWorker_AnalRapeAttempt::Interacted( " + rapist.NameStringShort + ", " + victim.NameStringShort + " ) called");
            xxx.AttemptAnalRape(rapist, victim);
        }
    }
}
