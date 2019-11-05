using BepInEx;
using BepInEx.Configuration;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace HarbTweaks
{
    

    [PluginDependency("community.mmbait", PluginDependency.DependencyFlags.SoftDependency)]
    [PluginDependency("community.mmhook", PluginDependency.DependencyFlags.SoftDependency)]
    [PluginDependency("com.bepis.r2api", PluginDependency.DependencyFlags.SoftDependency)]
    [PluginMetadata(GUID, modname, version)]
    public class HarbTweaks : BaseUnityPlugin
    {
        public const string GUID = "com.harbingerofme.harbtweaks";
        public const string modname = "HarbTweaks";
        public const string version = "1.0.0";

        internal ConfigEntry<int> LogLevel;

        private readonly Queue<Type> startTweaks;
        private static readonly Type[] constructorParameters = new Type[] { typeof(ConfigFile), typeof(string), typeof(bool), typeof(string) };

        public HarbTweaks()
        {
            new TweakLogger(Logger);
            LogLevel = Config.Bind(
                new ConfigDefinition(
                    "",
                    "Log Level"
                    ),
                2,
                new ConfigDescription(
                    TweakLogger.LogLevelDescription,
                    new AcceptableValueRange<int>(0, 3),
                    new ConfigurationManagerAttributes { IsAdvanced = true }
                    )
                );
            LogLevel.SettingChanged += LogLevel_SettingChanged;
            LogLevel_SettingChanged(null, null);
            startTweaks = new Queue<Type>();
        }


        public void Awake()
        {
              EnableAllTweaks();
        }

        public void Start()
        {
            LateEnableTweaks();
        }

        /// <summary>
        /// This is a seperate function to encourage people to not have giant Awake()'s.
        /// </summary>
        private void EnableAllTweaks()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                var customAttr = (TweakAttribute)type.GetCustomAttribute(typeof(TweakAttribute), false);
                if (customAttr != null)
                {
                    if(customAttr.target == TweakStartupTarget.Start)
                    {
                        startTweaks.Enqueue(type);
                    }
                    else
                    {
                        EnableTweak(type, customAttr);
                    }
                }
            }
        }

        private void LateEnableTweaks()
        {
            while (startTweaks.Count > 0)
            {
                Type tweak = startTweaks.Dequeue();
                EnableTweak(tweak, (TweakAttribute)tweak.GetCustomAttribute(typeof(TweakAttribute), false));
            }
        }

        private void EnableTweak(Type type, TweakAttribute customAttr)
        {
            try
            {
                var ctor = type.GetConstructor(constructorParameters);
                Tweak tweak = (Tweak)ctor.Invoke(new object[4] { Config, customAttr.Name, customAttr.DefaultEnabled, customAttr.Description }); //Init this array outside the loop if making this a lib or you just have a lot of tweaks.
                tweak.ReloadHooks();
            }
            catch
            {
                TweakLogger.Log($"Couldn't load tweak: {customAttr.Name}",0); 
            }
        }

        private void LogLevel_SettingChanged(object _, EventArgs __)
        {
            TweakLogger.SetLogLevel(LogLevel.Value);
        }
    }
}
