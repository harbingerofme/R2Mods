using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using R2API;
using R2API.Utils;
using System.Reflection;
using RoR2;

/*
    Code By Guido "Harb". 
     */

namespace HarbCrate
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2APISubmoduleDependency(nameof(R2API.AssetPlus), nameof(R2API.ItemAPI))]
    [BepInPlugin("com.harbingerofme.HarbCrate", "HarbCrate", "0.0.0")]
    public class HarbCratePlugin : BaseUnityPlugin
    {
        public Dictionary<EquipmentIndex,Equip> Equipment;
        public Dictionary<ItemIndex,Item> Items;

        public HarbCratePlugin()
        {
            Equipment = new Dictionary<EquipmentIndex, Equip>();
            Items = new Dictionary<ItemIndex, Item>();
        }

        public void Awake()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                bool flagItem = type.GetCustomAttribute<ItemAttribute>() != null;
                bool flagEquip = type.GetCustomAttribute<EquipmentAttribute>() != null;
                Pickup pickup = null;
                if (flagItem)
                {
                    Item myItem = (Item) Activator.CreateInstance(type);
                    ItemAPI.AddCustomItem(myItem.CustomDef);
                    Items.Add(myItem.Definition.itemIndex,myItem);
                    pickup = myItem;
                }
                else if (flagEquip)
                {
                    Equip myEquip = (Equip) Activator.CreateInstance(type);
                    ItemAPI.AddCustomEquipment(myEquip.CustomDef);
                    Equipment.Add(myEquip.Definition.equipmentIndex,myEquip);
                    pickup = myEquip;
                }

                if (flagEquip || flagItem)
                {
                    foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Public| BindingFlags.NonPublic).Where((info => info.FieldType == typeof(TokenValue))))
                    {
                        AddLanguage((TokenValue) fieldInfo.GetValue(pickup));
                    }
                    pickup.Hook();
                }
            }

            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, index) =>
            {
                bool flag = orig(self, index);
                if (!flag && Equipment.ContainsKey(index))
                {
                    return Equipment[index].Effect(self);
                }
                return flag;
            };
        }


        public static void AddLanguage(TokenValue tv)
        {
            R2API.AssetPlus.Languages.AddToken(tv.Token,tv.Value);
        }

    }
}
