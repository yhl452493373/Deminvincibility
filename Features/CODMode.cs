using System.Threading.Tasks;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using UnityEngine;

namespace Deminvincibility.Features
{
    internal class CODModeComponent : MonoBehaviour
    {
        private Player player;
        private ActiveHealthController healthController;
        private float timeSinceLastHit;
        private bool isRegenerating;
        private float newHealRate;
        private DamageInfo tmpDmg;
        private ValueStruct currentHealth;

        private readonly EBodyPart[] bodyPartsDict =
        {
            EBodyPart.Stomach, EBodyPart.Chest, EBodyPart.Head, EBodyPart.RightLeg,
            EBodyPart.LeftLeg, EBodyPart.LeftArm, EBodyPart.RightArm
        };

        private static ManualLogSource Logger { get; set; }

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

                Logger.LogDebug("Deminvincibility: CODModeComponent enabled");
            }
        }

        private void Start()
        {
            player = Singleton<GameWorld>.Instance.MainPlayer;
            healthController = player.ActiveHealthController;

            player.OnPlayerDeadOrUnspawn += Player_OnPlayerDeadOrUnspawn;
            player.BeingHitAction += Player_BeingHitAction;
        }

        private async void Update()
        {
            if (DeminvicibilityPlugin.CODModeToggle.Value)
            {
                timeSinceLastHit += Time.unscaledDeltaTime;

                if (timeSinceLastHit >= DeminvicibilityPlugin.CODModeHealWait.Value)
                {
                    if (!isRegenerating)
                    {
                        isRegenerating = true;
                    }

                    await StartHealingAsync();
                }
            }
        }

        private async Task StartHealingAsync()
        {
            if (isRegenerating && DeminvicibilityPlugin.CODModeToggle.Value)
            {
                newHealRate = DeminvicibilityPlugin.CODModeHealRate.Value * Time.unscaledDeltaTime;

                foreach (var limb in bodyPartsDict)
                {
                    currentHealth = healthController.GetBodyPartHealth(limb, false);

                    if (!DeminvicibilityPlugin.CODBleedingDamageToggle.Value)
                    {
                        healthController.RemoveNegativeEffects(limb);
                    }

                    if (!currentHealth.AtMaximum)
                    {
                        healthController.ChangeHealth(limb, newHealRate, tmpDmg);
                    }
                }

                await Task.Yield();
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