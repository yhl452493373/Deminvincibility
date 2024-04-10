using System.Collections.Generic;
using System.Timers;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using UnityEngine;
#if SIT
using AbstractIEffect = EFT.HealthSystem.ActiveHealthController.AbstractEffect;
#else
using AbstractIEffect = EFT.HealthSystem.ActiveHealthController.GClass2415;
#endif

namespace Deminvincibility.Features
{
    internal class CODModeComponent : BaseComponent
    {
        private ActiveHealthController healthController;
        private float timeSinceLastHit;
        private static readonly int healFreq = 16;
        private static readonly double timerInterval = 1000f / healFreq;
        private static readonly Timer timer = new(timerInterval);


        private readonly EBodyPart[] bodyPartsDict =
        {
            EBodyPart.Head, EBodyPart.Chest, EBodyPart.Stomach, EBodyPart.LeftArm,
            EBodyPart.RightArm, EBodyPart.LeftLeg, EBodyPart.RightLeg
        };

        internal static void Enable()
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            gameWorld.GetOrAddComponent<CODModeComponent>();

            Logger.LogDebug("Deminvincibility: CodModeComponent enabled");
        }

        private void Start()
        {
            base.Start();

            player.OnPlayerDeadOrUnspawn += Player_OnPlayerDeadOrUnspawn;
            player.BeingHitAction += Player_BeingHitAction;
            healthController = player.ActiveHealthController;
            healthController.EffectAddedEvent += HealthController_EffectAddedEvent;
            timer.AutoReset = true;
            timer.Elapsed += StartHealing;
        }

        private void HealthController_EffectAddedEvent(IEffect effect)
        {
            if (DeminvicibilityPlugin.CODModeToggle.Value && !DeminvicibilityPlugin.CODBleedingDamageToggle.Value)
            {
                // ActiveHealthController.cs: public void RemoveNegativeEffects
#if SIT
                if (!(effect is GInterface236) && !(effect is GInterface237))
                {
                    //IEffect4 is LightBleeding
                    //IEffect5 is HeavyBleeding
                    //IEffect7 is fracture
                    //IEffect21 is pain                    
#else
                if (!(effect is GInterface237) && !(effect is GInterface238))
                {
                    //GInterface242 is LightBleeding
                    //GInterface243 is HeavyBleeding
                    //GInterface245 is fracture
                    //GInterface259 is pain
#endif
                    //Effect is a Fracture, Bleeding, or Pain and has been removed
                    healthController.RemoveEffectFromList((AbstractIEffect)effect);
                }
            }
        }

        private void Update()
        {
            if (DeminvicibilityPlugin.CODModeToggle.Value)
            {
                timeSinceLastHit += Time.unscaledDeltaTime;

                if (timeSinceLastHit >= DeminvicibilityPlugin.CODModeHealWait.Value)
                {
                    if (!timer.Enabled)
                    {
                        timer.Start();
                    }
                }
            }
            else
            {
                timer.Stop();
            }
        }

        private void StartHealing(object sender, ElapsedEventArgs e)
        {
#if SIT
            Dictionary<EBodyPart, AHealthController<AbstractIEffect>.BodyPartState> injuredBodyParts = new();
#else
            Dictionary<EBodyPart, GClass2416<AbstractIEffect>.BodyPartState> injuredBodyParts = new();
#endif
            foreach (var limb in bodyPartsDict)
            {
                var bodyPartState = healthController.Dictionary_0[limb];
                var currentHealth = bodyPartState.Health;

                // ignore destroyed body
                if (bodyPartState.IsDestroyed)
                {
                    continue;
                }

                if (currentHealth.AtMaximum)
                {
                    if (DeminvicibilityPlugin.CODHealEffectsToggle.Value)
                    {
                        healthController.RemoveNegativeEffects(limb);
                    }

                    continue;
                }

                injuredBodyParts.Add(limb, bodyPartState);
            }

            if (!injuredBodyParts.IsNullOrEmpty())
            {
                float healRate = DeminvicibilityPlugin.CODModeHealRate.Value / injuredBodyParts.Count / healFreq;

                foreach (var injuredBodyPart in injuredBodyParts)
                {
                    var bodyPartState = injuredBodyPart.Value;
                    // ignore destroyed body
                    if (bodyPartState.IsDestroyed)
                    {
                        continue;
                    }

                    var health = bodyPartState.Health;
                    health.Current += healRate;
                    if (health.AtMaximum)
                    {
                        if (DeminvicibilityPlugin.CODHealEffectsToggle.Value)
                        {
                            healthController.RemoveNegativeEffects(injuredBodyPart.Key);
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
                healthController.EffectAddedEvent -= HealthController_EffectAddedEvent;
                timer.Stop();
            }
        }

        private void Player_BeingHitAction(DamageInfo arg1, EBodyPart arg2, float arg3)
        {
            timeSinceLastHit = 0f;
            timer.Stop();
        }

        private void Player_OnPlayerDeadOrUnspawn(Player player)
        {
            Disable();
        }
    }
}