using RoR2;

namespace HarbCrate
{
    internal static class Extensions
    {
        public static bool HasItem(this Inventory inventory, ItemIndex itemIndex, out int itemCount)
        {
            if (inventory)
            {
                itemCount = inventory.GetItemCount(itemIndex);
                if(itemCount > 0)
                {
                    return true;
                }
            }
            itemCount = 0;
            return false;
        }

        public static bool HasItem(this Inventory inventory, ItemIndex itemIndex)
        {
            return inventory.HasItem(itemIndex, out int _);
        }

        public static bool HasItem(this Inventory inventory, Item item)
        {
            return inventory.HasItem(item.Definition.itemIndex);
        }

        public static bool HasItem(this Inventory inventory, Item item, out int itemCount)
        {
            return inventory.HasItem(item.Definition.itemIndex, out itemCount);
        }

        public static bool HasEquip(this Inventory inventory, Equip equip)
        {
            return inventory.GetEquipmentIndex() == equip.Definition.equipmentIndex;
        }
    }
}
