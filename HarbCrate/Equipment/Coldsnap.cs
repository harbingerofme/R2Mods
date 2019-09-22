using ItemLib;
using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    public sealed class ColdSnap : HarbEquipment
    {
        private new static readonly float Cooldown = 64;
        private static readonly float Radius = 16;
        private static readonly float Duration = 3;
        public new static readonly string Name = "Coldsnap";

        public new static CustomEquipment Build()
        {

            EquipmentDef myDef = new EquipmentDef
            {
                cooldown = Cooldown,
                pickupModelPath = "Prefabs/PickupModels/PickupFrostRelic",
                pickupIconPath = "Textures/ItemIcons/texFrostRelicIcon",
                nameToken = Name,
                pickupToken = "Freeze nearby enemies",
                descriptionToken = "Freeze nearby enemies for " + Duration + " seconds. Frozen enemies can be executed.",
                canDrop = true,
                enigmaCompatible = true
            };
            return new CustomEquipment(myDef, null, null, null);
        }

        public new static bool Effect(EquipmentSlot slot)
        {
            var ownerBody = slot.GetComponent<CharacterBody>();
            Vector3 pos = Util.GetCorePosition(ownerBody);
            for (TeamIndex teamindex = TeamIndex.Neutral; teamindex < TeamIndex.Count; teamindex++)
            {
                if (teamindex != ownerBody.teamComponent.teamIndex)
                {
                    foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(teamindex))
                    {
                        if ((teamComponent.transform.position - pos).sqrMagnitude <= Mathf.Pow(Radius, 2))
                        {
                            CharacterBody component = teamComponent.GetComponent<CharacterBody>();
                            if (component)
                            {
                                var state = component.GetComponent<SetStateOnHurt>();
                                if (state)
                                {
                                    state.SetFrozen(Duration);
                                }
                            }
                        }
                    }
                }
            }
            GameObject explosionPreFab = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GenericDelayBlast"), pos, Quaternion.identity);
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
    }
}
