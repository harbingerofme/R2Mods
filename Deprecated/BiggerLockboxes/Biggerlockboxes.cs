using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

/*
    Code By Guido "Harb". 
     */

namespace HarbTweaks
{
    [BepInDependency("community.mmbait", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInIncompatibility("com.harbingerofme.harbtweaks")]

    [BepInPlugin("com.harbingerofme.biggerlockboxes", "BiggerLockboxes", "2.1.0")]
    public class BiggerLockboxes : BaseUnityPlugin
    {
        private ConfigEntry<bool> ModeConfig;
        private ConfigEntry<bool> FirstConfig;
        private ConfigEntry<bool> DoNotScaleConfig;
        private ConfigEntry<float> AScaleConfig;
        private ConfigEntry<float> MScaleConfig;

        public void Awake()
        {
            ModeConfig = Config.AddSetting("", "Use Additive Scaling", true, "Recommended. Makes the box increase linearily.");
            FirstConfig = Config.AddSetting("", "Ignore first key", true, "The first box spawned can be normal sized.");
            DoNotScaleConfig = Config.AddSetting("", "Constant Increase", false, "If set to true, lockboxes will not scale with number of keys. Rather, it will aply the selected scaling to all boxes all the time. Even the first, regardless of the other setting.");
            AScaleConfig = Config.AddSetting("Additive Scaling", "scale", 0.1f, "How much to increase the size of each dimension with for each key. (If enabled)");
            MScaleConfig = Config.AddSetting("Linear Scaling", "scale", 0.5f, "How much to multiply the size of each dimension with for each key. (If enabled)");
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        public void SceneDirector_PopulateScene(ILContext il)
        {
            var c = new ILCursor(il);
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
                        float a = 1 + amount * MScaleConfig.Value;
                        box.transform.localScale *= a;
                    }

                }
            );
        }
    }
}
