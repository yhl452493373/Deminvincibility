using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using UnityEngine;

namespace Deminvincibility.Features;

internal class CodModeComponent : BaseComponent
{
    private ActiveHealthController healthController;
    private float timeSinceLastHit;
    private bool isRegenerating;
    private float newHealRate;
    private DamageInfo tmpDmg;
    private HealthValue currentHealth;

    private readonly EBodyPart[] bodyPartsDict =
    {
        EBodyPart.Stomach, EBodyPart.Chest, EBodyPart.Head, EBodyPart.RightLeg,
        EBodyPart.LeftLeg, EBodyPart.LeftArm, EBodyPart.RightArm
    };

    internal static void Enable()
    {
        var gameWorld = Singleton<GameWorld>.Instance;
        gameWorld.GetOrAddComponent<CodModeComponent>();

        Logger.LogInfo("Deminvincibility: CODModeComponent enabled");
    }

    private void Start()
    {
        base.Start();
        healthController = player.ActiveHealthController;

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

                if (DeminvicibilityPlugin.CODHealEffectsToggle.Value)
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