using RoR2;
using UnityEngine;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class Rubbish : Item
    {
        public Rubbish() : base()
        {
            Name = new TokenValue
            {
                Token = "HC_RUBBISH",
                Value = "Rubbish"
            };
            Description = new TokenValue
            {
                Token = "HC_RUBBISH_DESC",
                Value =
                    "Every 3 of these you collect get turned into an item of a higher tier."
            };
            PickupText = new TokenValue
            {
                Token = "HC_RUBBISH_PICKUP",
                Value = "Useless. But one man's trash..."
            };
            AssetPath = "Prefabs/PickupModels/PickupMystery";
            SpritePath = "Textures/MiscIcons/texMysteryIcon";
            Tier = ItemTier.Tier1;
            Tags = new ItemTag[]
            {
                ItemTag.AIBlacklist
            };
        }

        public override void Hook()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += MaybeAwardLoot;
            On.RoR2.Chat.AddPickupMessage += FixPickupMessage;
        }

        private void FixPickupMessage(On.RoR2.Chat.orig_AddPickupMessage orig, CharacterBody body, string pickupToken, Color32 pickupColor, uint pickupQuantity)
        {
            if(pickupToken == Name.Token && pickupQuantity == 0)
            {
                pickupQuantity = 3;
            }
            orig(body, pickupToken, pickupColor, pickupQuantity);
        }

        private void MaybeAwardLoot(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var amount = self.inventory.GetItemCount(Definition.itemIndex);
            if (amount > 0 && amount % 3 == 0)
            {
                self.inventory.RemoveItem(Definition.itemIndex, 3);
                PickupIndex loot;
                if (Util.CheckRoll(5, self.master))
                {
                    loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier3DropList);
                }
                else
                {
                    loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier2DropList);
                }
                if(self.isPlayerControlled)
                    PickupDropletController.CreatePickupDroplet(loot, self.corePosition, Vector3.up * 5);
                else
                {
                    PickupDef def = PickupCatalog.GetPickupDef(loot);
                    self.inventory.GiveItem(def.itemIndex);
                    var lootCount = self.inventory.GetItemCount(def.itemIndex);
                    Chat.AddPickupMessage(self, def.nameToken, ColorCatalog.GetColor(ItemCatalog.GetItemDef(def.itemIndex).colorIndex), (uint) lootCount);
                }
            }
        }
    }
}
