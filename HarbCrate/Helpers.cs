using R2API;
using RoR2;
using JetBrains.Annotations;


namespace HarbCrate
{
    public struct TokenValue
    {
        public TokenValue(string token, string value)
        {
            Token = token;
            Value = value;
        }
        public string Token;
        public string Value;
    }
    
    [MeansImplicitUse]
    internal class EquipmentAttribute : System.Attribute
    { }
    
    [MeansImplicitUse]
    internal class ItemAttribute : System.Attribute
    { }

    internal abstract class Singleton
    {
        public static Singleton Instance;
        public Singleton()
        {
            Instance = this;
        }
        
    }
    internal abstract class Pickup : Singleton
    {
        public Pickup():base() { }
        public TokenValue Name;
        public TokenValue Description;
        public TokenValue PickupText;
        public string AssetPath;
        public string SpritePath;

        public abstract void Hook();
    }

    internal abstract class Item : Pickup
    {
        public Item() : base()
        { }

        public ItemTier Tier;
        public ItemDef Definition;
        public CustomItem CustomDef;
    }

    internal  abstract  class Equip:  Pickup
    {
        public Equip() : base()
        { }

        public EquipmentDef Definition;
        public CustomEquipment CustomDef;
        public float Cooldown;
        public bool IsLunar;
        public bool IsEnigmaCompat;
        public abstract bool Effect(EquipmentSlot equipmentSlot);
    }
}