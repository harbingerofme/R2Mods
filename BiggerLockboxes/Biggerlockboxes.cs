using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using BepInEx.Configuration;

/*
    Code By Guido "Harb". 
     */

namespace HarbCrate
{
    [BepInDependency("community.mmbait",BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.harbingerofme.biggerlockboxes", "BiggerLockboxes", "1.1.2")]
    public class BiggerLockboxes : BaseUnityPlugin
    {

        public static ConfigWrapper<bool> ModeConfig { get; set; }
        public static ConfigWrapper<bool> FirstConfig { get; set; }
        public static ConfigWrapper<bool> DoNotScaleConfig { get; set; }
        public static ConfigWrapper<float> AScaleConfig { get; set; }
        public static ConfigWrapper<float> LScaleConfig { get; set; }
        public void Awake()
        {
            var LoadedMods = BepInEx.Bootstrap.Chainloader.Plugins;
            LoadedMods.Exists((plugin) =>
                        {
                            string GUID = MetadataHelper.GetMetadata(plugin).GUID;
                            if (GUID == "community.mmbait" || GUID == "community.mmhook")
                            {
                                return true;
                            }
                            if (GUID == "com.bepis.r2api")
                            {
                                base.Logger.LogWarning("I've been loaded with r2api. Consider using the seperate hook package instead.");
                                return true;
                            }
                            return false;
                        });

            ModeConfig = Config.Wrap<bool>("", "additiveStacking", "Use Additive Scaling? (recommended)", true);
            FirstConfig = Config.Wrap<bool>("", "ignoreFirst", "Does the box that spawns with only 1 key have a vanilla size?", true);
            DoNotScaleConfig = Config.Wrap<bool>("", "constantScale", "Maybe you just want to increase the size of all lockboxes permanently, but not have it scale with keys. I got you, fam.", false);
            AScaleConfig = Config.Wrap<float>("Additive Stacking", "scale", "How much to increment with for each key. If constantScale is set to true, it applies once to all boxes instead.", 0.1f);
            LScaleConfig = Config.Wrap<float>("Linear Stacking", "scale", "How much to multiply with with for each key. If constantScale is set to true, it applies once to all boxes instead.", 0.5f);
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        private void SceneDirector_PopulateScene(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchCall(out _),//Match any all instruction
                x => x.MatchBrfalse(out _),//match any BrFalse instruction
                x => x.MatchLdloc(20)//Match load local 20.
                );
            c.Index += 3;//Move the cursor to past where I just checked
            c.Emit(OpCodes.Ldloc, 20);//Put local 20 (the box spawned) on the stack
            c.Emit(OpCodes.Ldloc, 17);//Put local 17 (the amount of keys) on the stack
            c.EmitDelegate<Action<GameObject, int>>(//Emit a function that takes the top two args of the stack
                (box/*20*/, keycount/*17*/) => 
                {//normal C# code.
                    float amount = keycount - (FirstConfig.Value ? 1 : 0);
                    amount = DoNotScaleConfig.Value ? 1 : amount;
                    if (ModeConfig.Value)
                    {
                        float a = amount * AScaleConfig.Value;
                        box.transform.localScale += new Vector3(a, a, a);
                    }
                    else
                    {
                        float a = 1 + amount * LScaleConfig.Value;
                        box.transform.localScale *= a;
                    }

                }
            );
        }
    }
}
