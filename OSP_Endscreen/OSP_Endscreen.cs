using RoR2;
using RoR2.Stats;
using BepInEx;
using UnityEngine;

namespace OSP_Endscreen
{
    [BepInDependency("com.mistername.AssetPlus")]
    [BepInPlugin("com.harbingerofme.ospendscreen", "OSP_Endscreen", "0.3.0")]
    public class OSP_Endscreen : BaseUnityPlugin
    {
        private StatDef ourStatDef;
#if DEBUG
        private StatDef testDef;
#endif
        private readonly string token = "ospcount";

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            this.ourStatDef = StatDef.Register(this.token, StatRecordType.Sum, StatDataType.ULong, 0, null);
#if DEBUG
            this.testDef = StatDef.Register("test", StatRecordType.Sum, StatDataType.ULong, 0, null);
#endif
            On.RoR2.UI.GameEndReportPanelController.Awake += (orig, self) =>
            { //Function that hopefully gets implemented into R2API.
                orig(self);

                string[] strArray = new string[self.statsToDisplay.Length + 1];
                self.statsToDisplay.CopyTo(strArray, 0);
                strArray[self.statsToDisplay.Length] = this.token;
                self.statsToDisplay = strArray;
#if DEBUG       //Add our Debug stat
                strArray = new string[strArray.Length + 1];
                self.statsToDisplay.CopyTo(strArray, 0);
                strArray[self.statsToDisplay.Length] = "test";
                self.statsToDisplay = strArray;
#endif          //
            };
            On.RoR2.HealthComponent.TakeDamage += HealthDamageHook;

        }

        private void HealthDamageHook(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            orig.Invoke(self, damageInfo);
            float dmg = damageInfo.damage;
            if (!self.body || !self.body.isPlayerControlled)
                return; //Ignore if it's not a player.

            if (damageInfo.damageType.HasFlag(DamageType.NonLethal) || damageInfo.rejected || !self.alive)
                return;  //Ignore if it's fall damage, blocked, or we just straight up died.
            if (dmg > self.fullCombinedHealth * 0.9) //the big Damage
            {
#if DEBUG
                PlayerStatsComponent.FindBodyStatSheet(self.body).PushStatValue(this.testDef, 1UL);
#endif
                if (self.combinedHealthFraction <= 0.12) //Are we barely alive?
                    PlayerStatsComponent.FindBodyStatSheet(self.body).PushStatValue(this.ourStatDef, 1UL);
            }
        }

    }
}