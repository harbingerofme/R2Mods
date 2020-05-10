using JetBrains.Annotations;
using R2API;
using RoR2;
using UnityEngine;

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

    public abstract class Pickup
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
        public ItemDisplayRuleDict DisplayRules = null;

        public abstract void Hook();

        public void AddTokens(HarbCratePlugin plugin)
        {
            plugin.AddLanguage(Name);
            plugin.AddLanguage(Description);
            plugin.AddLanguage(PickupText);
            plugin.AddLanguage(Lore);
        }

        protected void Log(string message)
        {
            HarbCratePlugin.Log(message);
        }
    }

    public abstract class Item : Pickup
    {
        public Item() : base()
        { }

        public ItemTier Tier;
        public ItemTag[] Tags = new ItemTag[0];

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
                        tier = Tier,
                        tags = Tags,
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
                    customDef = new CustomItem(Definition, DisplayRules);
                }
                return customDef;
            }
        }
    }

    public abstract class Equip : Pickup
    {
        public Equip() : base()
        { }

        public float Cooldown;
        public bool IsLunar = false;
        public bool IsEnigmaCompat = true;
        protected Vector3 DefaultScale = new Vector3(1,1,1);
        public abstract bool Effect(EquipmentSlot equipmentSlot);

        private CustomEquipment customDef;

        public CustomEquipment CustomDef
        {
            get
            {
                if (customDef == null)
                {
                    customDef = new CustomEquipment(Definition, DisplayRules);
                }
                return customDef;
            }
        }

        protected void SetupDisplayRules()
        {
            var Prefab = Resources.Load<GameObject>(AssetPath);
            var Rule = ItemDisplayRuleType.ParentedPrefab;
            DisplayRules = new ItemDisplayRuleDict(new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(0.2f, 0.15f, 0),
                localAngles = new Vector3(-0.0001368911f, 90.00013f, 180),
                localScale = DefaultScale * 0.3f
            });
            DisplayRules.Add("mdlCroco", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-1.3f, 1.31f, 0.57f),
                localAngles = new Vector3(0.4302638f, 112.6148f, 178.8079f),
                localScale = DefaultScale * 2
            });
            DisplayRules.Add("mdlEngi", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.17f, 0.18f, -0.07f),
                localAngles = new Vector3(0.1981713f, 89.21727f, 182.4698f),
                localScale = DefaultScale * 0.3f
            });
            DisplayRules.Add("mdlHuntress", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(0.129f, 0.043f, 0.105f),
                localAngles = new Vector3(0f, 120.74f, 186.3f),
                localScale = DefaultScale * 0.25f
            });
            DisplayRules.Add("mdlLoader", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.18f, 0.21f, 0.092f),
                localAngles = new Vector3(352.083f, 112.7895f, 178.7964f),
                localScale = DefaultScale * 0.3f
            });
            DisplayRules.Add("mdlMage", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.1f, 0.24f, 0.107f),
                localAngles = new Vector3(354.2113f, 105f, 180f),
                localScale = DefaultScale * 0.25f
            });
            DisplayRules.Add("mdlMerc", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.16f, -0.7f, 0),
                localAngles = new Vector3(356.9035f, 89.99998f, 180),
                localScale = DefaultScale * 0.3f
            });
            DisplayRules.Add("mdlScav", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Chest",
                localPos = new Vector3(14.04f, 2.7f, 6.18f),
                localAngles = new Vector3(35f, 280.3f, 358.8f),
                localScale = DefaultScale * 3f
            });
            DisplayRules.Add("mdlToolbot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(-0.09f, 0.15f, 0.88f),
                localAngles = new Vector3(0.8589115f, 1.487932f, 240.0112f),
                localScale = DefaultScale * 1.5f
            });
            DisplayRules["mdlHAND"] = DisplayRules["mdlToolbot"];
            DisplayRules.Add("mdlTreebot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "PlatformBase",
                localPos = new Vector3(-0.702f, -0.046f, -0.017f),
                localAngles = new Vector3(0, 261.7092f, 0),
                localScale = DefaultScale * 0.4f
            });
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
                        canDrop = true,
                        enigmaCompatible = IsEnigmaCompat,
                        unlockableName = ""
                    };
                }
                return definition;
            }
        }
    }
}