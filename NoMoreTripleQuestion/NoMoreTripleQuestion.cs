using BepInEx;
using UnityEngine;
using R2API.Utils;
using RoR2;

/*
    Code By Guido "Harb". 
     */

namespace NoMoreTripleQuestion
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.harbingerofme.noMoreTripleQuestion", "NoMoreTripleQuestion", "1.0.0")]
    public class NoMoreTripleQuestion : BaseUnityPlugin
    {

        public void Awake()
        {
            On.RoR2.MultiShopController.CreateTerminals += MultiShopController_on_CreateTerminals1;
        }

        private void MultiShopController_on_CreateTerminals1(On.RoR2.MultiShopController.orig_CreateTerminals orig, RoR2.MultiShopController self)
        {
            orig(self);
            int count = 0;
            bool allSame = true;
            GameObject[] terminals = self.GetFieldValue<GameObject[]>("terminalGameObjects");
            ShopTerminalBehavior lastSTB = null;
            foreach (GameObject terminal in terminals)
            {
                var stb = terminal.GetComponent<ShopTerminalBehavior>();
                if (stb)
                {
                    lastSTB = stb;
                    if (stb.pickupIndexIsHidden)
                        count++;
                    if (allSame && lastSTB && lastSTB.CurrentPickupIndex() != stb.CurrentPickupIndex())
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
    }
}
