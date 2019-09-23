using RoR2;
using UnityEngine;
using HarbCrate.Items;
using R2API.Utils;

namespace HarbCrate
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterBody))]
    public class DebuffStatReducerComponent : MonoBehaviour 
    {
        public ItemIndex BrawnOverBrain, GreenShield;

        public float Reduction
        {
            get
            {
                float reductionMulti = 1;
                if (isReady)
                {
                    var body = GetComponent<CharacterBody>();
                    if (body && body.inventory)
                    {
                        int amount = body.inventory.GetItemCount(GreenShield);
                        if (amount > 0)
                        {
                            reductionMulti = DebuffReducer.GetMulti(amount);
                        }
                        if (body.inventory.GetItemCount(BrawnOverBrain) > 0 && body.healthComponent.shield > 0)
                        {
                            reductionMulti *= 0.5f;
                        }

                    }
                }
                else
                {
                    Debug.LogWarning("DebuffStatReducer component not properly init!");
                }
                return reductionMulti;
            }
        }

        private bool isReady
        {
            get { return (BrawnOverBrain != ItemIndex.None && GreenShield != ItemIndex.None); }
        }


        public static void Hooks(ItemIndex Brawn,ItemIndex GreenShield)
        {
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.DotController.AddDot += DotController_AddDot;
            On.RoR2.SetStateOnHurt.SetFrozen += SetStateOnHurt_SetFrozen;
            On.RoR2.SetStateOnHurt.SetStun += SetStateOnHurt_SetStun;
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.inventory)
                {
                    if (self.inventory.GetItemCount(Brawn) + self.inventory.GetItemCount(GreenShield) > 0)
                    {
                        if (self.GetComponent<DebuffReducer>() == null)
                        {
                            var a = self.gameObject.AddComponent<DebuffStatReducerComponent>();
                            a.BrawnOverBrain = Brawn;
                            a.GreenShield = GreenShield;
                        }
                    }
                }
            };
        }

        private static void SetStateOnHurt_SetFrozen(On.RoR2.SetStateOnHurt.orig_SetFrozen orig, SetStateOnHurt self, float duration)
        {
            float newduration = duration;
            var component = self.GetComponentInChildren<DebuffStatReducerComponent>();
            if (component)
                newduration *= component.Reduction;
            orig(self, newduration);
        }

        private static void SetStateOnHurt_SetStun(On.RoR2.SetStateOnHurt.orig_SetStun orig, SetStateOnHurt self, float duration)
        {
            float newduration = duration;
            var component = self.GetComponentInChildren<DebuffStatReducerComponent>();
            if (component)
                newduration *= component.Reduction;
            orig(self, newduration);
        }

        private static void DotController_AddDot(On.RoR2.DotController.orig_AddDot orig, DotController self, GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier)
        {
            var cb = self.GetPropertyValue<CharacterBody>("victimBody");
            float newduration = duration;
            var component = cb.GetComponent<DebuffStatReducerComponent>();
            if (component)
                newduration *= component.Reduction;
            orig(self, attackerObject, newduration, dotIndex, damageMultiplier);
        }

        private static void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            float newduration = duration;
            var component = self.GetComponent<DebuffStatReducerComponent>();
            if (component)
            {
                var buffDef = BuffCatalog.GetBuffDef(buffType);
                if (buffDef.isDebuff)
                {
                    newduration *= component.Reduction;
                }
            }
            orig(self, buffType, newduration);
        }
    }
}
