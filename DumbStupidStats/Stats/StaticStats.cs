using System;
using RoR2;
using RoR2.Stats;
using static DumbStupidStats.Util;

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
            PushToAllPlayers(Definition,
                        ((ulong)random.Next(int.MinValue, int.MaxValue))
                        +
                        int.MaxValue
                    );
        }

        public override void DeActivate()
        { }
    }

        public override void DeActivate()
        { }
    }
}