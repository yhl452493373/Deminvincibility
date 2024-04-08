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
    internal class OnGameStarted : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchFinalizer]
        private static void Finalizer()
        {
            CODModeComponent.Enable();
            NoFallingDamageComponent.Enable();
            MaxStaminaComponent.Enable();
            HydrationComponent.Enable();
            EnergyComponent.Enable();
            MagazineSpeedComponent.Enable();
        }
    }
}