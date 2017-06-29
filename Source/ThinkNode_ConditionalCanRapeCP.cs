
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalCanRapeCP : ThinkNode_Conditional
	{
		protected override bool Satisfied (Pawn pawn)
		{
			if (pawn.Faction == null) {
				return false;
			}

            //return (xxx.config.comfort_prisoners_enabled && pawn.Faction.IsPlayer /*&& pawn.IsColonist*/ && xxx.can_fuck (pawn) && (pawn.Map != null));
            // Allow pawns from any faction to rape comfort prisoners
            return (xxx.config.comfort_prisoners_enabled /*&& pawn.Faction.IsPlayer && pawn.IsColonist*/ && xxx.can_fuck(pawn) && (pawn.Map != null));
        }
	}
}
