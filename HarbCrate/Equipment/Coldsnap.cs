using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    [Equipment]
    internal sealed class ColdSnap : Equip
    {
        private const float Radius = 16;
        private const float FreezeDuration = 3;


        public ColdSnap() : base()
        {
            Cooldown = 64;
            Name = new TokenValue("HC_COLDSNAP", "Coldsnap");
            PickupText = new TokenValue("HC_COLDSNAP_PICKUP", "<style=cIsUtility>Freeze</style> nearby enemies.");
            Description = new TokenValue("HC_COLDSNAP_DESC",
                $"<style=cIsUtility>Freeze</style> nearby enemies for {FreezeDuration} seconds."
                + " Frozen enemies can be <style=cIsDamage>executed</style>."
            );
            Lore = new TokenValue
            {
                Token = "HC_COLDSNAP_LORE",
                Value = "Order: \"Instant Antiheat Crystal\"\r\nTracking Number: 23******\r\nEstimated Delivery: 02/29/2056\r\nShipping Method:  Standard/Fragile\r\nShipping Address: Tortoise Infrastructure, Salvador, Earth\r\nShipping Details:\r\n\r\nTo safeguard the interests of our clients, we need to up our fire proofing. I have ordered a test run of IACs for you. Install them and find out if they are worth investing into. "
            };
            IsLunar = false;
            IsEnigmaCompat = true;
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/IceShard/ColdSnap.prefab";
            SpritePath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/IceShard/coldsnap.png";

            DefaultScale *= 60;
            SetupDisplayRules();
        }

        public override bool Effect(EquipmentSlot slot)
        {
            var ownerBody = slot.GetComponent<CharacterBody>();
            Vector3 pos = Util.GetCorePosition(ownerBody);
            for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex++)
            {
                if (teamIndex != ownerBody.teamComponent.teamIndex)
                {
                    foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(teamIndex))
                    {
                        if ((teamComponent.transform.position - pos).sqrMagnitude <= Mathf.Pow(Radius, 2))
                        {
                            CharacterBody component = teamComponent.GetComponent<CharacterBody>();
                            if (component)
                            {
                                var state = component.GetComponent<SetStateOnHurt>();
                                if (state)
                                {
                                    state.SetFrozen(FreezeDuration);
                                }
                            }
                        }
                    }
                }
            }
            GameObject explosionPreFab = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), pos, Quaternion.identity);
            explosionPreFab.transform.localScale = new Vector3(Radius, Radius, Radius);
            DelayBlast delayBlast = explosionPreFab.GetComponent<DelayBlast>();
            delayBlast.explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
            delayBlast.delayEffect = Resources.Load<GameObject>("Prefabs/Effects/AffixWhiteDelayEffect");
            delayBlast.position = pos;
            delayBlast.baseDamage = 0;
            delayBlast.baseForce = 0f;
            delayBlast.radius = Radius;
            delayBlast.maxTimer = 0.1f;
            delayBlast.procCoefficient = 0f;

            return true;
        }

        public override void Hook()
        { }

        /*
        private void SetupDisplayRules()
        {
            var Prefab = Resources.Load<GameObject>(AssetPath);
            var Rule = ItemDisplayRuleType.ParentedPrefab;
            var angles = new Vector3(275, 180, 180);
            var reverseAngles = new Vector3(275, 0, 180);
            DisplayRules = new R2API.ItemDisplayRuleDict(new ItemDisplayRule()
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(20, 20, 20),
                localPos = new Vector3(0.43f, -0.72f, -0.43f),
                localAngles = angles
            });
            DisplayRules.Add("mdlToolbot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(175, 175, 175),
                localPos = new Vector3(-3.39f, -8f, 3f),
                localAngles = reverseAngles
            });
            DisplayRules.Add("mdlCroco", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(175, 175, 175),
                localPos = new Vector3(3, 4f, 3f),
                localAngles = reverseAngles
            });
            //Dunno why I bother with turrets, but they got'em
            DisplayRules.Add("mdlEngiTurret", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Head",
                localScale = new Vector3(25, 25, 25),
                localPos = new Vector3(1.4f, 1f, -1f),
                localAngles = angles
            });
            DisplayRules["mdlEngiWalkerTurret"] = DisplayRules["mdlEngiTurret"];//The bases are the same.... hopefully?
            DisplayRules.Add("mdlScav", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Base",
                localScale = new Vector3(200, 200, 200),
                localPos = new Vector3(5f, 10f, 5f),
                localAngles = angles
            });
            DisplayRules["mdlHAND"] = DisplayRules["mdlToolbot"];
        }
        */
    }
}
