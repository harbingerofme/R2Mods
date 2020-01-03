using System;
using RoR2;
using RoR2.Stats;

namespace DumbStupidStats.Stats
{

    [DumbStatDef]
    class Healed : DumbStat
    {
        private const string token = "dss_healed";
        private const string fullText = "Amount Healed";
        private const StatRecordType recordType = StatRecordType.Sum;
        private const StatDataType dataType = StatDataType.Double;
        public Healed()
        {
            Definition = StatDef.Register(token, recordType, dataType, 0, null);
            FullText = fullText;
            HealthComponent.onCharacterHealServer += HealthComponent_onCharacterHealServer;
        }

        private void HealthComponent_onCharacterHealServer(HealthComponent healthComponent, float amount)
        {
            var body = healthComponent.body;
            if (body.isPlayerControlled)
            {
                PlayerStatsComponent.FindBodyStatSheet(body).PushStatValue(Definition, Convert.ToDouble(amount));
            }
        }
    }
}
