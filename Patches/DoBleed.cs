﻿using System.Reflection;
using EFT.HealthSystem;
using HarmonyLib;
using System;
#if SIT
using StayInTarkov;
#else
using Aki.Reflection.Patching;
#endif

namespace Deminvincibility.Patches
{
    internal class DoBleed : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.DoBleed));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart)
        {
            try
            {
                // Target is not our player - don't do anything
                if (__instance.Player == null || !__instance.Player.IsYourPlayer)
                {
                    return true;
                }

                if (DeminvicibilityPlugin.Keep1Health.Value && DeminvicibilityPlugin.MedicineBool.Value)
                {
                    Logger.LogMessage("DoBleed: Keep1Health & MedicineBool RETURN FALSE");
                    return false;
                }
                else
                {
                    Logger.LogMessage("DoBleed: Keep1Health & MedicineBool RETURN TRUE");
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            return true;
        }
    }
}