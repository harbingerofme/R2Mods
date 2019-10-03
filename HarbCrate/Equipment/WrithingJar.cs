using ItemLib;
using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    public sealed class WrithingJar : HarbEquipment
    {
        public new static readonly float Cooldown = 120;
        public new static readonly string Name = "The Writhing Jar";

        public new static CustomEquipment Build() {
            EquipmentDef myDef = new EquipmentDef
            {
                cooldown = Cooldown,
                pickupModelPath = "Prefabs/PickupModels/PickupWilloWisp",
                pickupIconPath = "Textures/ItemIcons/texWilloWispIcon",
                nameToken = Name,
                pickupToken = "Inside of you there's two worms. One is a friend, the other a monster. You are a monster.",
                descriptionToken = "Summons a friendly Magma Worm and a HOSTILE Overloading Magma Worm.",
                canDrop = true,
                isLunar = true,
                colorIndex = ColorCatalog.ColorIndex.LunarItem,
                enigmaCompatible = false
            };
            return new CustomEquipment(myDef, null, null, null);
        }
        public new static bool Effect(EquipmentSlot equipmentSlot)
        {
            
            var transform = equipmentSlot.transform;
            var placementRules = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 10f,
                maxDistance = 100f,
                spawnOnTarget = transform
            };
            var hostileCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
            var hateRequest = new DirectorSpawnRequest(hostileCard, placementRules, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = false,
                teamIndexOverride = TeamIndex.Monster
            };
            var spawn = DirectorCore.instance.TrySpawnObject(hateRequest);
            spawn.transform.TransformDirection(0, 100, 0);
            CharacterMaster cm = spawn.GetComponent<CharacterMaster>();
            cm.inventory.GiveItem(ItemIndex.BoostDamage, 20);
            cm.inventory.GiveItem(ItemIndex.BoostHp, 47);
            var friendCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
            var friendRequest = new DirectorSpawnRequest(friendCard, placementRules, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = false,
                teamIndexOverride = TeamIndex.Player,
                summonerBodyObject = equipmentSlot.GetComponent<CharacterBody>().gameObject
            };
            var spawn2 = DirectorCore.instance.TrySpawnObject(friendRequest);
            spawn2.transform.TransformDirection(0, 100, 0);
            if (spawn || spawn2)
                return true;
            return false;
        }
    }
}
