using BepInEx;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;
using BepInEx.Configuration;
using System;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{

    public class FirstStageSpawns
    {
        private readonly ConfigEntry<float> scaling;
        private bool Enabled { get { return enabled.Value; } }
        private readonly ConfigEntry<bool> enabled;
        private bool prevEnabled = false;

        private readonly string name = "First Stage Spawns";

        

        public FirstStageSpawns()
        {
            enabled = HarbTweaks.Instance.AddConfig(name,"Enabled",true,"This tweak aims to get you going quicker by adding enemies to the first stage.",ReloadHook);
            scaling =  HarbTweaks.Instance.AddConfig(
                name,
                "First stage scaling",
                2f,
                "Vanilla gameplay is 0. But since you have this tweak to start quicker anyway, I've doubled it by default.",
                ReloadHook
                ) ;
            if (Enabled)
            {
                MakeHook();
            }
        }

        public void ReloadHook(object e, EventArgs args)
        {
            if(prevEnabled)
                RemoveHook();
            if (Enabled)
                MakeHook();
        }
        
        public void RemoveHook()
        {
            IL.RoR2.SceneDirector.PopulateScene -= SceneDirector_PopulateScene;
            prevEnabled = false;
        }

        public void MakeHook()
        {
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
            prevEnabled = true;
        }

        private void SceneDirector_PopulateScene(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            float val = scaling.Value;
            c.GotoNext(
                MoveType.After,
                x => x.MatchCall(out _),
                x => x.MatchLdfld(out _),
                x => x.MatchBrtrue(out _),
                x => x.MatchLdarg(0),
                x => x.MatchLdcI4(0)
                );
            c.Index--;
            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, typeof(RoR2.SceneDirector).GetField("monsterCredit", BindingFlags.NonPublic | BindingFlags.Instance));
            c.Emit(OpCodes.Conv_R4);
            c.Emit(OpCodes.Ldc_R4,val);
            c.Emit(OpCodes.Mul);
            c.Emit(OpCodes.Conv_I4);
        }
    }
}
