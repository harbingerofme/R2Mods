

using BepInEx;
using ItemLib;
using RoR2;
using HarbCrate.Equipment;
using HarbCrate.Items;


/*
    Code By Guido "Harb". 
     */

namespace HarbCrate
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(ItemLibPlugin.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.harbingerofme.HarbCrate", "HarbCrate", "0.0.0")]
    public class HarbCratePlugin : BaseUnityPlugin
    {
        private EquipmentIndex[] myEquipmentIDs;
        private ItemIndex[] myItemIds;


        BepInEx.Logging.ManualLogSource log;

        public void Awake()
        {
            log = base.Logger;

            myEquipmentIDs = new EquipmentIndex[4];

            myEquipmentIDs[0] = (EquipmentIndex)ItemLib.ItemLib.GetEquipmentId(ColdSnap.Name);
            myEquipmentIDs[1] = (EquipmentIndex)ItemLib.ItemLib.GetEquipmentId(DivinationDistillate.Name);
            myEquipmentIDs[2] = (EquipmentIndex)ItemLib.ItemLib.GetEquipmentId(WrithingJar.Name);
            myEquipmentIDs[3] = (EquipmentIndex)ItemLib.ItemLib.GetEquipmentId(GravityDisplacement.Name);


            myItemIds = new ItemIndex[2];

            myItemIds[0] = (ItemIndex)ItemLib.ItemLib.GetItemId(DebuffReducer.Name);
            myItemIds[1] = (ItemIndex)ItemLib.ItemLib.GetItemId(BrawnOverBrain.Name);

            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, equipSlot, equipIndex) =>
            {
                if (equipIndex == myEquipmentIDs[0])
                    return ColdSnap.Effect(equipSlot);
                if (equipIndex == myEquipmentIDs[1])
                    return DivinationDistillate.Effect(equipSlot);
                if (equipIndex == myEquipmentIDs[2])
                    return WrithingJar.Effect(equipSlot);
                if (equipIndex == myEquipmentIDs[3])
                    return GravityDisplacement.Effect(equipSlot);
                return orig(equipSlot, equipIndex);
            };

            On.RoR2.GlobalEventManager.OnCharacterDeath += DivinationDistillate.DistillateQuantEffect;
            DebuffReducer.Hooks();
            BrawnOverBrain.Hooks();

            log.LogError("BRIGHT RED SO YOU CAN FIND IT:");
            log.LogError("\t Equipment: coldsnap=" + (int)myEquipmentIDs[0] + ", distillate=" + (int)myEquipmentIDs[1] + ", writhing jar=" + (int)myEquipmentIDs[2] + ", gravnade=" + (int)myEquipmentIDs[3]);
            log.LogError("\t Items: reduceDebuffs=" + (int)myItemIds[0]+ ", brawnbrains="+(int)myItemIds[1]);
        }


        [Item(ItemAttribute.ItemType.Equipment)]
        public static CustomEquipment Coldsnap()
        {
            return ColdSnap.Build();
        }

        [Item(ItemAttribute.ItemType.Equipment)]
        public static CustomEquipment HealAndLoot()
        {
            return DivinationDistillate.Build();
        }

        [Item(ItemAttribute.ItemType.Equipment)]
        public static CustomEquipment SummonWorms()
        {
            return WrithingJar.Build();
        }

        [Item(ItemAttribute.ItemType.Equipment)]
        public static CustomEquipment GravityWellEquip()
        {

            return GravityDisplacement.Build();
        }

        [Item(ItemAttribute.ItemType.Buff)]
        public static CustomBuff DistillateBuff()
        {
            return DivinationDistillate.Buff();
        }

        [Item(ItemAttribute.ItemType.Item)]
        public static CustomItem ReduceDebuffs()
        {
            return DebuffReducer.Build();
        }

        [Item(ItemAttribute.ItemType.Item)]
        public static CustomItem Brawnoverbrain()
        {
            return BrawnOverBrain.Build();
        }
    }
}
