using System.Collections.Generic;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace rjw
{
    public class Building_Bed_Patch
    {
        [HarmonyPatch(typeof(Building_Bed))]
        [HarmonyPatch("ForPrisoners", PropertyMethod.Setter)]
        public class ForPrisoners
        {
            [HarmonyPostfix]
            public static void Postfix(Building_Bed __instance)
            {
                if (!__instance.ForPrisoners) return;

                if (__instance is Building_WhoreBed)
                {
                    Building_WhoreBed.Swap(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(Building_Bed), "GetGizmos")]
        public class GetGizmos
        {
            [HarmonyPostfix]
            public static void Postfix(Building_Bed __instance, ref IEnumerable<Gizmo> __result)
            {
                __result = Process(__instance, __result);
            }

            private static IEnumerable<Gizmo> Process(Building_Bed __instance, IEnumerable<Gizmo> __result)
            {
                foreach (var gizmo in __result)
                {
                    yield return gizmo;
                }
                if (!__instance.ForPrisoners && !__instance.Medical)
                {
                    yield return
                        new Command_Toggle
                        {
                            defaultLabel = "CommandBedSetAsWhoreLabel".Translate(),
                            defaultDesc = "CommandBedSetAsWhoreDesc".Translate(),
                            icon = ContentFinder<Texture2D>.Get("UI/Commands/AsWhore"),
                            isActive = () => false,
                            toggleAction = () => Building_WhoreBed.Swap(__instance),
                            hotKey = KeyBindingDefOf.Misc4
                        };
                }
            }
        }
    }
}