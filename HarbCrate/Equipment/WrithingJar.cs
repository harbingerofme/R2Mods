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
                pickupToken = "Overconfidence is a screeching and static killer.",
                descriptionToken = "Summons a HOSTILE Overloading Magma Worm.",
                canDrop = true,
                isLunar = true,
                colorIndex = ColorCatalog.ColorIndex.LunarItem,
                enigmaCompatible = false
            };
            return new CustomEquipment(myDef, null, null, null);
        }
        public new static bool Effect(EquipmentSlot equipmentSlot)
        {
            var card = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
            var transform = equipmentSlot.transform;
            var placementRules = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 10f,
                maxDistance = 100f,
                spawnOnTarget = transform
            };
            var request = new DirectorSpawnRequest(card, placementRules, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = false,
                teamIndexOverride = TeamIndex.Monster
            };
            var spawn = DirectorCore.instance.TrySpawnObject(request);
            spawn.transform.TransformDirection(0, 100, 0);
            if (spawn)
                return true;
            return false;
        }
    }
}
