using Comfort.Common;
using EFT;

namespace Deminvincibility.Features;

internal class MagazineSpeedComponent : BaseComponent
{
    private float baseLoadTime;
    private float baseUnloadTime;

    private void Start()
    {
        var backendConfigSettings = new BackendConfigSettingsClass();
        baseLoadTime = backendConfigSettings.BaseLoadTime;
        baseUnloadTime = backendConfigSettings.BaseUnloadTime;
        base.Start();
    }

    private void Update()
    {
        Singleton<BackendConfigSettingsClass>.Instance.BaseLoadTime =
            baseLoadTime * DeminvicibilityPlugin.MagazineSpeed.Value / 100;
        Singleton<BackendConfigSettingsClass>.Instance.BaseUnloadTime =
            baseUnloadTime * DeminvicibilityPlugin.MagazineSpeed.Value / 100;
    }

    public static void Enable()
    {
        if (Singleton<IBotGame>.Instantiated)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            gameWorld.GetOrAddComponent<MagazineSpeedComponent>();

            Logger.LogDebug("Deminvincibility: Reset Magazine Speed");
        }
    }
}