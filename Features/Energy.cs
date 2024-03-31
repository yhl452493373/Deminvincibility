using Comfort.Common;
using EFT;

namespace Deminvincibility.Features;

internal class EnergyComponent : BaseComponent
{
    private void Start()
    {
        base.Start();
        player.ActiveHealthController.EnergyChangedEvent += ActiveHealthController_EnergyChangedEvent;
    }

    private void ActiveHealthController_EnergyChangedEvent(float obj)
    {
        if (DeminvicibilityPlugin.MaxEnergyToggle.Value)
            player.ActiveHealthController.ChangeEnergy(player.ActiveHealthController.Energy.Maximum);
    }

    public static void Enable()
    {
        var gameWorld = Singleton<GameWorld>.Instance;
        gameWorld.GetOrAddComponent<EnergyComponent>();

        Logger.LogDebug("Deminvincibility: Setting Energy Component");
    }
}