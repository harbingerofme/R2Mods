using System;
using RoR2.Stats;
using RoR2;
namespace DumbStupidStats.Stats
{
    [DumbStatDef]
    class AbsoluteTime : DumbStat
    {
        private const string token = "dss_AbsoluteTime";
        private const string fullText = "Real Time";
        private DateTime time;

        public AbsoluteTime()
        {
            Definition = StatDef.Register(token, StatRecordType.Sum, StatDataType.Double, 0, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));
            FullText = fullText;

        }

        public override void Activate()
        {
            time = DateTime.Now;
        }

        public override void DeActivate()
        {
            var newNow = DateTime.Now;
            var difference = newNow.Subtract(time);
            Util.PushToAllPlayers(Definition, difference.TotalSeconds);
        }
    }
    [DumbStatDef]
    class WastedTime : DumbStat
    {
        private const string token = "dss_WastedTime";
        private const string fullText = "Paused Time";
        public WastedTime()
        {
            Definition = StatDef.Register(token, StatRecordType.Sum, StatDataType.Double, 0, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));
            FullText = fullText;
        }

        public override void Activate()
        {
            On.RoR2.Stats.PlayerStatsComponent.ServerFixedUpdate += PlayerStatsComponent_ServerFixedUpdate;
        }

        public override void DeActivate()
        {
            On.RoR2.Stats.PlayerStatsComponent.ServerFixedUpdate -= PlayerStatsComponent_ServerFixedUpdate;
        }

        private void PlayerStatsComponent_ServerFixedUpdate(On.RoR2.Stats.PlayerStatsComponent.orig_ServerFixedUpdate orig, PlayerStatsComponent self)
        {
            orig(self);
            if (Run.instance && Run.instance.isRunStopwatchPaused && self.characterMaster && self.characterMaster.GetBody())
                PlayerStatsComponent.FindBodyStatSheet(self.characterMaster.GetBody()).PushStatValue(Definition, UnityEngine.Time.fixedDeltaTime);
        }
    }
}
