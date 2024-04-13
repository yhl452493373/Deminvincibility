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
            // patch the method use by EquipmentClass.GetEquipmentWeight and EquipmentClass.GetEquipmentWeightEliteSkill
            return AccessTools.Method(typeof(EquipmentClass), nameof(EquipmentClass.method_11));
        }

        [PatchPostfix]
        private static void Postfix(EquipmentClass __instance, ref float __result)
        {
            // Get the total weight reduction setting
            float totalWeightReduction = DeminvicibilityPlugin.TotalWeightReductionPercentage.Value;

            // Convert it into a reduction factor: 0% -> full reduction (factor = 0), 100% -> no reduction (factor = 1)
            float reductionFactor = totalWeightReduction / 100f;

            // Apply the reduction factor
            __result *= reductionFactor;
        }
    }
}