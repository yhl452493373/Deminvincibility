using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using UnityEngine;

namespace Deminvincibility.Features
{
    internal class CODModeComponent : MonoBehaviour
    {
        private static Player player;
        private static ActiveHealthController healthController;
        private static float timeSinceLastHit = 0f;
        private static bool isRegenerating = false;
        private static float newHealRate;
        private static DamageInfo tmpDmg;
        private static HealthValue currentHealth;
        private static int frameCount = 0;

        private static readonly EBodyPart[] bodyPartsDict =
        {
            EBodyPart.Stomach, EBodyPart.Chest, EBodyPart.Head, EBodyPart.RightLeg,
            EBodyPart.LeftLeg, EBodyPart.LeftArm, EBodyPart.RightArm
        };

        protected static ManualLogSource Logger { get; private set; }

        private CODModeComponent()
        {
            if (Logger == null)
            {
                Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(CODModeComponent));
            }
        }

        internal static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                var gameWorld = Singleton<GameWorld>.Instance;
                gameWorld.GetOrAddComponent<CODModeComponent>();

                Logger.LogDebug("DadGamerMode: CODModeComponent enabled");
            }
        }

        private void Start()
        {
            player = Singleton<GameWorld>.Instance.MainPlayer;
            healthController = player.ActiveHealthController;
            isRegenerating = false;
            timeSinceLastHit = 0f;
            newHealRate = 0f;
            tmpDmg = new DamageInfo();
            currentHealth = null;
            frameCount = 0;

            player.OnPlayerDeadOrUnspawn += Player_OnPlayerDeadOrUnspawn;
            player.BeingHitAction += Player_BeingHitAction;
        }

        private void Update()
        {
            if (DeminvicibilityPlugin.CODModeToggle.Value)
            {
                frameCount++;
                timeSinceLastHit += Time.unscaledDeltaTime;

                if (frameCount >= 60) // Check every 60 frames instead
                {
                    frameCount = 0;

                    if (timeSinceLastHit >= DeminvicibilityPlugin.CODModeHealWait.Value)
                    {
                        if (!isRegenerating)
                        {
                            isRegenerating = true;
                        }

                        StartHealing();
                    }
                }
            }
        }

        private void StartHealing()
        {
            if (isRegenerating && DeminvicibilityPlugin.CODModeToggle.Value)
            {
                newHealRate = DeminvicibilityPlugin.CODModeHealRate.Value;

                foreach (var limb in bodyPartsDict)
                {
                    currentHealth = healthController.Dictionary_0[limb].Health;

                    if (!currentHealth.AtMaximum)
                    {
                        currentHealth.Current += newHealRate;
                        
                        if (DeminvicibilityPlugin.CODHealEffectsToggle.Value)
                        {
                            healthController.RemoveNegativeEffects(limb);
                        }
                    }
                }
            }
        }

        private void Disable()
        {
            if (player != null)
            {
                player.OnPlayerDeadOrUnspawn -= Player_OnPlayerDeadOrUnspawn;
                player.BeingHitAction -= Player_BeingHitAction;
            }
        }

        private void Player_BeingHitAction(DamageInfo arg1, EBodyPart arg2, float arg3)
        {
            //Logger.LogDebug("DadGamerMode: Player_BeingHitAction called");
            timeSinceLastHit = 0f;
            isRegenerating = false;
        }


        private void Player_OnPlayerDeadOrUnspawn(Player player)
        {
            Disable();
        }
    }
}