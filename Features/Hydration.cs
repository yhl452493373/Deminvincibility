using Comfort.Common;
using EFT;

namespace Deminvincibility.Features;

internal class HydrationComponent : BaseComponent
{
    private void Start()
    {
        base.Start();
        //setup event so it doesn't have to check every frame
        player.ActiveHealthController.HydrationChangedEvent += ActiveHealthController_HydrationChangedEvent;
    }

    private void ActiveHealthController_HydrationChangedEvent(float obj)
    {
        if (DeminvicibilityPlugin.MaxHydrationToggle.Value)
            player.ActiveHealthController.ChangeHydration(player.ActiveHealthController.Hydration.Maximum);
    }


    public static void Enable()
    {
        var gameWorld = Singleton<GameWorld>.Instance;
        gameWorld.GetOrAddComponent<HydrationComponent>();

        Logger.LogDebug("Deminvincibility: Setting Max Hydration");
    }
}