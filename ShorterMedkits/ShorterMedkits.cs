using BepInEx;
using BepInEx.Configuration;

/*
    Code By Guido "Harb". 
     */

namespace ShorterMedkits
{
    [BepInPlugin("com.harbingerofme.shortermedkits", "shortermedkits", "1.0.0")]
    public class ShorterMedkits : BaseUnityPlugin
    {
        public static ConfigWrapper<float> NewTime { get; set; }

        public void Awake()
        {
            NewTime = Config.Wrap<float>("", "time", "The new time for the medkits. In multiplayer, this is fixed to 0.55 seconds to prevent desyncs from different configs.", 0.55f);
            On.RoR2.CharacterBody.AddTimedBuff += (orig,self,buffType,duration)=>
                {
                    if (buffType == RoR2.BuffIndex.MedkitHeal)
                    {
                        if (RoR2.Run.instance.participatingPlayerCount > 1)
                            duration *= 0.5f;
                        else
                            duration = NewTime.Value;
                    }
                    orig(self, buffType, duration);
                };
        }

    }
}
