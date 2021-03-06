﻿
using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	
	[StaticConstructorOnStartup]
	internal static class comfort_prisoner_tex {
		public static Texture2D gizmo = ContentFinder<Texture2D>.Get("comfort_prisoner");
	}
    [StaticConstructorOnStartup]
    internal static class comfort_prisoner_invisible_tex
    {
        public static Texture2D gizmo = ContentFinder<Texture2D>.Get("comfort_prisoner_invisible");
    }
	
	public static class comfort_prisoners {
		
		public const int max_rapists_per_prisoner = 6;
		
		public static DesignationDef designation_def = DefDatabase<DesignationDef>.GetNamed ("ComfortPrisoner");

		public static DesignationDef designation_def_no_sticky = DefDatabase<DesignationDef>.GetNamed ("ComfortPrisonerNoSticky");
		
		public static bool is_designated (Pawn p)
		{
			foreach (var des in p.Map.designationManager.AllDesignationsOn (p))
				if (des.def == designation_def || des.def == designation_def_no_sticky)
					return true;
			
			return false;
		}
		
	}
	
}
