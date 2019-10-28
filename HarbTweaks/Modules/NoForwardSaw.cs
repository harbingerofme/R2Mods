using BepInEx;
using BepInEx.Configuration;
using System;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{
    [HarbTweak(TweakName, DefaultEnabled, Description)]
    internal sealed class NoForwardSaw : Tweak
    {

        private const string TweakName = "No Forward Saw";
        private const bool DefaultEnabled = false;
        private const string Description = "Makes MUL-T's Powersaw no longer push you forward.";

        private ConfigEntry<float> newForce;

        public NoForwardSaw(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description)
        { }

        protected override void MakeConfig()
        {
            newForce = AddConfig("New force to apply", 0f, "Force applied to MULT when the saw connects.");
        }

        protected override void Hook()
        {
            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter += FireBuzzsaw_OnEnter;
        }
        protected override void UnHook()
        {
            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter -= FireBuzzsaw_OnEnter;
        }

        private void FireBuzzsaw_OnEnter(On.EntityStates.Toolbot.FireBuzzsaw.orig_OnEnter orig, EntityStates.Toolbot.FireBuzzsaw self)
        {
            orig(self);
            EntityStates.Toolbot.FireBuzzsaw.selfForceMagnitude = newForce.Value;
        }
    }
}
