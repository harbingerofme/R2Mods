using MonoMod.Cil;
using Mono.Cecil.Cil;
using BepInEx.Configuration;
using R2API.Utils;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{

    [HarbTweak(TweakName,DefaultEnabled,Description)]
    internal sealed class FirstStageSpawns : Tweak
    {
        private const string TweakName = "First Stage Spawns";
        private const bool DefaultEnabled = true;
        private const string Description = "This tweak aims to get you going quicker by adding enemies to the first stage.";

        private ConfigEntry<float> scaling;
        
        public FirstStageSpawns(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description)
        { }

        protected override void MakeConfig()
        {
            scaling =  AddConfig(
                "First stage scaling",
                2f,
                "Vanilla gameplay is 0. But since you have this tweak to start quicker anyway, I've doubled it by default."
                );
        }

        protected override void UnHook()
        {
            IL.RoR2.SceneDirector.PopulateScene -= SceneDirector_PopulateScene;
        }

        protected override void Hook()
        {
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
            TweakLogger.LogInfo("FirstStageSpawns", $"Monstercredit for first stage multiplied by: {scaling.Value}");
        }

        private void SceneDirector_PopulateScene(ILContext il)
        {
            ILCursor c = new ILCursor(il);
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
            c.Emit(OpCodes.Ldfld, typeof(RoR2.SceneDirector).GetFieldCached("monsterCredit"));
            c.Emit(OpCodes.Conv_R4);
            c.Emit(OpCodes.Ldc_R4, scaling.Value);
            c.Emit(OpCodes.Mul);
            c.Emit(OpCodes.Conv_I4);
        }
    }
}
