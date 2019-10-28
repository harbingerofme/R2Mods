using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace HarbTweaks
{
    

    [BepInDependency("community.mmbait", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, modname, version)]
    public class HarbTweaks : BaseUnityPlugin
    {
        public const string GUID = "com.harbingerofme.harbtweaks";
        public const string modname = "HarbTweaks";
        public const string version = "1.0.0";

        internal ConfigEntry<int> LogLevel;

        public HarbTweaks()
        {
            Instance = this;
            configs = new Dictionary<string, int>();
        }

        public static HarbTweaks Instance { get; private set; }
        private readonly Dictionary<string,int> configs;
        public BepInEx.Logging.ManualLogSource Log { get { return Logger; } }

        public void Awake()
        {
            Config.AddSetting<int>(
                new ConfigDefinition(
                    "",
                    "Log Level"
                    ),
                1,
                new ConfigDescription(
                    TweakLogger.LogLevelDescription,
                    new AcceptableValueRange<int>(0, 2),
                    new ConfigurationManagerAttributes { IsAdvanced = true }
                    )
                );
            new BiggerLockboxes();
            new FirstStageSpawns();
            new NoForwardSaw();
            new NoMoreTripleQuestion();
            new GreedyLockBoxes();
            new ShorterMedkits();
            foreach (string tweakName in configs.Keys)
            {
                Logger.LogMessage("Added: \"" + tweakName + "\"");    
            }
        }


        public ConfigEntry<T> AddConfig<T>(string tweakName, string settingShortDescr, T value, string settingLongDescr, EventHandler callBack = null)
        {
            return AddConfig<T>(tweakName,settingShortDescr,value,new ConfigDescription(settingLongDescr),callBack);

        }

        public ConfigEntry<T> AddConfig<T>(string tweakName, string settingShortDescr, T value, ConfigDescription configDescription, EventHandler callBack = null)
        {
            if (!configs.ContainsKey(tweakName))
            {
                configs.Add(tweakName, 0);
            }
            ConfigEntry<T> entry = Config.AddSetting(tweakName, configs[tweakName] + " - " + settingShortDescr, value,configDescription);
            configs[tweakName]++;
            if (callBack != null)
                entry.SettingChanged += callBack;
            return entry;
        }

    }
}
