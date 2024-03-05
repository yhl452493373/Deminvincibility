using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace Deminvincibility.Features
{
    internal class NoFallingDamageComponent : MonoBehaviour
    {
        private Player player;
        protected static ManualLogSource Logger { get; private set; }

        private NoFallingDamageComponent()
        {
            if (Logger == null)
            {
                Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(NoFallingDamageComponent));
            }
        }

        private void Start()
        {
            player = Singleton<GameWorld>.Instance.MainPlayer;
            Logger.LogDebug("DadGamerMode: Setting No Falling Damage");
        }

        private void Update()
        {
            player.ActiveHealthController.FallSafeHeight = DeminvicibilityPlugin.NoFallingDamage.Value ? 999999f : 1.8f;
        }

        internal static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                gameWorld.GetOrAddComponent<NoFallingDamageComponent>();
            }
        }

        private static void Disable()
        {
            if (!DeminvicibilityPlugin.NoFallingDamage.Value)
            {
                var gameWorld = Singleton<GameWorld>.Instance;

                var player = gameWorld.MainPlayer;
                Logger.LogDebug("DadGamerMode: Setting Falling Damage To Normal");

                player.ActiveHealthController.FallSafeHeight = 1.8f;
            }
        }
    }
}