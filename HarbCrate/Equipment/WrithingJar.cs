using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    [Equipment]
    internal sealed class WrithingJar : Equip
    {
        public WrithingJar() : base()
        {

            Name = new TokenValue("HC_WORMJAR", "The Writhing Jar");
            Description = new TokenValue("HC_WORMJAR_DESC",
                "Summons a <style=cIsHealing>friendly</style> Magma Worm and a <style=cDeath>HOSTILE</cstyle> <color=blue>Overloading Magma Worm</color>.");
            PickupText = new TokenValue("HC_WORMJAR_DESC",
                "Inside of you there's two worms. One is a friend, the other a monster. You are a <color=blue>monster</color>.");
            Lore = new TokenValue("HC_WORMJAR_LORE",
                "<style=cStack>Recovered from a half burned notebook, near unmarked research facility at [Redacted]</style>\r\n<style=cIsUtility>Day 4:</style>\nThe writings said the urn was the 'vessel of a great scourge', that it was a 'tremendous power that turned breath to smoke and flesh to ash'. Mikhail thinks it's just a jar of dead worms - probably a plague that they didn't understand and some leeches that they used to try and 'draw it out'.Or something.But... I'm not so sure. \n\n<style=cIsUtility>Day 12:</style>\nI've been having a dream lately, more or less the same every night. It's always there, almost like it's waiting. The air feels like being in an oven; heavy and so hot it feels like I'm going to burn up or dry out or both. I open up the lid, and reach inside. The skin on my fingers blisters and cracks. I push my hands in deeper and my flesh turns to char, and then to dust...but I don't feel anything. It's almost a relief, to be free from that heat. I fall inside, and my body turns to smoke that joins the darkness.It's a little bit unsettling, to be sure.And yet, I keep having this feeling like there's something more here. Something coiled just beneath the surface.\nI need to run some more tests. Maybe I can coax these dried husks back to life, or clone them, or something. Get some answers.\nI'll keep it to myself for now - Mikhail would just think I'm being stupid - but there is more to this jar than just a couple of silly little worms.\n<style=cStack>The rest of the log is unreadable.</style>");
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/2_Worm_Jar/WormJar.prefab";
            SpritePath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/2_Worm_Jar/worm.png";
            Cooldown = 120f;
            IsLunar = true;
            IsEnigmaCompat = true;

            hostileCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
            friendCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");

            DefaultScale *= 2.3f;
            SetupDisplayRules();
        }

        private readonly SpawnCard hostileCard;
        private readonly SpawnCard friendCard;

        private const float HostileDMG = 2;
        private const float HostileHP = 4.7f;
        private const float AllyDMG = 1.5f;
        private const float AllyHP = 2;

        public override bool Effect(EquipmentSlot equipmentSlot)
        {
            var transform = equipmentSlot.transform;
            var placementRules = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 10f,
                maxDistance = 100f,
                spawnOnTarget = transform
            };
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
                    cm.inventory.GiveItem(ItemIndex.BoostDamage, GetItemCountFromMultiplier(HostileDMG));
                    cm.inventory.GiveItem(ItemIndex.BoostHp, GetItemCountFromMultiplier((HostileHP)));
                }
            }
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
                    cm.inventory.GiveItem(ItemIndex.BoostDamage, GetItemCountFromMultiplier(AllyDMG));
                    cm.inventory.GiveItem(ItemIndex.BoostHp, GetItemCountFromMultiplier((AllyHP)));
                }
            }
            if (spawn || spawn2)
                return true;
            return false;
        }


        private int GetItemCountFromMultiplier(float multiplier)
        {
            if (multiplier <= 1)
                return 0;
            return Mathf.CeilToInt((multiplier - 1) * 10);
        }

        public override void Hook()
        { }
    }
}
