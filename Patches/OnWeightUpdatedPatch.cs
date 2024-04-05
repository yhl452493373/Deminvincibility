using System.Reflection;
using Comfort.Common;
using HarmonyLib;
using UnityEngine;
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
#if SIT
        private static bool Prefix(Physical __instance)
#else
        private static bool Prefix(GClass681 __instance)
#endif
        {
#if SIT
            var iobserverToPlayerBridge_0 =
                AccessTools.FieldRefAccess<Physical, Physical.IObserverToPlayerBridge>(__instance,
                    "iobserverToPlayerBridge_0");
#else
            var iobserverToPlayerBridge_0 =
                AccessTools.FieldRefAccess<GClass681, GClass681.IObserverToPlayerBridge>(__instance,
                    "iobserverToPlayerBridge_0");
#endif

            if (iobserverToPlayerBridge_0.iPlayer.IsYourPlayer)
            {
#if SIT
                var float_3 = AccessTools.FieldRefAccess<Physical, float>(__instance, "float_3");
#else
                var float_3 = AccessTools.FieldRefAccess<GClass681, float>(__instance, "float_3");
#endif
                //modify the weight by our TotalWeightReductionPercentage(int converted to percentage)
                float totalWeight = iobserverToPlayerBridge_0.TotalWeight *
                                    (DeminvicibilityPlugin.TotalWeightReductionPercentage.Value / 100f);

                BackendConfigSettingsClass.InertiaSettings inertia =
                    Singleton<BackendConfigSettingsClass>.Instance.Inertia;
                __instance.Inertia = __instance.CalculateValue(__instance.BaseInertiaLimits, totalWeight);
                __instance.SprintAcceleration = inertia.SprintAccelerationLimits.InverseLerp(__instance.Inertia);
                __instance.PreSprintAcceleration = inertia.PreSprintAccelerationLimits.Evaluate(__instance.Inertia);
                float num = Mathf.Lerp(inertia.MinMovementAccelerationRangeRight.x,
                    inertia.MaxMovementAccelerationRangeRight.x, __instance.Inertia);
                float num2 = Mathf.Lerp(inertia.MinMovementAccelerationRangeRight.y,
                    inertia.MaxMovementAccelerationRangeRight.y, __instance.Inertia);
                EFTHardSettings.Instance.MovementAccelerationRange.MoveKey(1, new Keyframe(num, num2));
                __instance.Overweight = __instance.BaseOverweightLimits.InverseLerp(totalWeight);
                __instance.WalkOverweight = __instance.WalkOverweightLimits.InverseLerp(totalWeight);
                float_3 = __instance.SprintOverweightLimits.InverseLerp(totalWeight);
                __instance.WalkSpeedLimit = 1f - __instance.WalkSpeedOverweightLimits.InverseLerp(totalWeight);
                __instance.MoveSideInertia = inertia.SideTime.Evaluate(__instance.Inertia);
                __instance.MoveDiagonalInertia = inertia.DiagonalTime.Evaluate(__instance.Inertia);
                if (iobserverToPlayerBridge_0.iPlayer.IsAI)
                {
                    float_3 = 0f;
                }

                __instance.FallDamageMultiplier = Mathf.Lerp(1f, __instance.StaminaParameters.FallDamageMultiplier,
                    __instance.Overweight);
                __instance.SoundRadius = __instance.StaminaParameters.SoundRadius.Evaluate(__instance.Overweight);
                __instance.MinStepSound.SetDirty();
                __instance.TransitionSpeed.SetDirty();
                __instance.method_3();
                __instance.method_7(totalWeight);

                return false;
            }

            return true;
        }
    }
}