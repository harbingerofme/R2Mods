using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    [R2APISubmoduleDependency(nameof(R2API.AssetPlus), nameof(R2API.ItemAPI), nameof(AssetAPI), nameof(ResourcesAPI))]
    [BepInPlugin("com.harbingerofme.HarbCrate", "HarbCrate", "0.0.0")]
    public class HarbCratePlugin : BaseUnityPlugin
    {
        readonly Dictionary<EquipmentIndex, Equip> equipmentTable;
        private const string assetProvider = "@HarbCrate";
        internal const string assetPrefix = assetProvider + ":";

        public HarbCratePlugin()
        {
            equipmentTable = new Dictionary<EquipmentIndex,Equip>();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HarbCrate.harbcrate"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new R2API.AssetBundleResourcesProvider(assetProvider, bundle);
                R2API.ResourcesAPI.AddProvider(provider);
            }
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
                    ItemAPI.Add(myItem.CustomDef);
                    pickup = myItem;
                }
                else if (flagEquip)
                {
                    Equip myEquip = (Equip) Activator.CreateInstance(type,null);
                    EquipmentIndex index = ItemAPI.Add(myEquip.CustomDef);
                    equipmentTable.Add(index,myEquip);
                    pickup = myEquip;
                }

                if (flagEquip || flagItem)
                {
                    pickup.AddTokens(this);
                    pickup.Hook();
                }
            }

            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, index) =>
            {
                bool flag = orig(self, index);
                if (!flag && equipmentTable.ContainsKey(index))
                {
                    return equipmentTable[index].Effect(self);
                }
                return flag;
            };
        }


        public void AddLanguage(TokenValue tv)
        {
            if (tv.Token!=null && tv.Token != "")
            {
                R2API.AssetPlus.Languages.AddToken(tv.Token, tv.Value);
            }
        }

    }
}
