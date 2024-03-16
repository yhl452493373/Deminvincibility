using System.Reflection;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace Deminvincibility.Features

{
    internal class MagReloadSpeed : MonoBehaviour
    {
        private Player player;
        private float baseLoadTime;
        private float baseUnloadTime;

        private void Start()
        {
            player = Singleton<GameWorld>.Instance.MainPlayer;
            var backendConfigSettings = new BackendConfigSettingsClass();
            baseLoadTime = backendConfigSettings.BaseLoadTime;
            baseUnloadTime = backendConfigSettings.BaseUnloadTime;
        }

        private void Update()
        {
            Singleton<BackendConfigSettingsClass>.Instance.BaseLoadTime = baseLoadTime * DeminvicibilityPlugin.MagazineSpeed.Value / 100;
            Singleton<BackendConfigSettingsClass>.Instance.BaseUnloadTime = baseUnloadTime * DeminvicibilityPlugin.MagazineSpeed.Value / 100;
        }

        public static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                gameWorld.GetOrAddComponent<MagReloadSpeed>();
            }
        }
    }
}