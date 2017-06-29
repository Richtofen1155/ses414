
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class ThinkNode_ConditionalHorny : ThinkNode_Conditional {

		protected override bool Satisfied (Pawn p)
		{
			var need_sex = p.needs.TryGetNeed<Need_Sex> ();
			return ((need_sex != null) && (need_sex.CurLevel <= need_sex.thresh_horny ()));
		}

	}
}
