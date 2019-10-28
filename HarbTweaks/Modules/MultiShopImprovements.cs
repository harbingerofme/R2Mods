using BepInEx;
using UnityEngine;
using R2API.Utils;
using RoR2;
using BepInEx.Configuration;


namespace HarbTweaks
{
    [HarbTweak(TweakName, DefaultEnabled, Description)]
    internal sealed class MultiShopImprovements : Tweak
    {
        private const string TweakName = "Multishop Improvements";
        private const bool DefaultEnabled = true;
        private const string Description = "This tweak aims to always leave a choice when interacting with a multishop.";

        private ConfigEntry<int> maxQuestions;
        private ConfigEntry<int> maxSame;

        public MultiShopImprovements(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description)
        { }

        protected override void MakeConfig()
        {
            maxQuestions = AddConfig("Max amount of question marks.", 2, new ConfigDescription("Amount of question marks that can appear at once on a multishop. Vanilla is 3.",new AcceptableValueRange<int>(0,3)));
            maxSame = AddConfig("Max amount of duplicates.", 1, new ConfigDescription("Max amount of duplicates in a shop. Vanilla is 2.", new AcceptableValueRange<int>(0, 2)));
        }

        protected override void Hook()
        {
            On.RoR2.MultiShopController.CreateTerminals += MultiShopController_on_CreateTerminals1;
        }

        protected override void UnHook()
        {
            On.RoR2.MultiShopController.CreateTerminals -= MultiShopController_on_CreateTerminals1;
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
                    TweakLogger.LogWarning("MultiShopImprovements", "Something was wrong with a terminal, aborting.");
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
