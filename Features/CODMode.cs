using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using UnityEngine;

namespace Deminvincibility.Features;

internal class CodModeComponent : BaseComponent
{
    private static ActiveHealthController healthController;
    private static float timeSinceLastHit;
    private static bool isRegenerating;
    private static float newHealRate;
    private static DamageInfo tmpDmg;
    private static HealthValue currentHealth;

    private static readonly EBodyPart[] bodyPartsDict =
    {
        EBodyPart.Stomach, EBodyPart.Chest, EBodyPart.Head, EBodyPart.RightLeg,
        EBodyPart.LeftLeg, EBodyPart.LeftArm, EBodyPart.RightArm
    };

    internal static void Enable()
    {
        if (Singleton<IBotGame>.Instantiated)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            gameWorld.GetOrAddComponent<CodModeComponent>();

            Logger.LogInfo("Deminvincibility: CODModeComponent enabled");
        }
    }

    private void Start()
    {
        base.Start();
        healthController = player.ActiveHealthController;
        isRegenerating = false;
        timeSinceLastHit = 0f;
        newHealRate = 0f;
        tmpDmg = new DamageInfo();
        currentHealth = null;

        player.OnPlayerDeadOrUnspawn += Player_OnPlayerDeadOrUnspawn;
        player.BeingHitAction += Player_BeingHitAction;
    }

    private void Update()
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

                StartHealing();
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
                }

                if (currentHealth.AtMaximum && DeminvicibilityPlugin.CODHealEffectsToggle.Value)
                {
                    healthController.RemoveNegativeEffects(limb);
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
        timeSinceLastHit = 0f;
        isRegenerating = false;
    }


    private void Player_OnPlayerDeadOrUnspawn(Player player)
    {
        Disable();
    }
}