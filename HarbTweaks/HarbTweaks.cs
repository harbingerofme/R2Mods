using BepInEx;
using BepInEx.Configuration;
using System;
using System.Reflection;
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

        private readonly Queue<Type> startTweaks;
        private static readonly Type[] constructorParameters = new Type[] { typeof(ConfigFile), typeof(string), typeof(bool), typeof(string) };

        public HarbTweaks()
        {
            new TweakLogger(Logger);
            LogLevel = Config.AddSetting(
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
                var customAttr = (HarbTweak)type.GetCustomAttribute(typeof(HarbTweak), false);
                if (customAttr != null)
                {
                    if(customAttr.target == HarbTweak.Target.Start)
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
                EnableTweak(tweak, (HarbTweak)tweak.GetCustomAttribute(typeof(HarbTweak), false));
            }
        }

        private void EnableTweak(Type type, HarbTweak customAttr)
        {
            var ctor = type.GetConstructor(constructorParameters);
            ctor.Invoke(new object[4] { Config, customAttr.Name, customAttr.DefaultEnabled, customAttr.Description }); //Init this array outside the loop if making this a lib or you just have a lot of tweaks.
        }

        private void LogLevel_SettingChanged(object _, EventArgs __)
        {
            TweakLogger.SetLogLevel(LogLevel.Value);
        }
    }
}
