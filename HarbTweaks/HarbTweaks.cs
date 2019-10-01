using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace HarbTweaks
{

    [BepInDependency("community.mmbait", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInPlugin("com.harbingerofme.harbtweaks", "HarbTweaks", "1.0.0")]
    public class HarbTweaks : BaseUnityPlugin
    {
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
            new BiggerLockboxes();
            new FirstStageSpawns();
            new NoForwardSaw();
            new NoMoreTripleQuestion();
            new GreedyLockBoxes();
            new ShorterMedkits();
            foreach (string tweakName in configs.Keys)
            {
                Logger.LogMessage("Added Tweak: \"" + tweakName + "\"");    
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
