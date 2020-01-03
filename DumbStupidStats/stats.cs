using System;
using RoR2;
using RoR2.Stats;

namespace DumbStupidStats.Stats
{

    [DumbStatDef]
    class Healed : DumbStat{
        private const string token = "dss_healed";
        private const string fullText = "Amount Healed";
        private const StatRecordType recordType = StatRecordType.Sum;
        private const StatDataType dataType = StatDataType.Double;
        public Healed(){
            Definition = StatDef.Register(token, recordType, dataType, 0, null);
            FullText = fullText;
            HealthComponent.onCharacterHealServer += (healthComponent, amount)=>
            {
                var body = healthComponent.body;
                if (body.isPlayerControlled)
                {
                    PlayerStatsComponent.FindBodyStatSheet(body).PushStatValue(Definition, Convert.ToDouble(amount));
                }
            };
        }
    }

    [DumbStatDef]
    class Jumps : DumbStat {
        private const string token = "dss_jumped";
        private const string fullText = "Times Jumped";
        private const StatRecordType recordType = StatRecordType.Sum;
        private const StatDataType dataType = StatDataType.ULong;
        public Jumps(){
            Definition = StatDef.Register(token,recordType ,dataType, 0, null);
            FullText = fullText;
            On.EntityStates.GenericCharacterMain.ApplyJumpVelocity += GenericCharacterMain_ApplyJumpVelocity;
        }
        private void GenericCharacterMain_ApplyJumpVelocity(On.EntityStates.GenericCharacterMain.orig_ApplyJumpVelocity orig, CharacterMotor characterMotor, CharacterBody characterBody, float horizontalBonus, float verticalBonus){
            orig(characterMotor, characterBody, horizontalBonus, verticalBonus);
            if (characterBody.isPlayerControlled)
                PlayerStatsComponent.FindBodyStatSheet(characterBody).PushStatValue(Definition, 1);
        }
    }

        {
        }
    }
}
