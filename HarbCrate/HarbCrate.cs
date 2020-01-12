

using BepInEx;
using RoR2;
using HarbCrate.Equipment;
using HarbCrate.Items;
using R2API;
using R2API.Utils;


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
        private EquipmentIndex[] myEquipmentIDs;
        private ItemIndex[] myItemIds;

        public void Awake()
        {
            
        }

    }
}
