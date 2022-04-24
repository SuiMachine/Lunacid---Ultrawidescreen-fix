using BepInEx;
using HarmonyLib;

namespace LunacidUltrawidescreenFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Harmony Harmony { get; private set; }
        public static BepInEx.Logging.ManualLogSource LogInstance { get; private set; }

        private void Awake()
        {
            LogInstance = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} initializing!");
            Harmony = new Harmony("local.ultrawidescreenfix.suimachine");
            Harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
