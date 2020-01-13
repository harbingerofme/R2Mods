using R2API;
using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    [Equipment]
    internal sealed class ColdSnap : Equip
    {
        private const float Radius = 16;
        private const float FreezeDuration = 3;


        public ColdSnap():base()
        {
            Cooldown = 64;
            Name = new TokenValue("HC_COLDSNAP","Coldsnap");
            PickupText = new TokenValue("HC_COLDSNAP_PICKUP","Freeze nearby enemies.");
            Description = new TokenValue("HC_COLDSNAP_DESC",
                $"Freeze nearby enemies for {FreezeDuration} seconds."
                + " Frozen enemies can be executed."
            );
            IsLunar = false;
            IsEnigmaCompat = true;
            AssetPath = "";
            SpritePath = "";
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

        public override void Hook()
        { }
    }
}
