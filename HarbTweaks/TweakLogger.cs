using RoR2.ConVar;
using BepInEx.Logging;

namespace HarbTweaks
{
    internal class TweakLogger
    {
        private static ManualLogSource logger;
        internal static IntConVar DebugConvar = new IntConVar(HarbTweaks.modname.ToLower() + "_loglevel", RoR2.ConVarFlags.None, "1",LogLevelDescription
            );
        internal const string LogLevelDescription = "Loglevel of HarbTweaks: 0=SHUT UP. 1=startup messages only. 2=developer logging.";

    }
}
