using System.Linq;
using System.Reflection;
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
    public class EquipmentAttribute : System.Attribute
    { }
    
    [MeansImplicitUse]
    public class ItemAttribute : System.Attribute
    { }
    
    public abstract class Singleton
    {
        public static Singleton Instance;
        public Singleton()
        {
            Instance = this;
        }
        
    }
    public abstract class Pickup : Singleton
    {
        public Pickup() : base()
        {
        }
        public TokenValue Name;
        public TokenValue Description;
        public TokenValue PickupText;
        public TokenValue Lore;
        public string AssetPath;
        public string SpritePath;
        public ItemDisplayRule[] DisplayRules = null;

        public abstract void Hook();
    }

    public abstract class Item : Pickup
    {
        public Item() : base()
        { }

        public ItemTier Tier;

        private ItemDef _definiton = null;
        public ItemDef Definition
        {
            get
            {
                if (_definiton == null)
                {
                    _definiton = new ItemDef
                    {
                        name = Name.Value,
                        canRemove = true,
                        descriptionToken = Description.Token,
                        hidden = (Tier == (Tier) - 1),
                        loreToken = Lore.Token,
                        nameToken = Name.Token,
                        itemIndex = ItemIndex.None,
                        pickupIconPath = SpritePath,
                        pickupModelPath = AssetPath,
                        pickupToken = PickupText.Token,
                        unlockableName = ""
                    };
                }
                return _definiton;
            }
        }
        private CustomItem customDef;

        public CustomItem CustomDef
        {
            get
            {
                if (customDef == null)
                {
                    customDef = new CustomItem(Definition,DisplayRules);
                }
                return customDef;
            }
        }
    }

    public  abstract  class Equip:  Pickup
    {
        public Equip() : base()
        { }
        
        public float Cooldown;
        public bool IsLunar;
        public bool IsEnigmaCompat;
        public abstract bool Effect(EquipmentSlot equipmentSlot);

        private CustomEquipment customDef;

        public CustomEquipment CustomDef
        {
            get
            {
                if (customDef == null)
                {
                    customDef = new CustomEquipment(Definition,DisplayRules);
                }
                return customDef;
            }
        }
        
        
        private EquipmentDef definition;
        public EquipmentDef Definition
        {
            get
            {
                if (definition == null)
                {
                    definition = new EquipmentDef()
                    {
                        name = Name.Value,
                        descriptionToken = Description.Token,
                        loreToken = Lore.Token,
                        nameToken = Name.Token,
                        pickupIconPath = SpritePath,
                        pickupModelPath = AssetPath,
                        pickupToken = PickupText.Token,
                        cooldown = Cooldown,
                        isLunar = IsLunar,
                        enigmaCompatible= IsEnigmaCompat,
                        unlockableName = ""
                    };
                }
                return definition;
            }
        }
    }
}