using RoR2;
using UnityEngine;
using MonoMod.Cil;
using R2API.Utils;
using System;
using Mono.Cecil.Cil;
using R2API;
using UserProfile = On.RoR2.UserProfile;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class ShieldOnMultiKill : Item
    {
        
        private const float ShieldPerMK = 10f;//Scaling in percent.
        private const int MultikillCountNeeded = 4;
        private const float MaxSize =100;//shield fraction in actual numbers.
        private const float PerStack = 100;
        
        public ShieldOnMultiKill() : base()
        {
            Tier = ItemTier.Tier2;
            Name = new TokenValue("HC_MAXSHIELDONMULTIKILL","Perfect Timepiece");
            Description = new TokenValue(
                "HC_MAXSHIELDONMULTIKILL_DESC",
                $" Gain {ShieldPerMK} additional maximum shield on multikill."
                + $" Maximum shield tops of at an aditional {MaxSize}<style=cStack>(+{PerStack} per stack)</style>.");
            PickupText = new TokenValue("HC_MAXSHIELDONMULTIKILL_PICKUP", "Gain maximum shield on multikill.");
            AssetPath = "";
            SpritePath = "";
        }

        public override void Hook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ModifyRecalcStats;
            Inventory.onServerItemGiven += UpdateShieldInfusion;
            On.RoR2.CharacterBody.AddMultiKill += CharacterBodyOnAddMultiKill;
        }

        private void CharacterBodyOnAddMultiKill(On.RoR2.CharacterBody.orig_AddMultiKill orig, CharacterBody self, int kills)
        {
            orig(self, kills);
            if (self.inventory && self.multiKillCount % MultikillCountNeeded == 0 && self.inventory.GetItemCount(Definition.itemIndex) > 0 )
            {
                self.inventory.GetComponent<ShieldInfusion>().ShieldCharge += ShieldPerMK;
            }
        }

        private void ModifyRecalcStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int shieldsLoc = 33;
            c.GotoNext(
                MoveType.Before,
                x => x.MatchLdloc(out shieldsLoc),
                x => x.MatchCallvirt(typeof(CharacterBody).GetMethodCached("set_maxShield"))
            );
            c.Emit(OpCodes.Ldloc, shieldsLoc);
            c.EmitDelegate<Func<CharacterBody, float, float>>((self, shields) =>
            {
                ShieldInfusion si = self.inventory.GetComponent<ShieldInfusion>();
                if (si)
                {
                    shields += si.ShieldCharge;
                }
                return shields;
            });
            c.Emit(OpCodes.Stloc, shieldsLoc);
            c.Emit(OpCodes.Ldarg_0);
        }
        private void UpdateShieldInfusion(Inventory inventory,ItemIndex index,int count)
        {
            if (index != Definition.itemIndex)
                return;

            ShieldInfusion si = inventory.GetComponent<ShieldInfusion>();
            if (si == null)
            {
                si = inventory.gameObject.AddComponent<ShieldInfusion>();
            }

            si.Count += count;
        }

        public class ShieldInfusion : MonoBehaviour
        {
            private float shieldCharge;
            [NonSerialized] public int Count;
            public CharacterBody body;

            public float ShieldCharge
            {
                get => shieldCharge;
                set => shieldCharge = Math.Min(value, (MaxSize * Math.Max(1,Count) + (Count-1)*PerStack));
            }

            private void Start()
            {
                body.onInventoryChanged += () =>
                {
                    Count = body.inventory.GetItemCount(((ShieldOnMultiKill) Instance).Definition.itemIndex);
                    ShieldCharge = shieldCharge;
                };
                
            }
        }
    }

}
