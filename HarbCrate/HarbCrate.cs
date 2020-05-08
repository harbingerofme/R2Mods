using BepInEx;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/*
    Code By Guido "Harb". 
     */

namespace HarbCrate
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2API.Utils.R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ItemAPI), nameof(AssetAPI), nameof(ResourcesAPI), nameof(ItemDropAPI), nameof(R2API.Utils.CommandHelper))]
    [BepInPlugin(GUID, Name, Version)]
    public class HarbCratePlugin : BaseUnityPlugin
    {
        public const string Name = "HarbCrate";
        public const string GUID = "com.harbingerofme." + Name;
        public const string Version = "0.0.0";


        private static BepInEx.Logging.ManualLogSource logger;
        public static Dictionary<string, Pickup> AllPickups = new Dictionary<string, Pickup>();
        readonly Dictionary<EquipmentIndex, Equip> equipmentTable;
        private const string assetProvider = "@HarbCrate";
        internal const string assetPrefix = assetProvider + ":";

        public HarbCratePlugin()
        {
            logger = Logger;
            equipmentTable = new Dictionary<EquipmentIndex, Equip>();

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
                    Item myItem = (Item)Activator.CreateInstance(type);
                    ItemAPI.Add(myItem.CustomDef);
                    pickup = myItem;
                }
                else if (flagEquip)
                {
                    Equip myEquip = (Equip)Activator.CreateInstance(type, null);
                    EquipmentIndex index = ItemAPI.Add(myEquip.CustomDef);
                    equipmentTable.Add(index, myEquip);
                    pickup = myEquip;
                }

                if (flagEquip || flagItem)
                {
                    pickup.AddTokens(this);
                    pickup.Hook();
                    AllPickups.Add(type.Name, pickup);
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

            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
        }

        public static event EventHandler Started;

        private void Start()
        {
            Started.Invoke(this, null);
        }


        public void AddLanguage(TokenValue tv)
        {
            if (tv.Token != null && tv.Token != "")
            {
               LanguageAPI.Add(tv.Token, tv.Value);
            }
        }

        internal static void Log(object input)
        {
            if (logger != null)
            {
                logger.LogMessage(input);
            }
        }
    }
}
