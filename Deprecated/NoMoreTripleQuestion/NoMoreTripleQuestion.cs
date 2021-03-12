using BepInEx;
using R2API.Utils;
using RoR2;
using UnityEngine;

/*
    Code By Guido "Harb". 
     */

namespace NoMoreTripleQuestion
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.harbingerofme.noMoreTripleQuestion", "NoMoreTripleQuestion", "1.1.1")]
    [BepInIncompatibility("com.harbingerofme.harbtweaks")]
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
                    if (stb.pickupIndexIsHidden)
                        count++;
                    if (allSame && lastSTB && lastSTB.CurrentPickupIndex() != stb.CurrentPickupIndex())
                    {
                        allSame = false;
                    }
                    lastSTB = stb;
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
                var pre = lastSTB.CurrentPickupIndex();
                var pre2 = lastSTB.pickupIndexIsHidden;
                lastSTB.SetPickupIndex(lastSTB.CurrentPickupIndex(), false);
                Logger.LogMessage(string.Format("All question marks. Unhiding the last terminal. ({0},{1} -> {2},{3})", pre, pre2, lastSTB.CurrentPickupIndex(), lastSTB.pickupIndexIsHidden));
            }
            if (allSame && lastSTB)
            {
                var pre = lastSTB.CurrentPickupIndex();
                var pre2 = lastSTB.pickupIndexIsHidden;
                lastSTB.selfGeneratePickup = true;
                lastSTB.Start();
                Logger.LogMessage(string.Format("All same. Making the last terminal reroll itself. ({0},{1} -> {2},{3})", pre, pre2, lastSTB.CurrentPickupIndex(), lastSTB.pickupIndexIsHidden));
            }
        }
    }
}
