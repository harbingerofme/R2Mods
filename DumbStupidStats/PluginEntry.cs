using BepInEx;
using BepInEx.Configuration;
using RoR2.Stats;
using System;
using System.Collections.Generic;
using System.Reflection;
using R2API.AssetPlus;

namespace DumbStupidStats
{
    [R2API.Utils.R2APISubmoduleDependency(nameof(R2API.AssetPlus.AssetPlus))]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class PluginEntry : BaseUnityPlugin
    {
        public const string
            NAME = "DumbStupidStats",
            GUID = "com.harbingerofme." + NAME,
            VERSION = "0.0.0";

        private readonly ConfigEntry<bool> disablePoints;

        private readonly List<DumbStat> statDefs;

        PluginEntry()
        {
#if DEBUG
            Logger.LogWarning("This is an expiremental build!");
#endif
            statDefs = new List<DumbStat>();
            disablePoints = Config.Bind("", "Disable points in the end screen?", false);

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type tInfo in types)
            {
                if (typeof(DumbStat).IsAssignableFrom(tInfo)) {
                    var statAttrs = tInfo.GetCustomAttribute<DumbStatDefAttribute>();
                    if (statAttrs != null)
                    {
                        var stat = (DumbStat)Activator.CreateInstance(tInfo);
                        statDefs.Add(stat);
                    }
                }
            }
        }

        void Awake()
        {
            if (disablePoints.Value)
            {
                Languages.AddToken("STAT_POINTS_FORMAT", "");
                Languages.AddToken("STAT_TOTAL", "");
            }

            Logger.LogInfo($"Adding {statDefs.Count} stats.");
            foreach (DumbStat stat in statDefs)
            {
                Languages.AddToken(stat.Definition.displayToken, stat.FullText);
            }
            On.RoR2.UI.GameEndReportPanelController.Awake += GameEndReportPanelController_Awake;
        }

        /// <summary>
        /// If you want to add your own statdef without worrying about how to get it on the endscreen, use this. You'll be appended to the bottom. Don't use before Awake!
        /// </summary>
        /// <param name="yourDef"></param>
        public void AddStatDef(StatDef yourDef)
        {
            statDefs.Add(new DumberStat(yourDef));
        }

        private void GameEndReportPanelController_Awake(On.RoR2.UI.GameEndReportPanelController.orig_Awake orig, RoR2.UI.GameEndReportPanelController self)
        {
            orig(self);
            string[] strArray = new string[self.statsToDisplay.Length + statDefs.Count];
            self.statsToDisplay.CopyTo(strArray, 0);
            for (int i = 0; i < statDefs.Count; i++)
            {
                strArray[self.statsToDisplay.Length + i] = statDefs[i].Definition.name;
            }
            self.statsToDisplay = strArray;
        }
    }

    internal abstract class DumbStat
    {
        public StatDef Definition { get; protected set; }
        public string FullText { get; protected set; }

        public DumbStat() { }
    }

    /// <summary>
    /// A DumbStat that doesn't even have a fulltext.
    /// </summary>
    internal class DumberStat : DumbStat
    {
        public new string FullText {get{ throw new Exception("DumberStat FullText requested!"); }}
        public DumberStat(StatDef def)
        {
            Definition = def;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class DumbStatDefAttribute : Attribute
    {
    }
}
