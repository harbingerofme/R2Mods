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
        }
        public override void Activate()
        {
            HealthComponent.onCharacterHealServer += hook;
        }

        public override void DeActivate()
        {
            HealthComponent.onCharacterHealServer -= hook;
        }

        private void hook(HealthComponent healthComponent, float amount)
        {
            var body = healthComponent.body;
            if (body.isPlayerControlled)
            {
                PlayerStatsComponent.FindBodyStatSheet(body).PushStatValue(Definition, Convert.ToDouble(amount));
            }
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
        }

        public override void Activate()
        {
            On.EntityStates.GenericCharacterMain.ApplyJumpVelocity += GenericCharacterMain_ApplyJumpVelocity;
        }

        public override void DeActivate()
        {
            On.EntityStates.GenericCharacterMain.ApplyJumpVelocity -= GenericCharacterMain_ApplyJumpVelocity;
        }

        private void GenericCharacterMain_ApplyJumpVelocity(On.EntityStates.GenericCharacterMain.orig_ApplyJumpVelocity orig, CharacterMotor characterMotor, CharacterBody characterBody, float horizontalBonus, float verticalBonus){
            orig(characterMotor, characterBody, horizontalBonus, verticalBonus);
            if (characterBody.isPlayerControlled)
                PlayerStatsComponent.FindBodyStatSheet(characterBody).PushStatValue(Definition, 1);
        }
    }

    [DumbStatDef]
    class DamageBlocked : DumbStat{
        private const string token = "dss_DamageBlocked";
        private const string fullText = "Damage instances blocked";
        public DamageBlocked(){
            Definition = StatDef.Register(token, StatRecordType.Sum, StatDataType.ULong, 0, null);
            FullText = fullText;
        }

        private void hook(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (damageInfo.rejected)
            {
                PlayerStatsComponent.FindBodyStatSheet(self.body).PushStatValue(Definition, 1);
            }
        }

        public override void Activate()
        {
            On.RoR2.HealthComponent.TakeDamage += hook;
        }

        public override void DeActivate()
        {
            On.RoR2.HealthComponent.TakeDamage -= hook;
        }
    }
}
