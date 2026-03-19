using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace TemplateMod;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]

public class Plugin : BaseUnityPlugin
{
    private static class PluginInfo {
        public const string Guid = "yourname.modname";
        public const string Name = "Template";
        public const string Version = "1.0.0";
    }
    
    internal new static ManualLogSource Logger;
    
    private void Awake()
    {
        Logger = base.Logger;
        
        new Harmony(PluginInfo.Guid).PatchAll();
        AssetManager.LoadCatalog();
        Logger.LogInfo($"{PluginInfo.Name} v{PluginInfo.Version} has been loaded.");
    }
}
