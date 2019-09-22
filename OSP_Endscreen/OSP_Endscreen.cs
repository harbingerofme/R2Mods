using RoR2;
using RoR2.Stats;
using BepInEx;
using UnityEngine;

namespace OSP_Endscreen
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a very simple plugin that adds Bandit to the game, and gives you a tier 3 item whenever you press F2.
    //Lets examine what each line of code is for:

    //This attribute specifies that we have a dependency on R2API, as we're using it to add Bandit to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.mistername.AssetPlus")]

    //This attribute is required, and lists metadata for your plugin.
    //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). I like to use the java package notation, which is "com.[your name here].[your plugin name here]"
    //The name is the name of the plugin that's displayed on load, and the version number just specifies what version the plugin is.
    [BepInPlugin("com.harbingerofme.ospendscreen", "OSP_Endscreen", "0.3.0")]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
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
            On.RoR2.UI.GameEndReportPanelController.Awake += (orig,self) => {
                orig(self);
                string[] strArray = new string[self.statsToDisplay.Length + 1];
                self.statsToDisplay.CopyTo(strArray, 0);
                strArray[self.statsToDisplay.Length] = this.token;
                self.statsToDisplay = strArray;
#if DEBUG
                strArray = new string[strArray.Length + 1];
                self.statsToDisplay.CopyTo(strArray, 0);
                strArray[self.statsToDisplay.Length] = "test";
                self.statsToDisplay = strArray;
#endif
            };
            On.RoR2.HealthComponent.TakeDamage += HealthDamageHook;

        //AssetPlus.Languages.Add("OSP_Endscreen.language");

    }

    private void HealthDamageHook(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            orig.Invoke(self, damageInfo);
            float dmg = damageInfo.damage;
            if (!self.body || !self.body.isPlayerControlled)
                return;

            if (damageInfo.damageType.HasFlag(DamageType.NonLethal) || damageInfo.rejected || !self.alive)
                return;
            if (dmg > self.fullCombinedHealth*0.9)
#if DEBUG
                PlayerStatsComponent.FindBodyStatSheet(self.body).PushStatValue(this.testDef, 1UL);
#endif
            if (self.combinedHealthFraction <= 0.12)
                PlayerStatsComponent.FindBodyStatSheet(self.body).PushStatValue(this.ourStatDef, 1UL);
        }

    }
}