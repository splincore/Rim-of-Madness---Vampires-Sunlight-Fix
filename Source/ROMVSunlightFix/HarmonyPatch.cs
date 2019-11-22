using Verse;
using Harmony;
using RimWorld;
using Vampire;
using System.Collections.Generic;

namespace ROMVSunlightFix
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = HarmonyInstance.Create("rimworld.carnysenpai.romvsunlightfix");
            harmony.Patch(AccessTools.Method(typeof(Scenario), "Notify_PawnGenerated", null, null), null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Notify_PawnGenerated_PostFix")), null);
        }

        [HarmonyPostfix]
        public static void Notify_PawnGenerated_PostFix(Pawn pawn)
        {
            if (pawn.IsVampire())
            {
                Map currentMap = Find.CurrentMap;
                if (currentMap == null) return;
                if (VampireUtility.IsDaylight(currentMap) && pawn.Faction != Faction.OfPlayerSilentFail)
                {
                    pawn.health.hediffSet.hediffs.RemoveAll(h=> h.def == VampDefOf.ROM_Vampirism);
                    Dictionary<Pawn, int> recentVampires = Find.World.GetComponent<WorldComponent_VampireTracker>().recentVampires;
                    if (recentVampires != null && recentVampires.ContainsKey(pawn))
                    {
                        recentVampires.Remove(pawn);
                    }
                }
            }
        }
    }
}