
using System;
using System.Collections.Generic;
using System.Reflection;

using Verse;
using Verse.AI;
using RimWorld;

using Harmony;

namespace rjw {
	
	// Add a fail condition to JobDriver_Lovin that prevents pawns from lovin' if they aren't physically able
	[HarmonyPatch (typeof (JobDriver_Lovin))]
	[HarmonyPatch ("MakeNewToils")]
	static class PATCH_JobDriver_Lovin_MakeNewToils {
		[HarmonyPrefix]
		static bool on_begin_lovin (JobDriver_Lovin __instance)
		{
			var lov = __instance;
			lov.FailOn (() => (! xxx.can_fuck (lov.pawn)));
			return true;
		}
		
	}
	
	// Call xxx.aftersex after pawns have finished lovin'
	// You might be thinking, "wouldn't it be easier to add this code as a finish condition to JobDriver_Lovin in the patch above?" I tried that
	// at first but it didn't work because the finish condition is always called regardless of how the job ends (i.e. if it's interrupted or not)
    // and there's no way to find out from within the finish condition how the job ended. I want to make sure not apply the effects of sex if the
    // job was interrupted somehow.
	[HarmonyPatch (typeof (JobDriver))]
	[HarmonyPatch ("Cleanup")]
	static class PATCH_JobDriver_Cleanup {
		static Pawn find_partner (JobDriver_Lovin lov)
		{
			var any_ins = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return (Pawn)(typeof (JobDriver_Lovin).GetProperty ("Partner", any_ins).GetValue (lov, null));
		}
		
		[HarmonyPrefix]
		static bool on_cleanup_driver (JobDriver __instance, JobCondition condition)
		{
			var lov = __instance as JobDriver_Lovin;
			if ((lov != null) && (condition == JobCondition.Succeeded)) {
				var par = find_partner (lov);
				xxx.aftersex (lov.pawn, par);
				xxx.aftersex (par, lov.pawn); // We have to apply effects to both pawns here because, given how JobDriver_Lovin was written by Ludeon,
				                              // the job will only end successfully for the pawn that initiated lovin' and will fail for their partner.-
			}
			return true;
		}
	}

}
