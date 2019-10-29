using BepInEx.Configuration;
using RoR2;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{
    [HarbTweak(TweakName, DefaultEnabled, Description, HarbTweak.Target.Start)]
    internal sealed class ShorterMedkits : Tweak
    {
        private const string TweakName = "Shorter Medkits";
        private const bool DefaultEnabled = false;
        private const string Description = "Some people complain that medkits take too long to act.";

        private BuffDef MedkitDef;
        private bool VanillaDebuffVal;

        private ConfigEntry<float> NewTime;
        private ConfigEntry<bool> IsDebuff;


        public ShorterMedkits(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description)
        {
        }

        

        protected override void MakeConfig()
        {
            NewTime = AddConfig("Medkit delay in seconds.", 1.1f, new ConfigDescription("The new time for the medkits. In multiplayer, this is fixed to 0.9 seconds to prevent desyncs from different configs.", new AcceptableValueRange<float>(1f / 60f, 2.2f)));
            IsDebuff = AddConfig("Medkit buff is a debuff", true, "By making it a debuff, it makes debuff removing effects apply the medkit heal instantly. Like Blast Shower.");
        }


        protected override void Hook()
        {
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            MedkitDef.isDebuff = IsDebuff.Value;

        }

        protected override void UnHook()
        {
            On.RoR2.CharacterBody.AddTimedBuff -= CharacterBody_AddTimedBuff;
            MedkitDef.isDebuff = VanillaDebuffVal;
        }

        private void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, RoR2.CharacterBody self, RoR2.BuffIndex buffType, float duration)
        {
            if (buffType == BuffIndex.MedkitHeal)
            {
                duration = NewTime.Value;
            }
            orig(self, buffType, duration);
        }
    }
}
