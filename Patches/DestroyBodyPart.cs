using System;
using System.Reflection;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;
#if SIT
using StayInTarkov;

#else
using Aki.Reflection.Patching;
#endif

namespace Deminvincibility.Patches
{
    internal class DestroyBodyPartPatch : ModulePatch
    {
        private static readonly EBodyPart[] critBodyParts = { EBodyPart.Stomach, EBodyPart.Head, EBodyPart.Chest };
        // private static DamageInfo tmpDmg;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.DestroyBodyPart));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, EDamageType damageType)
        {
            try
            {
                // Target is not our player - don't do anything
                if (__instance.Player == null || !__instance.Player.IsYourPlayer)
                {
                    return true;
                }

                //if CODMode is enabled
                if (DeminvicibilityPlugin.CODModeToggle.Value)
                {
                    // if BleedingDamage is disabled
                    if (!DeminvicibilityPlugin.CODBleedingDamageToggle.Value)
                    {
                        return false;
                    }
                    // if BleedingDamage is enabled, we don't want to destroy critical body parts
                    if (Array.Exists(critBodyParts, element => element == bodyPart))
                    {
                        return false;
                    }
                }

                // If Keep1Health is disabled, don't do anything
                if (!DeminvicibilityPlugin.Keep1Health.Value)
                {
                    return true;
                }

                // If AllowBlacking is disabled, we prevent the original method from running regardless
                if (!DeminvicibilityPlugin.AllowBlacking.Value)
                {
                    return false; // skip original method
                }

                // If limb blacking is enabled, but Head & Thorax are protected (and being currently hit), we'll skip the original method here
                if (!DeminvicibilityPlugin.AllowBlackingHeadAndThorax.Value &&
                    (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest))
                {
                    return false; // skip original method
                }


                // In all other cases, we let the original method run
                return true; // run original method
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            return true;
        }
    }
}