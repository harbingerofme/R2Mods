using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using BepInEx.Configuration;
using System;

/*
    Code By Guido "Harb". 
     */

namespace HarbMods
{
    [BepInDependency("community.mmbait", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    
    [BepInPlugin("com.harbingerofme.biggerlockboxes", "BiggerLockboxes", "2.0.0")]
    public class BiggerLockboxes : BaseUnityPlugin
    {

        public static ConfigWrapper<bool> ModeConfig { get; set; }
        public static ConfigWrapper<bool> FirstConfig { get; set; }
        public static ConfigWrapper<bool> DoNotScaleConfig { get; set; }
        public static ConfigWrapper<float> AScaleConfig { get; set; }
        public static ConfigWrapper<float> LScaleConfig { get; set; }

        private bool FSSCompat = false;
        public void Start()
        { 
            ModeConfig = Config.Wrap("", "additiveStacking", "Use Additive Scaling? (recommended)", true);
            FirstConfig = Config.Wrap("", "ignoreFirst", "Does the box that spawns with only 1 key have a vanilla size?", true);
            DoNotScaleConfig = Config.Wrap("", "constantScale", "Maybe you just want to increase the size of all lockboxes permanently, but not have it scale with keys. I got you, fam.", false);
            AScaleConfig = Config.Wrap("Additive Stacking", "scale", "How much to increment with for each key. If constantScale is set to true, it applies once to all boxes instead.", 0.1f);
            LScaleConfig = Config.Wrap("Linear Stacking", "scale", "How much to multiply with with for each key. If constantScale is set to true, it applies once to all boxes instead.", 0.5f);
            var LoadedMods = BepInEx.Bootstrap.Chainloader.Plugins;
            FSSCompat = LoadedMods.Exists((plugin) =>
            {
                string GUID = MetadataHelper.GetMetadata(plugin).GUID;
                return GUID == "com.harbingerofme.firststagespawns";
            });
            if (FSSCompat)
            {
                Logger.LogInfo("First Stage Spawns Detected, extending my hook");
            }
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        public void SceneDirector_PopulateScene(ILContext il)
        {
            var c = new ILCursor(il);
            if (FSSCompat)
            {
                FSSHooks(c);   
            }
            int lockbox = 0;
            int keyAmount = 0;
            var publicInstance = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            c.GotoNext(
                x => x.MatchCallvirt(typeof(RoR2.Inventory).GetMethod("GetItemCount", publicInstance)),
                x => x.MatchAdd(),
                x => x.MatchStloc(out keyAmount)
            );
            c.GotoNext(
                x => x.MatchLdstr("SpawnCards/InteractableSpawnCard/iscLockbox")
                );
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallvirt(typeof(RoR2.DirectorCore).GetMethod("TrySpawnObject", publicInstance)),
                x => x.MatchStloc(out lockbox),
                x => x.MatchLdloc(lockbox),
                x => x.MatchCall(out _),//Match any call instruction
                x => x.MatchBrfalse(out _),//match any BrFalse instruction
                x => x.MatchLdloc(lockbox)
                );
            c.Emit(OpCodes.Ldloc, lockbox);
            c.Emit(OpCodes.Ldloc, keyAmount);
            c.EmitDelegate<Action<GameObject, int>>(//
                (box, keycount) =>
                {
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

        private void FSSHooks(ILCursor c)
        {
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
            c.Emit(OpCodes.Ldfld, typeof(RoR2.SceneDirector).GetField("monsterCredit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            c.Emit(OpCodes.Ldc_I4_2);
            c.Emit(OpCodes.Mul);
        }
    }
}
