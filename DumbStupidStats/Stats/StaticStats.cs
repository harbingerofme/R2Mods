using System;
using RoR2;
using RoR2.Stats;

namespace DumbStupidStats.Stats
{
    [DumbStatDef]
    class RandomNumber : DumbStat
    {
        private const string fulltext = "A Random Number";
        private const string token = "dss_pointlessNR";

        private Random random;
        public RandomNumber()
        {
            random = new Random();
            Definition = StatDef.Register(token, StatRecordType.Newest, StatDataType.ULong, 0);
            FullText = fulltext;
        }

        public override void Activate()
        {
            foreach(NetworkUser user in NetworkUser.readOnlyInstancesList)
            {
                var body = user.GetCurrentBody();
                if (body)
                    PlayerStatsComponent.FindBodyStatSheet(body).PushStatValue(
                    Definition,
                        ((ulong)random.Next(int.MinValue, int.MaxValue))
                        +
                        int.MaxValue
                    );
            }
        }

        public override void DeActivate()
        { }
    }
}