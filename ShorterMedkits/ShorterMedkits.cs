using BepInEx;
using BepInEx.Configuration;

/*
    Code By Guido "Harb". 
     */

namespace ShorterMedkits
{
    [BepInPlugin("com.harbingerofme.shortermedkits", "shortermedkits", "1.0.1")]
    [BepInIncompatibility("com.harbingerofme.harbtweaks")]
    public class ShorterMedkits : BaseUnityPlugin
    {
        public static ConfigEntry<float> NewTime { get; set; }

        public void Awake()
        {
            NewTime = Config.AddSetting("", "time", 0.9f , new ConfigDescription("The new time for the medkits. In multiplayer, this is fixed to 0.9 seconds to prevent desyncs from different configs.", new AcceptableValueRange<float>(1f/60f,2.2f)));
            On.RoR2.CharacterBody.AddTimedBuff += (orig,self,buffType,duration)=>
                {
                    if (buffType == RoR2.BuffIndex.MedkitHeal)
                    {
                        if (RoR2.Run.instance.participatingPlayerCount > 1)
                            duration = 0.9f;
                        else
                            duration = NewTime.Value;
                    }
                    orig(self, buffType, duration);
                };
        }

    }
}
