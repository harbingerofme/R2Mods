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

        private readonly Vector3 vanillaScale;
        private readonly GameObject lockboxPrefab;

        private bool hookSet;

        public BiggerLockboxes(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description, false)
        {
            lockboxPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/lockbox");
            vanillaScale = lockboxPrefab.transform.localScale;
            hookSet = false;
            ReloadHooks(null, null);
        }

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
            TweakLogger.LogInfo("BiggerLockboxes","Unhooking.");
            lockboxPrefab.transform.localScale = vanillaScale;
            if(hookSet)
            {
                On.RoR2.Stage.ctor -= Stage_ctor;
                hookSet = false;
            }
            //IL.RoR2.SceneDirector.PopulateScene -= SceneDirector_PopulateScene;
        }

        protected override void Hook()
        {
            if (DoNotScaleConfig.Value)
            {
                TweakLogger.LogInfo("BiggerLockboxes","doNotScale");
                ApplyTransform(GetScaleAmount());
            }
            else
            {
                TweakLogger.LogInfo("BiggerLockboxes","Hooked");
                hookSet = true;
                On.RoR2.Stage.ctor += Stage_ctor;

            }
           //IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        private void Stage_ctor(On.RoR2.Stage.orig_ctor orig, Stage self)
        {
            orig(self);
            int keys = 0;
            foreach (CharacterMaster master in CharacterMaster.readOnlyInstancesList)
            {
                keys += master.inventory.GetItemCount(ItemIndex.TreasureCache);
            }
            TweakLogger.LogInfo("BiggerLockboxes","stage_ctor");
            ApplyTransform(GetScaleAmount(keys,FirstConfig.Value));
        }

        private float GetScaleAmount(int keys = 1, bool ignoreFirst = false)
        {
            if (ignoreFirst)
            {
                keys -= 1;
            }

            if (ModeConfig.Value)
            {
                return ((float)keys) * AScaleConfig.Value;
            }
            else
            {
                return 1 + ((float)keys) * MScaleConfig.Value;
            }
        }

        private void ApplyTransform(float scale)
        {
            lockboxPrefab.transform.localScale = vanillaScale;
            if (ModeConfig.Value)
            {
                lockboxPrefab.transform.localScale += new Vector3(scale, scale, scale);
            }
            else
            {
                lockboxPrefab.transform.localScale *= scale;
            }
            TweakLogger.LogInfo("BiggerLockboxes",$"Apply transform: {vanillaScale}->{lockboxPrefab.transform.localScale}");
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


                }
            );
        }
    }
}
