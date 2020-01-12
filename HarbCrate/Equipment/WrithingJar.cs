using R2API;
using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    [Equipment]
    public sealed class WrithingJar : IPickup, IEquip
    {
        public TokenValue Name => new TokenValue
        {
            Token = "HC_WORMJAR",
            Value = "The Writhing Jar"
        };

        public TokenValue Description => new TokenValue
        {
            Token = "HC_WORMJAR_DESC",
            Value =
                "Summons a <style=cHealing>friendly</style> Magma Worm and a <style=cDeath>HOSTILE</cstyle> Overloading Magma Worm."
        };

        public TokenValue PickupText => new TokenValue
        {
            Token = "HC_WORMJAR_DESC",
            Value = "Inside of you there's two worms. One is a friend, the other a monster. You are a <color=blue>monster</color>."
        };

        public string AssetPath => "";
        public string SpritePath => "";

        public EquipmentDef Definition { get; set; }
        public CustomEquipment CustomDef { get; set; }
        public float Cooldown => 120f;
        public bool IsLunar => true;
        public bool IsEnigmaCompat => true;

        private const float HostileDMG = 3;
        private const float HostileHP = 4.7f;
        private const float AllyDMG = 1.5f;
        private const float AllyHP = 1;

        public bool Effect(EquipmentSlot equipmentSlot)
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
            if (spawn)
            {
                CharacterMaster cm = spawn.GetComponent<CharacterMaster>();
                if (cm)
                {
                    cm.inventory.GiveItem(ItemIndex.BoostDamage, getItemCountFromMultiplier(HostileDMG));
                    cm.inventory.GiveItem(ItemIndex.BoostHp, getItemCountFromMultiplier((HostileHP)));
                }
            }

            var friendCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
            var friendRequest = new DirectorSpawnRequest(friendCard, placementRules, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = false,
                teamIndexOverride = TeamIndex.Player,
                summonerBodyObject = equipmentSlot.GetComponent<CharacterBody>().gameObject
            };
            var spawn2 = DirectorCore.instance.TrySpawnObject(friendRequest);
            spawn2.transform.TransformDirection(0, 100, 0);
            if (spawn2)
            {
                CharacterMaster cm = spawn2.GetComponent<CharacterMaster>();
                if (cm)
                {
                    cm.inventory.GiveItem(ItemIndex.BoostDamage, getItemCountFromMultiplier(AllyDMG));
                    cm.inventory.GiveItem(ItemIndex.BoostHp, getItemCountFromMultiplier((AllyHP)));
                }   
            }
            if (spawn || spawn2)
                return true;
            return false;
        }


        private int getItemCountFromMultiplier(float multiplier)
        {
            if (multiplier <= 1)
                return 0;
            return Mathf.CeilToInt((multiplier - 1) * 10);
        }

    }
}
