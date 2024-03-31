using Comfort.Common;
using EFT;

namespace Deminvincibility.Features;

internal class NoFallingDamageComponent : BaseComponent
{
    private void Start()
    {
        base.Start();
        Logger.LogInfo("Deminvincibility: Setting No Falling Damage");
    }

    private void Update()
    {
        player.ActiveHealthController.FallSafeHeight = DeminvicibilityPlugin.NoFallingDamage.Value ? 999999f : 1.8f;
    }

    internal static void Enable()
    {
        var gameWorld = Singleton<GameWorld>.Instance;
        gameWorld.GetOrAddComponent<NoFallingDamageComponent>();
    }

    private static void Disable()
    {
        if (!DeminvicibilityPlugin.NoFallingDamage.Value)
        {
            var gameWorld = Singleton<GameWorld>.Instance;

            var player = gameWorld.MainPlayer;
            Logger.LogDebug("Deminvincibility: Setting Falling Damage To Normal");

            player.ActiveHealthController.FallSafeHeight = 1.8f;
        }
    }
}