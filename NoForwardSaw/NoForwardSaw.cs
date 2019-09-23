using BepInEx;

/*
    Code By Guido "Harb". 
     */

namespace GreedyLockbox
{
    [BepInPlugin("com.harbingerofme.noforwardsaw", "NoForwardSaw", "1.0.0")]
    public class NoMoreTripleQuestion : BaseUnityPlugin
    {

        public void Awake()
        {
            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter += FireBuzzsaw_OnEnter;
        }

        private void FireBuzzsaw_OnEnter(On.EntityStates.Toolbot.FireBuzzsaw.orig_OnEnter orig, EntityStates.Toolbot.FireBuzzsaw self)
        {
            orig(self);
            EntityStates.Toolbot.FireBuzzsaw.selfForceMagnitude = 0;
        }
    }
}
