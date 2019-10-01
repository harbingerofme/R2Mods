using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using BepInEx.Configuration;
using System;
using RoR2;

namespace HarbTweaks
{

    public class BiggerLockboxes
    {
        public bool Enabled { get { return enabled.Value; } }
        private bool prevEnabled = false;
        private readonly ConfigEntry<bool> enabled;
        private readonly ConfigEntry<bool> ModeConfig;
        private readonly ConfigEntry<bool> FirstConfig;
        private readonly ConfigEntry<bool> DoNotScaleConfig;
        private readonly ConfigEntry<float> AScaleConfig;
        private readonly ConfigEntry<float> MScaleConfig;

        private readonly string name = "Bigger Lockboxes";

        public BiggerLockboxes()
        {
            enabled = HarbTweaks.Instance.AddConfig(name, "Enabled", true, "Bigger Lockboxes increases the size of rusted lockboxes if the team has more keys. Also configurable to permanently scale the box regardless of keys.",ReloadHook);
            ModeConfig = HarbTweaks.Instance.AddConfig(name, "Use Additive Scaling", true, "Recommended. Makes the box increase linearily.", ReloadHook);
            FirstConfig = HarbTweaks.Instance.AddConfig(name, "Ignore first key", true, "The first box spawned can be normal sized.", ReloadHook);
            DoNotScaleConfig = HarbTweaks.Instance.AddConfig(name, "Constant Increase", false, "If set to true, lockboxes will not scale with number of keys. Rather, it will aply the selected scaling to all boxes all the time. Even the first, regardless of the other setting.", ReloadHook);
            AScaleConfig = HarbTweaks.Instance.AddConfig(name, "Additive Scaling", 0.1f, "How much to increase the size of each dimension with for each key. (If enabled)", ReloadHook);
            MScaleConfig = HarbTweaks.Instance.AddConfig(name, "Linear Scaling", 0.5f, "How much to multiply the size of each dimension with for each key. (If enabled)",ReloadHook);
            if(Enabled)
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

        public void SceneDirector_PopulateScene(ILContext il)
        {
            var c = new ILCursor(il);
            int lockbox = 0;
            int keyAmount = 0;
            var publicInstance = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            c.GotoNext(
                x => x.MatchCallvirt(typeof(Inventory).GetMethod("GetItemCount", publicInstance)),
                x => x.MatchAdd(),
                x => x.MatchStloc(out keyAmount)
            );
            c.GotoNext(
                x => x.MatchLdstr("SpawnCards/InteractableSpawnCard/iscLockbox")
                );
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallvirt(typeof(DirectorCore).GetMethod("TrySpawnObject", publicInstance)),
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
