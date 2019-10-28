using BepInEx;
using RoR2;
using UnityEngine;

/*
    Code By Guido "Harb". 
     */

namespace GreedyLockbox
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInIncompatibility("com.harbingerofme.harbtweaks")]
    [BepInPlugin("com.harbingerofme.greedylockboxes", "GreedyLockboxes", "1.0.1")]
    public class GreedyLockboxes : BaseUnityPlugin
    {

        public void Awake()
        {
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
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
