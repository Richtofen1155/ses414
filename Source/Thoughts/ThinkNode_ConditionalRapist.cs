
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalRapist : ThinkNode_Conditional {

		protected override bool Satisfied (Pawn p)
		{
            return xxx.config.random_rape_enabled && xxx.is_rapist(p);
		}

	}
}
