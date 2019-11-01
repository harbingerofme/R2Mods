using System;
using BepInEx.Configuration;
using System.Reflection;
using BepInEx.Logging;


namespace HarbTweaks
{
    internal abstract class Tweak
    {
        private readonly ConfigFile Config;
        public readonly string Name;

        protected bool PreviouslyEnabled = false;
        protected readonly ConfigEntry<bool> Enabled;
        

        private int ConfigOrder = 0;

        /// <summary>Use Reflection to set the name from the HarbTweak attribute to prevent having to declare it twice</summary>
        /// <param name="config">A reference to the Config of the calling plugin</param>
        /// <param name="name">The name of this tweak, should be identical to the HarbTweak attribute name.</param>
        /// <param name="defaultEnabled">If this tweak is enabled by default.</param>
        /// <param name="description">If this tweak is enabled by default.</param>
        public Tweak(ConfigFile config, string name, bool defaultEnabled, string description, bool CanStartAuto = true)
        {
            Config = config;
            Name = name;
            Enabled = AddConfig("Enabled", defaultEnabled, description);
            Enabled.SettingChanged += Enabled_SettingChanged;
            MakeConfig();
            if(CanStartAuto)
                ReloadHooks(null, null);
            TweakLogger.Log($"Loaded Tweak: {Name}.");
        }

        private void Enabled_SettingChanged(object sender, EventArgs e)
        {
            if (Enabled.Value)
            {
                TweakLogger.LogInfo("TweakBase", $"Enabled Tweak: {Name}.");
            }
            else
            {
                TweakLogger.LogInfo("TweakBase", $"Disabled Tweak: {Name}.");
            }
        }

        public void ReloadHooks(object _, EventArgs __)
        {
            if (PreviouslyEnabled)
            {
                UnHook();
                PreviouslyEnabled = false;
            }
            if (Enabled.Value)
            {
                Hook();
                PreviouslyEnabled = true;
            }
        }

        protected abstract void UnHook();
        protected abstract void Hook();

        protected abstract void MakeConfig();


        protected ConfigEntry<T> AddConfig<T>(string settingShortDescr, T value, string settingLongDescr)
        {
            return AddConfig(settingShortDescr, value, new ConfigDescription(settingLongDescr));
        }

        public ConfigEntry<T> AddConfig<T>(string settingShortDescr, T value, ConfigDescription configDescription)
        {
            ConfigDescription orderedConfigDescription = new ConfigDescription(configDescription.Description, configDescription.AcceptableValues, new ConfigurationManagerAttributes { Order = --ConfigOrder });
            ConfigEntry<T> entry = Config.AddSetting(Name,settingShortDescr, value, orderedConfigDescription);
            entry.SettingChanged += ReloadHooks;
            return entry;
        }

    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class HarbTweak : Attribute
    {
        public readonly string Name;
        public readonly bool DefaultEnabled;
        public readonly string Description;
        public Target target;
        public HarbTweak(string name, bool defaultEnabled, string description, Target target = Target.Awake)
        {
            Name = name;
            DefaultEnabled = defaultEnabled;
            Description = description;
            this.target = target;
        }

        public enum Target
        {
            Awake,
            Start
        }
    }
}
