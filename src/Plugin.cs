using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StationCrew.Settings;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace StationCrew
{
    public class PluginInfo
    {
        public const string PluginGuid = "com.shard.station_crew";
        public const string PluginName = "StationCrew";
        public const string PluginVersion = "1.0.0";
        public const string ProcessName = "Star Valor.exe";
    }

    [BepInPlugin(PluginInfo.PluginGuid, PluginInfo.PluginName, PluginInfo.PluginVersion)]
    [BepInProcess(PluginInfo.ProcessName)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log { get; private set; }
        public static ConfigOptions Settings { get; private set; } = new();
        public static Harmony HarmonyInstance { get; private set; } = null;

        public void Awake()
        {
            //Find every class with the [HarmonyPatch] decoration and load it
            GetHarmonyAssemblies().ForEach(LoadHarmonyAssembly);

            Log = Logger;

            Log.LogInfo($"Plugin {PluginInfo.PluginGuid} is loaded!");

            ConfigOptions.LoadConfig(Config);
        }

        public void OnModUnload()
        {
            HarmonyInstance.UnpatchSelf();
            Destroy(gameObject);
        }

        public static List<Type> GetHarmonyAssemblies()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly
                    .GetTypes()
                    .Where(t => t.IsDefined(typeof(HarmonyPatch)))
                )
                .ToList();
        }

        public static void LoadHarmonyAssembly(Type type)
        {
            if (HarmonyInstance == null)
            {
                HarmonyInstance = Harmony.CreateAndPatchAll(type);
            }
            else
            {
                HarmonyInstance.PatchAll(type);
            }
            
        }

    }
}
