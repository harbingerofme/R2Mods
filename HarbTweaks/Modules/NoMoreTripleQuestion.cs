using BepInEx;
using UnityEngine;
using R2API.Utils;
using RoR2;
using BepInEx.Configuration;


namespace HarbTweaks
{
    public class NoMoreTripleQuestion
    {
        private bool Enabled { get { return enabled.Value; } }
        private readonly ConfigEntry<bool> enabled;
        private readonly ConfigEntry<int> maxQuestions;
        private readonly ConfigEntry<int> maxSame;
        private bool prevEnabled = false;

        private readonly string name = "Multishop Improvements";

        public NoMoreTripleQuestion()
        {
            enabled = HarbTweaks.Instance.AddConfig(name, "Actually use the config below?", true, "This tweak aims to always leave a choice when interacting with a multishop.", ReloadHook);
            maxQuestions = HarbTweaks.Instance.AddConfig(name, "Max amount of question marks.", 2, new ConfigDescription("Amount of question marks that can appear at once on a multishop. Vanilla is 3.",new AcceptableValueRange<int>(0,3)), ReloadHook);
            maxSame = HarbTweaks.Instance.AddConfig(name, "Max amount of duplicates.", 1, new ConfigDescription("Max amount of duplicates in a shop. Vanilla is 2.", new AcceptableValueRange<int>(0, 2)), ReloadHook);
            if (Enabled)
                MakeHook();
        }

        public void ReloadHook(object o, System.EventArgs args)
        {
            if (prevEnabled)
                RemoveHook();
            if (Enabled)
                MakeHook();
        }


        public void MakeHook()
        {
            On.RoR2.MultiShopController.CreateTerminals += MultiShopController_on_CreateTerminals1;
            prevEnabled = true;
        }

        public void RemoveHook()
        {
            On.RoR2.MultiShopController.CreateTerminals -= MultiShopController_on_CreateTerminals1;
            prevEnabled = false;
        }

        private void MultiShopController_on_CreateTerminals1(On.RoR2.MultiShopController.orig_CreateTerminals orig, RoR2.MultiShopController self)
        {
            orig(self);
            int questionCount = 0;
            int sameCount = 0;
            PickupIndex[] pickups = new PickupIndex[3];
            GameObject[] terminals = self.GetFieldValue<GameObject[]>("terminalGameObjects");
            ShopTerminalBehavior[] behaviors = new ShopTerminalBehavior[3];
            for (int i=0;i<3;i++)
            {
                GameObject terminal = terminals[i];
                var stb = terminal.GetComponent<ShopTerminalBehavior>();
                behaviors[i] = stb;
                if (stb)
                {
                    if (stb.pickupIndexIsHidden)
                        questionCount++;
                    pickups[i] = stb.CurrentPickupIndex();
                    if (i > 0)
                        if (pickups[i - 1] == pickups[i])
                            sameCount++;
                        else if(i>1 && pickups[0] == pickups[i])
                            sameCount++;
                }
                else
                {
                    HarbTweaks.Instance.Log.LogError("Something was wrong with a terminal, aborting.");
                    return;
                }
            }
            while (questionCount > maxQuestions.Value)
            {
                questionCount--;
                behaviors[questionCount].SetPickupIndex(pickups[questionCount], false);
            }
            while (sameCount>maxSame.Value)
            {
                var term = behaviors[sameCount];
                term.selfGeneratePickup = true;
                term.Start();
                if (sameCount >= 1 && term.CurrentPickupIndex() != behaviors[0].CurrentPickupIndex() )
                {
                    if(sameCount != 2 || term.CurrentPickupIndex() != behaviors[1].CurrentPickupIndex())
                    {
                        sameCount--;
                    }
                }
            }
        }
    }
}
