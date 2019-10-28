using RoR2.ConVar;
using BepInEx.Logging;
using System;

namespace HarbTweaks
{
    internal class TweakLogger
    {
        private static ManualLogSource Logger;
        internal static IntConVar DebugConvar = new IntConVar(HarbTweaks.modname.ToLower() + "_loglevel", RoR2.ConVarFlags.None, "2",LogLevelDescription);
        internal const string LogLevelDescription = "Loglevel of HarbTweaks: 0=Nothing but errors are logged. 1=Only warnings and errors are logged. 2=startup messages only. 3=You wish to know everything.";

        public TweakLogger(ManualLogSource logger)
        {
            Logger = logger;
            LogInfo(typeof(TweakLogger).Name,$"Loglevel = {DebugConvar.value}");//We use typeof() here to let refactoring find this name.
        }

        public static void SetLogLevel(int level)
        {
            DebugConvar.SetString(level.ToString());
            LogInfo(typeof(TweakLogger).Name,$"Loglevel = {DebugConvar.value}"); //We use typeof() here to let refactoring find this name.
        }

        public static void Log(string message, int logLevel = 2)
        {
            if(logLevel <= DebugConvar.value)
            {
                //see https://github.com/BepInEx/BepInEx/blob/master/BepInEx/Logging/LogLevel.cs
                //Our loglevels range from 0-3, with 0 being error and 3 being info. Bepinex uses 2,4,8,16 for those instead, so we have to raise 2 to that power instead.
                LogLevel bepInExLogLevel = (LogLevel) (1 << (logLevel + 1)); //This is equivalent to (LogLevel) Math.Pow(2, logLevel + 2);
                Logger.Log(bepInExLogLevel, message);
            }
        }

        public static void LogInfo(string module, string message) 
        {
            Log($"[{module}] {message}",3);
        }
    }
}
