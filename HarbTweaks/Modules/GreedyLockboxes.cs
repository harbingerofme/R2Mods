using RoR2;
using System;
using UnityEngine;
using BepInEx.Configuration;

namespace HarbTweaks
{
    [HarbTweak(TweakName, DefaultEnabled, Description)]
    internal sealed class GreedyLockBoxes : Tweak
    {

        private const string TweakName = "Greedy Lockboxes";
        private const bool DefaultEnabled = true;
        private const string Description = "When active, only people with rusted keys can open rusted lockboxes. Not recommended.";


        public GreedyLockBoxes(ConfigFile config, string name, bool defaultEnabled, string description) : base(config, name, defaultEnabled, description)
        { }


        protected override void MakeConfig() { }
        protected override void Hook()
        {
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
        }

        protected override void UnHook()
        {
            On.RoR2.PurchaseInteraction.GetInteractability -= PurchaseInteraction_GetInteractability;
            On.RoR2.Interactor.PerformInteraction -= Interactor_PerformInteraction;
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {
            CharacterBody buddy = self.GetComponent<CharacterBody>();
            var purchInter = interactableObject.GetComponent<PurchaseInteraction>();
            if (purchInter && purchInter.displayNameToken.ToLower() == "lockbox_name")
            {
                Debug.Log(purchInter.displayNameToken.ToLower());
                if (purchInter.costType == CostTypeIndex.None) {
                    if (buddy && buddy.inventory && buddy.inventory.GetItemCount(ItemIndex.TreasureCache) == 0)
                    {
                        return;
                    }
                }
            }
        orig(self, interactableObject);
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            
            CharacterBody buddy = activator.GetComponent<CharacterBody>();
            if (self.costType == CostTypeIndex.None) {
                if (self.displayNameToken.ToLower() == "lockbox_name")
                {
                    if (buddy && buddy.inventory && buddy.inventory.GetItemCount(ItemIndex.TreasureCache) == 0)
                    {
                        return Interactability.Disabled;
                    }
                }
            }
            return orig(self,activator);
        }
    }
}
