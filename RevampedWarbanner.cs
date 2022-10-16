using BepInEx;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RevampedWarbanner {
    // Declare R2API as a dependency.
    [BepInDependency(R2API.R2API.PluginGUID)]

    // Metadata
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // Declares LanguageAPI dependency
    [R2APISubmoduleDependency(nameof(LanguageAPI))]

    // Declare plugin class.
    public class RevampedWarbanner : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "SolidwoodFailures";
        public const string PluginName = "RevampedWarbanner";
        public const string PluginVersion = "1.0.0";

        private const string tooltip = "Drops a banner upon Equipment activation. Grants allies attack and movement speed.";
        private const string description = "Activating your Equipment drops a banner that strengthens all allies within <style=cIsUtility>8m</style> <style=cStack>(+4m per stack)</style>. Raises <style=cIsDamage>attack speed</style> and <style=cIsUtility>movement speed</style> by <style=cIsDamage>30%</style>.";

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake() {
            // Changes the tooltip and logbook description for Warbanner.
            LanguageAPI.Add("ITEM_WARDONLEVEL_PICKUP", tooltip);
            LanguageAPI.Add("ITEM_WARDONLEVEL_DESC", description);

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
                    gameObject.GetComponent<BuffWard>().Networkradius = 8f + 4f * (float)itemCount;
                    UnityEngine.Networking.NetworkServer.Spawn(gameObject);
                }
            };
        }
    }
}
