using RoR2;
using UnityEngine;
using HarbCrate.Items;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using System;


namespace HarbCrate
{
    [RequireComponent(typeof(CharacterBody))]
    public class DebuffStatComponent : MonoBehaviour 
    {
        public ItemIndex BrawnOverBrain, GreenShield;
        public CharacterBody cb;

        private int GS;
        private bool BoB;


        public void Start()
        {
            cb.onInventoryChanged += Cb_onInventoryChanged;
        }

        private void Cb_onInventoryChanged()
        {
            BoB = cb.inventory.GetItemCount(BrawnOverBrain)>0;
            GS = cb.inventory.GetItemCount(GreenShield);
        }

        public float Reduction
        {
            get
            {
                float reductionMulti = 1;
                if (IsReady)
                {
                    if (cb && cb.inventory)
                    {
                        if (GS > 0)
                        {
                            reductionMulti = Util.ConvertAmplificationPercentageIntoReductionPercentage(GS * Items.TimePiece.Scaling);
                            Debug.Log(reductionMulti);
                        }
                        if (BoB && cb.healthComponent.shield > 0)
                        {
                            reductionMulti *= 0.5f;
                            Debug.Log(reductionMulti);
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

        private bool IsReady
        {
            get { return (BrawnOverBrain != ItemIndex.None && GreenShield != ItemIndex.None); }
        }


        public static void Hooks(ItemIndex GreenShield, ItemIndex Brawn)
        {
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.DotController.AddDot += DotController_AddDot;
            On.RoR2.SetStateOnHurt.SetFrozen += SetStateOnHurt_SetFrozen;
            On.RoR2.SetStateOnHurt.SetStun += SetStateOnHurt_SetStun;
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                var a = self.GetComponent<DebuffStatComponent>();
                if (!a)
                {
                    a = self.gameObject.AddComponent<DebuffStatComponent>();
                    a.BrawnOverBrain = Brawn;
                    a.GreenShield = GreenShield;
                    a.cb = self;
                }
            };
        }

        private static void SetStateOnHurt_SetFrozen(On.RoR2.SetStateOnHurt.orig_SetFrozen orig, SetStateOnHurt self, float duration)
        {
            float newduration = duration;
            var component = self.GetComponentInChildren<DebuffStatComponent>();
            if (component)
                newduration *= component.Reduction;
            orig(self, newduration);
        }

        private static void SetStateOnHurt_SetStun(On.RoR2.SetStateOnHurt.orig_SetStun orig, SetStateOnHurt self, float duration)
        {
            float newduration = duration;
            var component = self.GetComponentInChildren<DebuffStatComponent>();
            if (component)
                newduration *= component.Reduction;
            orig(self, newduration);
        }

        private static void DotController_AddDot(On.RoR2.DotController.orig_AddDot orig, DotController self, GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier)
        {
            var cb = self.GetPropertyValue<CharacterBody>("victimBody");
            float newduration = duration;
            var component = cb.GetComponent<DebuffStatComponent>();
            if (component)
                newduration *= component.Reduction;
            orig(self, attackerObject, newduration, dotIndex, damageMultiplier);
        }

        private static void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            float newduration = duration;
            var component = self.GetComponent<DebuffStatComponent>();
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
