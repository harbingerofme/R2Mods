using BepInEx;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;

/*
    Code By Guido "Harb". 
     */

namespace HarbMods
{
    [BepInPlugin("com.harbingerofme.firststagespawns", "FirstStageSpawns", "2.0.0")]
    [BepInDependency("com.harbingerofme.biggerlockboxes", BepInDependency.DependencyFlags.SoftDependency)]
    public class FirstStageSpawns : BaseUnityPlugin
    {

        public void Awake()
        {
            var LoadedMods = BepInEx.Bootstrap.Chainloader.Plugins;
            if(LoadedMods.Exists((plugin) => { return MetadataHelper.GetMetadata(plugin).GUID == "com.harbingerofme.biggerlockboxes"; }))
            {
                Logger.LogInfo("Detected BiggerLockboxes. Delegating my hooks!");
            }
            else
            {
                IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
            }
            
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
            c.Emit(OpCodes.Ldfld, typeof(RoR2.SceneDirector).GetField("monsterCredit", BindingFlags.NonPublic | BindingFlags.Instance));
            c.Emit(OpCodes.Ldc_I4_2);
            c.Emit(OpCodes.Mul);
        }
    }
}
