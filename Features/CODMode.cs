using System.Collections.Generic;
using System.Timers;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using UnityEngine;
#if SPT_3_8_0 && SIT
using AbstractIEffect = EFT.HealthSystem.ActiveHealthController.AbstractEffect;
#elif SPT_3_9_0 && SIT
using AbstractIEffect = EFT.HealthSystem.ActiveHealthController.GClass2425;
#elif SPT_3_8_0
using AbstractIEffect = EFT.HealthSystem.ActiveHealthController.GClass2415;
#elif SPT_3_9_0
using AbstractIEffect = EFT.HealthSystem.ActiveHealthController.GClass2427;
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
                // Search for the following text in ActiveHealthController.cs
                // public void RemoveNegativeEffects
                // You will get such text: !(effect is GInterfaceXXX) && !(effect is GInterfaceYYY) 
                // !(effect is GInterfaceXXX) && !(effect is GInterfaceYYY) is the condition
                // Additional:
                // For class LightBleeding, IEffectX or GInterfaceXXX is the Sixth Inherited Object
                // For class HeavyBleeding, IEffectX or GInterfaceXXX is the Sixth Inherited Object
                // For class Fracture, IEffectX or GInterfaceXXX is the Sixth Inherited Object
                // For class Pain, IEffectX or GInterfaceXXX is the Fifth Inherited Object
#if SPT_3_8_0 && SIT
                if (!(effect is GInterface236) && !(effect is GInterface237))
                {
#elif SPT_3_9_0 && SIT
                if (!(effect is GInterface251) && !(effect is GInterface252))
                {
#elif SPT_3_8_0
                if (!(effect is GInterface237) && !(effect is GInterface238))
                {
#elif SPT_3_9_0
                if (!(effect is GInterface251) && !(effect is GInterface252))
                {
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
            // Search for the following text in ActiveHealthController.cs.
            // >.BodyPartState
            // You will get such text: A<ActiveHealthController.B>.BodyPartState
            // GClassXXXX is A
            // AbstractIEffect is B
#if SPT_3_8_0 && SIT
            Dictionary<EBodyPart, AHealthController<AbstractIEffect>.BodyPartState> injuredBodyParts = new();
#elif SPT_3_9_0 && SIT
            Dictionary<EBodyPart, GClass2426<AbstractIEffect>.BodyPartState> injuredBodyParts = new();
#elif SPT_3_8_0
            Dictionary<EBodyPart, GClass2416<AbstractIEffect>.BodyPartState> injuredBodyParts = new();
#elif SPT_3_9_0
            Dictionary<EBodyPart, GClass2428<AbstractIEffect>.BodyPartState> injuredBodyParts = new();
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