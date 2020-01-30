using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using JetBrains.Annotations;
using MonoMod.RuntimeDetour;
using R2API;
using R2API.Utils;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class BrawnOverBrain : Item
    {
        public BrawnOverBrain() : base()
        {
            Name = new TokenValue
            {
                Token = "HC_BOB",
                Value = "Brawn over Brain"
            };
            Description = new TokenValue
            {
                Token = "HC_BOB_DESC",
                Value =
                    "50%(+0% per stack) debuff reduction whilst you have shield."
                    + " 40%(+15% per stack)* of damage taken is taken from health before shield."
                    + " This damage cannot kill while you have enough shield."
            };
            PickupText = new TokenValue
            {
                Token = "HC_BOB_PICKUP",
                Value = "A percentage of damage is taken from health before shield."
                        + " 50% debuff reduction whilst you have shield."
            };
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/Tetrahdron/BoB.prefab";
            SpritePath = "";
            Tier = ItemTier.Tier3;
        }

        public override void Hook()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            DebuffStatComponent.BrawnIndex = Definition.itemIndex;
            DebuffStatComponent.Hooks();
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ItemIndex myIndex = CustomDef.ItemDef.itemIndex;
            ILCursor c = new ILCursor(il);
            int remainingDamage = 5;

            const float curveA = 0.09f, curveB = 2.13f, curveC = 1f;

            c.GotoNext(MoveType.After,
                x => x.MatchStloc(out _),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0),
                x => x.MatchCallvirt("RoR2.HealthComponent", "set_Networkbarrier"),
                x => x.MatchLdloc(out remainingDamage)
            );
            //Right after Ldloc(5) at 552/il_0617
            c.Emit(OpCodes.Ldarg_0);

            c.EmitDelegate<Func<float, HealthComponent, float>>((damage, self) =>
            {
                if (self.body && self.body.inventory)
                {
                    int amount = self.body.inventory.GetItemCount(myIndex);
                    if (amount > 0)
                    {
                        float passThroughAmount =
                            damage * (curveA + (1 - curveA) * (1 - (curveB / Mathf.Pow(amount + curveB, curveC))));
                        float reduceToAmount = Mathf.Min(self.health, 1);
                        float finalHealth = Mathf.Max(self.health - passThroughAmount, reduceToAmount);
                        passThroughAmount = self.health - finalHealth;
                        self.Networkhealth = finalHealth;
                        damage -= passThroughAmount;
                    }
                }

                return damage;
            });

            c.Emit(OpCodes.Stloc, remainingDamage);
            c.Emit(OpCodes.Ldloc, remainingDamage);
        }

        [RequireComponent(typeof(CharacterBody))]
        public class DebuffStatComponent : MonoBehaviour
        {
            public CharacterBody cb;

            private bool BoB;


            public void Start()
            {
                cb.onInventoryChanged += Cb_onInventoryChanged;
            }

            private void Cb_onInventoryChanged()
            {
                BoB = cb.inventory.GetItemCount(BrawnIndex) > 0;
            }

            public float Reduction
            {
                get
                {
                    float reductionMulti = 1;
                    if (cb && cb.inventory)
                    {
                        if (BoB && cb.healthComponent.shield > 0)
                        {
                            reductionMulti *= 0.5f;
                        }
                    }

                    return reductionMulti;
                }
            }

            public static void Hooks()
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
                        a.cb = self;
                    }
                };
            }

            public static ItemIndex BrawnIndex;

            private static void SetStateOnHurt_SetFrozen(On.RoR2.SetStateOnHurt.orig_SetFrozen orig,
                SetStateOnHurt self, float duration)
            {
                float newduration = duration;
                var component = self.GetComponentInChildren<DebuffStatComponent>();
                if (component)
                    newduration *= component.Reduction;
                orig(self, newduration);
            }

            private static void SetStateOnHurt_SetStun(On.RoR2.SetStateOnHurt.orig_SetStun orig, SetStateOnHurt self,
                float duration)
            {
                float newduration = duration;
                var component = self.GetComponentInChildren<DebuffStatComponent>();
                if (component)
                    newduration *= component.Reduction;
                orig(self, newduration);
            }

            private static void DotController_AddDot(On.RoR2.DotController.orig_AddDot orig, DotController self,
                GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier)
            {
                var cb = self.GetPropertyValue<CharacterBody>("victimBody");
                float newduration = duration;
                var component = cb.GetComponent<DebuffStatComponent>();
                if (component)
                {
                    Debug.LogWarning("AAAAAAAAAAAAA");
                    newduration *= component.Reduction;
                }
                orig(self, attackerObject, newduration, dotIndex, damageMultiplier);
            }

            private static void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig,
                CharacterBody self, BuffIndex buffType, float duration)
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
}
