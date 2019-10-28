using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;


namespace HarbTweaks
{
    abstract class Tweak
    {
        private ConfigFile Config;

        Tweak(ConfigFile config)
        {
            Config = config;
        }
    }
}
