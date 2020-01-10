using BepInEx;
using BepInEx.Configuration;
using RoR2.Stats;
using System;
using System.Collections.Generic;
using System.Reflection;
using R2API.AssetPlus;
using System.Text;
using RoR2;

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
        private ConfigEntry<string> blackList;
        private readonly List<string> BlackList;
        private readonly ConfigEntry<int> StatsToAdd;

        private readonly List<DumbStat> statDefs;
        private List<DumbStat> statsToAdd;
        private readonly List<DumberStat> additionalStatDefs;

        private Random rng;

        PluginEntry()
        {
#if DEBUG
            Logger.LogWarning("This is an experimental build!");
#endif
            rng = new Random();
            statDefs = new List<DumbStat>();
            StatsToAdd = Config.Bind("main", "amount", 5, "How many stats to add to the end screen.");
            disablePoints = Config.Bind("miscellaneous", "Disable points in the end screen?", false);

            List<string> stringBuilder = new List<string>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type tInfo in types)
            {
                if (typeof(DumbStat).IsAssignableFrom(tInfo)) {
                    var statAttrs = tInfo.GetCustomAttribute<DumbStatDefAttribute>();
                    if (statAttrs != null)
                    {
                        var stat = (DumbStat)Activator.CreateInstance(tInfo);
                        statDefs.Add(stat);
                        stringBuilder.Add(stat.Definition.displayToken);
                    }
                }
            }
            ConfigDefinition bl = new ConfigDefinition("main", "blacklist");
            ConfigDescription bld = new ConfigDescription("Possible values: " + string.Join(";", stringBuilder));
            blackList = Config.Bind<string>(bl,"",bld);
            var version = Config.Bind<string>("miscellaneous", "config version", VERSION);
            if(Version.Parse(VERSION).CompareTo(Version.Parse(version.Value)) != 0)
            {
                string hold = blackList.Value;
                blackList = null;
                Config.Remove(bl);
                Config.Save();
                version.Value = VERSION;
                blackList = Config.Bind<string>(bl, "", bld);
                blackList.Value = hold; 
            }
            BlackList = new List<string>(blackList.Value.Split(';',',',' '));
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
            RoR2.Run.onRunStartGlobal += PickRunStats;

        }

        private void PickRunStats(Run obj)
        {
            statsToAdd = new List<DumbStat>();
            int failcount = 0;const int MaxFails = 30;
            int max = Math.Min(StatsToAdd.Value, statDefs.Count);
            while (statsToAdd.Count < max && failcount < MaxFails)
            {
                DumbStat stat = statDefs[(int)rng.Next ()* statDefs.Count];
                if (!statsToAdd.Contains(stat) 
                    && !BlackList.Contains(stat.Definition.displayToken))
                {
                    stat.Activate();
                    statsToAdd.Add(stat);
                }
                else
                {
                    failcount++;
                }
            }
            if(failcount==MaxFails)
            {
                Logger.LogWarning("Couln't find enough stats to add! Try reducing your blacklist or the amount of stats to be displayed.");
            }
            statsToAdd.AddRange(additionalStatDefs);
            On.RoR2.UI.GameEndReportPanelController.Awake += GameEndReportPanelController_Awake;
            RoR2.Run.onRunDestroyGlobal += RemoveHooks;
        }

        void RemoveHooks(Run obj)
        {
            On.RoR2.UI.GameEndReportPanelController.Awake -= GameEndReportPanelController_Awake;
            Run.onRunDestroyGlobal -= RemoveHooks;
            foreach(DumbStat stat in statsToAdd)
            {
                stat.DeActivate();
            }
            statsToAdd.Clear();
        }


        /// <summary>
        /// If you want to add your own statdef without worrying about how to get it on the endscreen, use this. You'll be appended to the bottom. Don't use before Awake!
        /// </summary>
        /// <param name="yourDef"></param>
        public void AddStatDef(StatDef yourDef)
        {
            additionalStatDefs.Add(new DumberStat(yourDef));
        }

        private void GameEndReportPanelController_Awake(On.RoR2.UI.GameEndReportPanelController.orig_Awake orig, RoR2.UI.GameEndReportPanelController self)
        {
            orig(self);
            string[] strArray = new string[self.statsToDisplay.Length + statsToAdd.Count];
            self.statsToDisplay.CopyTo(strArray, 0);
            for (int i = 0; i < statsToAdd.Count; i++)
            {
                strArray[self.statsToDisplay.Length + i] = statsToAdd[i].Definition.name;
            }
            self.statsToDisplay = strArray;
        }
    }

    internal abstract class DumbStat
    {
        public StatDef Definition { get; protected set; }
        public string FullText { get; protected set; }

        public abstract void Activate();
        public abstract void DeActivate();
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

        public override void Activate()
        {
        }

        public override void DeActivate()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class DumbStatDefAttribute : Attribute
    {
    }
}
