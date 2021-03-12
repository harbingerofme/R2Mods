using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{

    [BepInPlugin("com.harbingerofme.firststagespawns", "FirstStageSpawns", "2.1.0")]
    [BepInDependency("community.mmbait", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInIncompatibility("com.harbingerofme.harbtweaks")]
    public class FirstStageSpawns : BaseUnityPlugin
    {
        private ConfigEntry<float> scaling;

        public void Awake()
        {
            scaling = Config.AddSetting<float>(
                "",
                "First stage scaling",
                2f,
                new ConfigDescription("Vanilla gameplay is 0. But since you have this tweak to start quicker anyway, I've doubled it by default.")
                );
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
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
            c.Emit(OpCodes.Ldc_R4, val);
            c.Emit(OpCodes.Mul);
            c.Emit(OpCodes.Conv_I4);
        }
    }
}
