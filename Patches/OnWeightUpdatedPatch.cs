using System.Reflection;
using HarmonyLib;
#if SIT
using StayInTarkov;
#else
using Aki.Reflection.Patching;
#endif

namespace Deminvincibility.Patches
{
    internal class OnWeightUpdatedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
#if SIT
            return AccessTools.Method(typeof(Physical), nameof(Physical.OnWeightUpdated));
#else
            return AccessTools.Method(typeof(GClass681), nameof(GClass681.OnWeightUpdated));
#endif
        }

        [PatchPrefix]
        static bool Prefix(ref float ___totalWeight)
        {
            float weightReductionMultiplier = 1f - (DeminvicibilityPlugin.TotalWeightReductionPercentage.Value / 100f);

            ___totalWeight *= weightReductionMultiplier;

            return true;
        }
    }
}