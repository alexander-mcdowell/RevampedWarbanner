using BepInEx;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;
using System.Text;

namespace RevampedWarbanner {
    // Declare R2API as a dependency.
    [BepInDependency(R2API.R2API.PluginGUID)]
    // For BetterUI integration
    [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]

    // Metadata
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // Declares LanguageAPI dependency
    [R2APISubmoduleDependency(nameof(LanguageAPI))]

    // Declare plugin class.
    public class RevampedWarbanner : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "SolidwoodFailures";
        public const string PluginName = "RevampedWarbanner";
        public const string PluginVersion = "1.0.1";

        private const string tooltip = "Drops a banner upon Equipment activation. Grants allies attack and movement speed.";

        private const float defaultInitialRadius = 8f;
        private const float defaultRadiusInc = 4f;

        public static ConfigEntry<float> initialRadiusConfig { get; set; }
        public static ConfigEntry<float> radiusIncConfig { get; set; }

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake() {
            // Create config entries.
            initialRadiusConfig = Config.Bind<float>("Warbanner Stats", "Initial Radius", defaultInitialRadius, "The initial radius of a warbanner (stacks = 1). Must be positive, otherwise the default value (8.0 m) will be chosen.");
            radiusIncConfig = Config.Bind<float>("Warbanner Stats", "Radius Increment", defaultRadiusInc, "Controls how quickly the warbanner radius grows with the number of warbanner stacks. Must be positive, otherwise the default value (4.0 m) will be chosen.");
            float initialRadius = (initialRadiusConfig.Value <= 0) ? defaultInitialRadius : initialRadiusConfig.Value;
            float radiusInc = (radiusIncConfig.Value <= 0) ? defaultRadiusInc : radiusIncConfig.Value;

            // Create the description string based on configuration values.
            StringBuilder descriptionBuilder = new StringBuilder();
            descriptionBuilder.Append("Activating your Equipment drops a banner that strengthens all allies within <style=cIsUtility>");
            descriptionBuilder.Append(initialRadius.ToString());
            descriptionBuilder.Append("m </style> <style=cStack>(+");
            descriptionBuilder.Append(radiusInc.ToString());
            descriptionBuilder.Append("m per stack)</style>. Raises <style=cIsDamage>attack speed</style> and <style=cIsUtility>movement speed</style> by <style=cIsDamage>30%</style>.");

            // Changes the tooltip and logbook description for Warbanner.
            LanguageAPI.Add("ITEM_WARDONLEVEL_PICKUP", tooltip);
            LanguageAPI.Add("ITEM_WARDONLEVEL_DESC", descriptionBuilder.ToString());

            // BetterUI integration
            if (BetterUICompatability.enabled) BetterUICompatability.updateTooltip(initialRadius, radiusInc);

            On.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += (orig, self) =>
            {
                // Do nothing.
            };

            // Disables the code that creates a warbanner on teleporter spawn.
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += context =>
            {
                ILCursor cursor = new ILCursor(context);
                if (!cursor.TryGotoNext(context => context.MatchStloc(8))) {
                    // Failure to find location.
                    return;
                }
                cursor.Emit(OpCodes.Pop);
                cursor.Emit(OpCodes.Ldc_I4, 0);
            };

            // On equipment activation, also place a warbanner if the player owns one.
            On.RoR2.EquipmentSlot.OnEquipmentExecuted += (orig, self) =>
            {
                orig(self);
                // Check if the player has a warbanner.
                if (self.characterBody.inventory.GetItemCount(ItemCatalog.itemDefs[182]) > 0)
                {
                    // Spawn a warbanner.
                    int itemCount = self.characterBody.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(RoR2.Items.WardOnLevelManager.wardPrefab,
                                                                                       self.characterBody.transform.position, Quaternion.identity);
                    gameObject.GetComponent<TeamFilter>().teamIndex = self.characterBody.teamComponent.teamIndex;
                    gameObject.GetComponent<BuffWard>().Networkradius = initialRadius + radiusInc * (float)itemCount;
                    UnityEngine.Networking.NetworkServer.Spawn(gameObject);
                }
            };
        }
    }
}
