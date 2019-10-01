using BepInEx;
using BepInEx.Configuration;
using System;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{
    
    public class NoForwardSaw 
    {

        private bool Enabled { get { return enabled.Value; } }
        private readonly ConfigEntry<bool> enabled;
        private bool prevEnabled = false;

        private readonly ConfigEntry<float> newForce;

        private readonly string name = "No Forward Saw";

        public NoForwardSaw()
        {
            enabled = HarbTweaks.Instance.AddConfig(name, "Enabled", false, "Makes MUL-T's Powersaw no longer push you forward.", ReloadHook);
            newForce = HarbTweaks.Instance.AddConfig(name, "New force to apply", 0f, "Force applied to MULT when the saw connects.", ReloadHook);
            if (Enabled)
                MakeHook();
        }

        public void ReloadHook(object o, EventArgs args)
        {
            if (prevEnabled)
                RemoveHook();
            if (Enabled)
                MakeHook();
        }

        public void MakeHook()
        {
            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter += FireBuzzsaw_OnEnter;
            prevEnabled = true;
        }
        public void RemoveHook()
        {
            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter -= FireBuzzsaw_OnEnter;
            prevEnabled = false;
        }

        private void FireBuzzsaw_OnEnter(On.EntityStates.Toolbot.FireBuzzsaw.orig_OnEnter orig, EntityStates.Toolbot.FireBuzzsaw self)
        {
            orig(self);
            EntityStates.Toolbot.FireBuzzsaw.selfForceMagnitude = newForce.Value;
        }
    }
}
