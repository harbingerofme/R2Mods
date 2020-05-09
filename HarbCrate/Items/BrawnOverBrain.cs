using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class BrawnOverBrain : Item
    {
        public BrawnOverBrain() : base()
        {
            const string cShield = "<style=cIsHealing>shield</style>";
            const string cHealth = "<style=cIsHealing>health</style>";

            Name = new TokenValue
            {
                Token = "HC_BOB",
                Value = "Brawn over Brain"
            };
            Description = new TokenValue
            {
                Token = "HC_BOB_DESC",
                Value =
                    $"<style=cIsUtility>50%</style><style=cStack>(+0% per stack)</style> <style=cIsUtility>debuff reduction</style> whilst you have {cShield}."
                    + $" <style=cIsUtility>40%</style><style=cStack>((+15% per stack)</style>* of damage taken is taken from {cHealth} before {cShield}."
                    + $" This damage <b>cannot</b> kill while you have enough {cShield}."
            };
            PickupText = new TokenValue
            {
                Token = "HC_BOB_PICKUP",
                Value = $"A percentage of damage is taken from {cHealth} before {cShield}."
                        + $" <style=cIsUtility>50% debuff reduction</style> whilst you have {cShield}."
            };
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/Tetrahdron/BoB.prefab";
            SpritePath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/Tetrahdron/bob.png";
            Tier = ItemTier.Tier3;
            Tags = new ItemTag[2]
            {
                ItemTag.Utility,
                ItemTag.Healing
            };
            SetupDisplayRules();
        }

        private void SetupDisplayRules()
        {
            var Prefab = Resources.Load<GameObject>(AssetPath);
            var Rule = ItemDisplayRuleType.ParentedPrefab;
            DisplayRules = new R2API.ItemDisplayRuleDict(new ItemDisplayRule()
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(20, 20, 20),
                localPos = new Vector3(-0.43f, -0.72f, -0.43f)
            });
            DisplayRules.Add("mdlToolbot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(175, 175, 175),
                localPos = new Vector3(3.39f, -8f, 3f)
            });
            DisplayRules.Add("mdlCroco", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(175, 175, 175),
                localPos = new Vector3(-3, 4f, 3f)
            });
            DisplayRules.Add("mdlEngiTurret", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Head",
                localScale = new Vector3(25, 25, 25),
                localPos = new Vector3(-1.4f, 1f, -1f)
            });
            DisplayRules["mdlEngiWalkerTurret"] = DisplayRules["mdlEngiTurret"];//The bases are the same.... hopefully?
            DisplayRules.Add("mdlScav", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(200, 200, 200),
                localPos = new Vector3(-5f, 10f, 5f)
            });
            DisplayRules["mdlHAND"] = DisplayRules["mdlToolbot"];
        }

        public override void Hook()
        {
            IL.RoR2.HealthComponent.TakeDamage += NonLethalBypassShield;
            DebuffStatComponent.BrawnIndex = Definition.itemIndex;
            DebuffStatComponent.Hooks();
        }

        private void NonLethalBypassShield(ILContext il)
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

            public static ItemIndex BrawnIndex;

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
                On.RoR2.CharacterBody.AddTimedBuff += ShortenTimedDebuffs;
                On.RoR2.DotController.AddDot += ShortenDots;
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

            private static void ShortenDots(On.RoR2.DotController.orig_AddDot orig, DotController self,
                GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier)
            {
                var cb = self.GetPropertyValue<CharacterBody>("victimBody");
                float newduration = duration;
                var component = cb.GetComponent<DebuffStatComponent>();
                if (component)
                {
                    newduration *= component.Reduction;
                }
                orig(self, attackerObject, newduration, dotIndex, damageMultiplier);
            }

            private static void ShortenTimedDebuffs(On.RoR2.CharacterBody.orig_AddTimedBuff orig,
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
