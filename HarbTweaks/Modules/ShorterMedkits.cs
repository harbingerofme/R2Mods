using System;
using BepInEx;
using BepInEx.Configuration;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{
    public class ShorterMedkits
    {
        public readonly ConfigEntry<float> NewTime;
        private bool Enabled { get { return enabled.Value; } }
        private readonly ConfigEntry<bool> enabled;
        private readonly string name = "Shorter Medkits";
        private bool prevEnabled = false;

        public ShorterMedkits()
        {
            enabled = HarbTweaks.Instance.AddConfig(name, "Change the medkit behaviour?", false, "Some people complain that medkits take too long to act.", ReloadHook);
            NewTime = HarbTweaks.Instance.AddConfig(name, "Medkit delay in seconds.", 0.9f , new ConfigDescription("The new time for the medkits. In multiplayer, this is fixed to 0.9 seconds to prevent desyncs from different configs.", new AcceptableValueRange<float>(1f/60f,2.2f)),ReloadHook);
            if(Enabled)
                MakeHook();
        }

        private void ReloadHook(object sender, EventArgs e)
        {
            if (prevEnabled)
                RemoveHook();
            if(Enabled)
                MakeHook();
        }

        private void MakeHook()
        {
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            prevEnabled = true;
        }
        private void RemoveHook()
        {
            On.RoR2.CharacterBody.AddTimedBuff -= CharacterBody_AddTimedBuff;
            prevEnabled = false;
        }

        private void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, RoR2.CharacterBody self, RoR2.BuffIndex buffType, float duration)
        {
            if (buffType == RoR2.BuffIndex.MedkitHeal)
            {
                if (RoR2.Run.instance.participatingPlayerCount > 1)
                    duration = 0.9f;
                else
                    duration = NewTime.Value;
            }
            orig(self, buffType, duration);
        }
    }
}
