using System.Reflection;
using Deminvincibility.Features;
using EFT;
#if SIT
using StayInTarkov;
#else
using Aki.Reflection.Patching;
#endif

namespace Deminvincibility.Patches
{
    internal class AfterGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchFinalizer]
        private static void Finalizer()
        {
            CodModeComponent.Enable();
            NoFallingDamageComponent.Enable();
            MaxStaminaComponent.Enable();
            HydrationComponent.Enable();
            EnergyComponent.Enable();
            MagazineSpeedComponent.Enable();
        }
    }
}