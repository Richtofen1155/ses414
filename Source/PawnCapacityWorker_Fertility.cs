using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace rjw {
    public class PawnCapacityWorker_Fertility : PawnCapacityWorker {
        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null) {
            return PawnCapacityUtility.CalculateTagEfficiency(diffSet, "FertilitySource", 3.40282347E+38f, impactors);
        }
        public override bool CanHaveCapacity(BodyDef body) {
            return body.HasPartWithTag("FertilitySource");
        }
    }
}
