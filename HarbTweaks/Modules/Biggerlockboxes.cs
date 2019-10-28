using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using BepInEx.Configuration;
using System;
using RoR2;
using R2API.Utils;

namespace HarbTweaks
{
    [HarbTweak(TweakName, DefaultEnabled, Description)]
    internal sealed class BiggerLockboxes : Tweak
    {
        private const string TweakName = "Bigger Lockboxes";
        private const bool DefaultEnabled = true;
        private const string Description = "Bigger Lockboxes increases the size of rusted lockboxes if the team has more keys. Also configurable to permanently scale the box regardless of keys.";

        private ConfigEntry<bool> ModeConfig;
        private ConfigEntry<bool> FirstConfig;
        private ConfigEntry<bool> DoNotScaleConfig;
        private ConfigEntry<float> AScaleConfig;
        private ConfigEntry<float> MScaleConfig;

        public BiggerLockboxes(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description)
        { }

        protected override void MakeConfig()
        {
            ModeConfig = AddConfig("Use Additive Scaling", true, "Recommended. Makes the box increase linearily.");
            FirstConfig = AddConfig("Ignore first key", true, "The first box spawned can be normal sized.");
            DoNotScaleConfig = AddConfig("Constant Increase", false, "If set to true, lockboxes will not scale with number of keys. Rather, it will aply the selected scaling to all boxes all the time. Even the first, regardless of the other setting.");
            AScaleConfig = AddConfig("Additive Scaling", 0.1f, "How much to increase the size of each dimension with for each key. (If enabled)");
            MScaleConfig = AddConfig("Linear Scaling", 0.5f, "How much to multiply the size of each dimension with for each key. (If enabled)");
        }

        protected override void UnHook()
        {
            IL.RoR2.SceneDirector.PopulateScene -= SceneDirector_PopulateScene;
        }

        protected override void Hook()
        {
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        public void SceneDirector_PopulateScene(ILContext il)
        {
            var c = new ILCursor(il);
            int lockbox = 0;
            int keyAmount = 0;
            c.GotoNext(
                x => x.MatchCallvirt(typeof(Inventory).GetMethodCached("GetItemCount")),
                x => x.MatchAdd(),
                x => x.MatchStloc(out keyAmount)
            );
            c.GotoNext(
                x => x.MatchLdstr("SpawnCards/InteractableSpawnCard/iscLockbox")
                );
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallvirt(typeof(DirectorCore).GetMethodCached("TrySpawnObject")),
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
                    TweakLogger.LogInfo("BiggerLockboxes", "Scaling Lockbox.");
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
