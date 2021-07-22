using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;

namespace AltAI
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        // this static constructor runs to create a HarmonyInstance and install a patch.
        static HarmonyPatches()
        {
            var harmony = new Harmony("rimworld.nilchei.altai");

            // find the FillTab method of the class RimWorld.ITab_Pawn_Character
            MethodInfo targetmethod = AccessTools.Method(typeof(Verse.Verb), "CanHitTargetFrom");

            // find the static method to call before (i.e. Prefix) the targetmethod
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(AltAI.HarmonyPatches).GetMethod("CanHitTargetFrom_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, prefixmethod, null);
        }

        // This method is now always called right before RimWorld.ITab_Pawn_Character.FillTab.
        // So, before the ITab_Pawn_Character is instantiated, reset the height of the dialog window.
        // The class RimWorld.ITab_Pawn_Character is static so there is no this __instance.
        public static bool CanHitTargetFrom_Prefix(Verb __instance, IntVec3 root, LocalTargetInfo targ, out bool __result)
        {
            if (targ.Thing != null && targ.Thing == __instance.caster)
            {
                __result = __instance.verbProps.targetParams.canTargetSelf;
                return false;
            }
            if (__instance.CasterIsPawn && __instance.CasterPawn.apparel != null)
            {
                List<Apparel> wornApparel = __instance.CasterPawn.apparel.WornApparel;
                for (int i = 0; i < wornApparel.Count; i++)
                {
                    if (!wornApparel[i].AllowVerbCast(root, __instance.caster.Map, targ, __instance))
                    {
                        __result = false;
                        return false;
                    }
                }
            }

            List<Thing> list = __instance.caster.Map.listerThings.ThingsOfDef(ThingDefOf.Turret_Mortar);
            for (int i = 0; i < list.Count; i++)
            {
                if (__instance.caster.Position.InHorDistOf(list[i].Position, 1f) && targ.Cell.GetRoof(__instance.caster.Map).isThickRoof)
                {
                    __result = false;
                    return false;
                }
            }

            ShootLine shootLine;
            __result = __instance.TryFindShootLineFromTo(root, targ, out shootLine);
            return false;
        }
    }
}