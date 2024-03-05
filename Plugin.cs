using System.Reflection;
using Aki.Reflection.Patching;
using BepInEx;
using BepInEx.Configuration;
using Deminvincibility.Model;
using Deminvincibility.Patches;
using Deminvincibility.Features;
using EFT;

namespace Deminvincibility
{
    [BepInPlugin("com.hazelify.deminvincibility", "Deminvincibility", "1.5.0")]
    public class DeminvicibilityPlugin : BaseUnityPlugin
    {
        private static bool placeholder = true;
        private static string credits = "Thanks Props <3 Ily https://github.com/dvize/DadGamerMode";

        public static ConfigEntry<bool> Keep1Health { get; set; }
        public static ConfigEntry<bool> Allow0HpLimbs { get; set; }
        public static ConfigEntry<bool> AllowBlacking { get; set; }
        public static ConfigEntry<bool> AllowBlackingHeadAndThorax { get; set; }
        public static ConfigEntry<bool> MedicineBool { get; set; }
        public static ConfigEntry<int> CustomDamageModeVal { get; set; }
        public static ConfigEntry<bool> SecondChanceProtection { get; set; }
        public static ConfigEntry<bool> SecondChanceEffectRemoval { get; set; }
        public static ConfigEntry<bool> NoFallingDamage { get; set; }
        public static ConfigEntry<bool> MaxStaminaToggle { get; set; }

        public static ConfigEntry<bool> CODModeToggle { get; set; }
        public static ConfigEntry<float> CODModeHealRate { get; set; }
        public static ConfigEntry<float> CODModeHealWait { get; set; }
        public static ConfigEntry<bool> CODBleedingDamageToggle { get; set; }

        public static ConfigEntry<SecondChanceRestoreEnum> SecondChanceHealthRestoreAmount { get; set; }

        private void Awake()
        {
            InitConfig();
            ApplyPatches();
        }

        private void ApplyPatches()
        {
            new NewGamePatch().Enable();
            new DestroyBodyPartPatch().Enable();
            new ApplyDamage().Enable();
            new DoFracture().Enable();
            new Kill().Enable();
        }

        private void InitConfig()
        {
            // - Keep1Health section
            if (placeholder)
            {
                Keep1Health = Config.Bind("1. Health", "1 HP Mode", false,
                    new ConfigDescription("Enable to keep yourself from dying",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 7 }));

                Allow0HpLimbs = Config.Bind("1. Health", "Allow 0hp on limbs", false,
                    new ConfigDescription("If enabled, Keep 1 Health on will allow arms and legs to hit 0 hp.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 6 }));

                AllowBlacking = Config.Bind("1. Health", "Allow blacking of limbs", false,
                    new ConfigDescription(
                        "If enabled, Keep 1 Health on will cause blacking of limbs when they reach 0hp.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 5 }));

                AllowBlackingHeadAndThorax = Config.Bind("1. Health", "Allow blacking of Head & Thorax", false,
                    new ConfigDescription(
                        "If enabled, Head & Thorax will be blacked out when they reach 0hp. This setting is ignored if \'Allow blacking of limbs\' is disabled.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 4 }));

                MedicineBool = Config.Bind("1. Health", "Ignore health side effects?", false,
                    new ConfigDescription(
                        "If enabled, fractures, bleeds and other forms of side effects to your health will be auto-healed.\n\nPSA: Disabling this could cause unintended side effects, use caution.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

                CustomDamageModeVal = Config.Bind("1. Health", "% Damage received", 100, new ConfigDescription(
                    "Set perceived damage in percent",
                    new AcceptableValueRange<int>(1, 100),
                    new ConfigurationManagerAttributes
                        { IsAdvanced = false, ShowRangeAsPercent = true, Order = 2 }));

                NoFallingDamage = Config.Bind("1. Health", "No Falling Damage", false, new ConfigDescription(
                    "No Falling Damage",
                    null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 2 }));

                MaxStaminaToggle = Config.Bind("1. Health", "Infinite Stamina", false, new ConfigDescription(
                    "Stamina Never Drains",
                    null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));
            }

            // Protection disable
            if (placeholder)
            {
                SecondChanceProtection = Config.Bind("2. Death", "Enable second chance protection?", false,
                    new ConfigDescription(
                        "If enabled, if you take a hit that would normally kill you, you\'ll be given a second chance to survive.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

                SecondChanceEffectRemoval = Config.Bind("2. Death", "Second chance health effect removal", true,
                    new ConfigDescription(
                        "If enabled, all negative health effects (bleeds, fractures, etc.) will be removed from your character when second chance triggers.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 2 }));

                SecondChanceHealthRestoreAmount = Config.Bind("2. Death",
                    "Second chance health restore on critical limbs", SecondChanceRestoreEnum.OneHealth,
                    new ConfigDescription(
                        "Choose what HP should be set on the Head and Thorax when second chance protection triggers. Will never remove health if the limb has more than the set amount. \n\n\'None\' and \'1HP\' options are risky if effect removal is disabled.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));
            }

            // COD Mode
            if (placeholder)
            {
                CODModeToggle = Config.Bind("3. COD", "CODMode", false, new ConfigDescription(
                    "Gradually heals all your damage over time including bleeds and fractures",
                    null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 4 }));

                CODModeHealRate = Config.Bind("3. COD", "CODMode Heal Rate", 10f, new ConfigDescription(
                    "Sets How Fast You Heal",
                    new AcceptableValueRange<float>(0f, 100f),
                    new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 3 }));

                CODModeHealWait = Config.Bind("3. COD", "CODMode Heal Wait", 10f, new ConfigDescription(
                    "Sets How Long You Have to Wait in Seconds with no damage before healing starts",
                    new AcceptableValueRange<float>(0f, 600f),
                    new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 2 }));

                CODBleedingDamageToggle = Config.Bind("3. COD", "CODMode Bleeding Damage", false, new ConfigDescription(
                    "You still get bleeding and fractures for COD Mode",
                    null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));
            }
        }

        internal class NewGamePatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod() =>
                typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));

            [PatchPrefix]
            private static void PatchPrefix()
            {
                CODModeComponent.Enable();
                NoFallingDamageComponent.Enable();
                MaxStaminaComponent.Enable();
            }
        }

        internal sealed class ConfigurationManagerAttributes
        {
            public bool? ShowRangeAsPercent;
            public System.Action<ConfigEntryBase> CustomDrawer;
            public CustomHotkeyDrawerFunc CustomHotkeyDrawer;

            public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting,
                ref bool isCurrentlyAcceptingInput);

            public bool? Browsable;
            public string Category;
            public object DefaultValue;
            public bool? HideDefaultButton;
            public bool? HideSettingName;
            public string Description;
            public string DispName;
            public int? Order;
            public bool? ReadOnly;
            public bool? IsAdvanced;
            public System.Func<object, string> ObjToStr;
            public System.Func<string, object> StrToObj;
        }
    }
}