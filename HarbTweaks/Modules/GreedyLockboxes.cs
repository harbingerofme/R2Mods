using RoR2;
using System;
using UnityEngine;
using BepInEx.Configuration;

namespace HarbTweaks
{ 
    public class GreedyLockBoxes
    {
        private bool Enabled { get { return enabled.Value; } }
        private readonly ConfigEntry<bool> enabled;
        private bool prevEnabled = false;

        private readonly string name = "Greedy Lockboxes";

        public GreedyLockBoxes()
        {
            enabled = HarbTweaks.Instance.AddConfig(name, "Enabled", false, "When active, only people with rusted keys can open rusted lockboxes. Not recommended.", ReloadHook);
            if(Enabled)
                MakeHook();
        }

        public void ReloadHook(object o, EventArgs args) 
        {
            if (prevEnabled)
                RemoveHook();
            if (Enabled)
                MakeHook();

        }

        public void MakeHook()
        {
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            prevEnabled = true;
        }

        public void RemoveHook()
        {
            On.RoR2.PurchaseInteraction.GetInteractability -= PurchaseInteraction_GetInteractability;
            On.RoR2.Interactor.PerformInteraction -= Interactor_PerformInteraction;
            prevEnabled = false;
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
