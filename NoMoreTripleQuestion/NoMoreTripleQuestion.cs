using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using R2API.Utils;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using BepInEx.Configuration;
using RoR2;

/*
    Code By Guido "Harb". 
     */

namespace NoMoreTripleQuestion
{
    //[BepInDependency("community.mmbait",BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("community.mmhook", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.harbingerofme.noMoreTripleQuestion", "NoMoreTripleQuestion", "1.0.0")]
    public class NoMoreTripleQuestion : BaseUnityPlugin
    {
        /*
        public static ConfigWrapper<bool> ModeConfig { get; set; }
        public static ConfigWrapper<bool> FirstConfig { get; set; }
        public static ConfigWrapper<bool> DoNotScaleConfig { get; set; }
        public static ConfigWrapper<float> AScaleConfig { get; set; }
        public static ConfigWrapper<float> LScaleConfig { get; set; }
        */
        public void Awake()
        {
           /* var LoadedMods = BepInEx.Bootstrap.Chainloader.Plugins;
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
                        });*/
            On.RoR2.MultiShopController.CreateTerminals += MultiShopController_on_CreateTerminals1;
        }

        private void MultiShopController_on_CreateTerminals1(On.RoR2.MultiShopController.orig_CreateTerminals orig, RoR2.MultiShopController self)
        {
            orig(self);
            int count = 0;
            bool allSame = true;
            GameObject[] terminals = self.GetFieldValue<GameObject[]>("terminalGameObjects");
            ShopTerminalBehavior lastSTB = null;
            foreach(GameObject terminal in terminals)
            {
                var stb = terminal.GetComponent<ShopTerminalBehavior>();
                if (stb)
                {
                    lastSTB = stb;
                    if (stb.pickupIndexIsHidden)
                        count++;
                    if(allSame && lastSTB && lastSTB.CurrentPickupIndex() != stb.CurrentPickupIndex())
                    {
                        allSame = false;
                    }
                }
                else
                {
                    count = -1;
                    allSame = false;
                    base.Logger.LogError("Something was wrong with a terminal, aborting.");
                    break;
                }
            }
            if (count == terminals.Length && lastSTB)
            {
                lastSTB.SetPickupIndex(lastSTB.CurrentPickupIndex(), false);
            }
            if (allSame && lastSTB)
            {
                lastSTB.selfGeneratePickup = true;
                lastSTB.Start();
            }
        }

#if aaaa
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
#endif
    }
}
