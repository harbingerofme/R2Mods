using R2API;
using RoR2;
using JetBrains.Annotations;


namespace HarbCrate
{
    public struct TokenValue
    {
        public string Token;
        public string Value;
    }
    
    [MeansImplicitUse]
    class EquipmentAttribute : System.Attribute
    {
        public EquipmentAttribute()
        {
        }
    }
    
    [MeansImplicitUse]
    class ItemAttribute : System.Attribute
    {
        public ItemAttribute()
        {
        }
    }

    public interface IPickup
    {
        TokenValue Name { get; }
        TokenValue Description { get; }
        TokenValue PickupText { get; }
        string AssetPath { get; }
        string SpritePath { get; }
    }

    public interface IItem
    {
        ItemTier Tier { get; }
        ItemDef Definition { get; set; }
        CustomItem CustomDef { get; set; }
    }

    public interface IEquip
    {
        EquipmentDef Definition { get; set; }
        CustomEquipment CustomDef { get; set; }
        float Cooldown { get; }
        bool IsLunar { get; }
        bool IsEnigmaCompat { get; }
        bool Effect(EquipmentSlot equipmentSlot);
    }
}