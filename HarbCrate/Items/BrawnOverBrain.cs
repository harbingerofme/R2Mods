using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace HarbCrate.Items
{
    [Item]
    class BrawnOverBrain : ItemDef
    {
        public static BrawnOverBrain instance;
        public readonly static string Name = "Brawn over Brain";
        public readonly static string PickupText = "A percentage of damage is taken from health before shield. 50% debuff reduction whilst you have shield.";
        public readonly static string Description = "50%  (+0% per stack) debuff reduction whilst you have shield. 40%(+15% per stack)* of damage taken is taken from health before shield. This damage cannot kill while you have enough shield.";
        public BrawnOverBrain()
        {
            tier = ItemTier.Tier3;
            pickupModelPath = "Prefabs/PickupModels/PickupMystery";
            pickupIconPath = "Textures/AchievementIcons/texLoaderClearGameMonsoonIcon";
            nameToken = "BOB_NAME_TOKEN";
            pickupToken = "BOB_PICKUP_TOKEN";
            descriptionToken = "BOB_DESCRIPTION_TOKEN";

            R2API.AssetPlus.Languages.AddToken(nameToken, Name);
            R2API.AssetPlus.Languages.AddToken(pickupToken, PickupText);
            R2API.AssetPlus.Languages.AddToken(descriptionToken, Description);
            instance = this;

            R2API.ItemAPI.AddCustomItem(new R2API.CustomItem(this, null));
        }

        public static void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private static void HealthComponent_TakeDamage(ILContext il)
        {
            ItemIndex myIndex = (ItemIndex)ItemLib.ItemLib.GetItemId(Name);
            ILCursor c = new ILCursor(il);
            int remainingDamage = 5;

            float curve_a = 0.09f, curve_b = 2.13f, curve_c = 1f;

            c.GotoNext(
                MoveType.After,
                x => x.MatchStloc(out _),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0),
                x => x.MatchCallvirt("RoR2.HealthComponent", "set_Networkbarrier"),
                x => x.MatchLdloc(out remainingDamage)
                );
            //Right after Ldloc(5) at 552/il_0617
            c.Emit(OpCodes.Ldarg_0);

            c.EmitDelegate<Func<float, HealthComponent, float>>((damage, self) => {
                if (self.body && self.body.inventory) {
                    int amount = self.body.inventory.GetItemCount(myIndex);
                    if (amount > 0)
                    {
                        float passThroughAmount = damage * (curve_a+ (1-curve_a)*(1 - (curve_b / Mathf.Pow(amount + curve_b, curve_c))));
                        float ReduceToAmount = Mathf.Min(self.health, 1);
                        float finalHealth = Mathf.Max(self.health - passThroughAmount, ReduceToAmount);
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
    }
}
