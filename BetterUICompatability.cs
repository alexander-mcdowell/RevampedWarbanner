using BepInEx;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using R2API;
using R2API.Utils;
using RoR2;

public static class BetterUICompatability
{
    private static bool? _enabled;

    public static bool enabled
    {
        get
        {
            if (_enabled == null)
            {
                _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI");
            }
            return (bool)_enabled;
        }
    }

    public static void updateTooltip(float initialRadius, float radiusInc)
    {
        RoR2Application.onLoad += () =>
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI"))
            {
                BetterUI.ItemStats.RegisterStat(RoR2Content.Items.WardOnLevel, "New Radius", initialRadius, radiusInc, null, statFormatter:BetterUI.ItemStats.StatFormatter.Range, null);
            }
        };
    }
}