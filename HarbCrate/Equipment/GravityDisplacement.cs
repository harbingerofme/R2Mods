using ItemLib;
using RoR2;
using RoR2.Projectile;
using UnityEngine;


namespace HarbCrate.Equipment
{
    public sealed class GravityDisplacement : HarbEquipment
    {
        public new static readonly float Cooldown = 10;
        public new static readonly string Name = "Gravity Displacement Emitter";
        public new static CustomEquipment Build()
        {
            EquipmentDef myDef = new EquipmentDef
            {
                cooldown = Cooldown,
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/ItemIcons/texDiamondIcon",
                nameToken = Name,
                pickupToken = "Throw a grenade that launches enemies away.",
                descriptionToken = "Throw a grenade that throws enemies up in the air.",
                canDrop = true,

                enigmaCompatible = true
            };
            return new CustomEquipment(myDef, null, null, null);
        }

        public new static bool Effect(EquipmentSlot slot)
        {
            var inputBank = slot.GetComponent<InputBankTest>();
            Ray aimRay = new Ray
            {
                direction = inputBank.aimDirection,
                origin = inputBank.aimOrigin
            };
            GameObject projPrefab = Resources.Load<GameObject>("prefabs/Projectiles/CommandoGrenadeProjectile");
            var projInfo = new FireProjectileInfo
            {
                crit = false,
                damage = 0f,
                force = 10000f,
                owner = slot.GetComponent<CharacterBody>().gameObject,
                position = slot.transform.position,
                projectilePrefab = projPrefab,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
            };
            ProjectileManager.instance.FireProjectile(projInfo);
            return true;
        }
    }
}
