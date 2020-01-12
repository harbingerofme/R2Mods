using RoR2;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using JetBrains.Annotations;
using R2API;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class BrawnOverBrain : IPickup,IItem
    {
        public TokenValue Name => new TokenValue
        {
            Token = "HC_BOB",
            Value = "Brawn over Brain"
        };

        public TokenValue Description => new TokenValue
        {
            Token = "HC_BOB_DESC",
            Value =
                "50%(+0% per stack) debuff reduction whilst you have shield."
                +" 40%(+15% per stack)* of damage taken is taken from health before shield."
                +" This damage cannot kill while you have enough shield."
        };

        public TokenValue PickupText => new TokenValue
        {
            Token = "HC_BOB_PICKUP",
            Value = "A percentage of damage is taken from health before shield."
                    + " 50% debuff reduction whilst you have shield."
        };

        public string AssetPath => "";
        public string SpritePath => "";

        public ItemTier Tier => ItemTier.Tier3;

        public CustomItem CustomDef { get; set; }
        public ItemDef Definition { get; set; }
        

        public BrawnOverBrain()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
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

            c.EmitDelegate<Func<float, HealthComponent, float>>((damage, self) => {
                if (self.body && self.body.inventory) {
                    int amount = self.body.inventory.GetItemCount(myIndex);
                    if (amount > 0)
                    {
                        float passThroughAmount = damage * (curveA+ (1-curveA)*(1 - (curveB / Mathf.Pow(amount + curveB, curveC))));
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
    }
}
