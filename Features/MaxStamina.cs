using Comfort.Common;
using EFT;

namespace Deminvincibility.Features;

internal class MaxStaminaComponent : BaseComponent
{
    private void Update()
    {
        if (DeminvicibilityPlugin.MaxStaminaToggle.Value)
        {
            player.Physical.Stamina.Current = player.Physical.Stamina.TotalCapacity.Value;
            player.Physical.HandsStamina.Current = player.Physical.HandsStamina.TotalCapacity.Value;
            player.Physical.Oxygen.Current = player.Physical.Oxygen.TotalCapacity.Value;
        }
    }

    public static void Enable()
    {
        var gameWorld = Singleton<GameWorld>.Instance;
        gameWorld.GetOrAddComponent<MaxStaminaComponent>();

        Logger.LogDebug("Deminvincibility: Setting Max Stamina");
    }
}