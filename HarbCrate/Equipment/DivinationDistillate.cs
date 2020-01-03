using RoR2;
using UnityEngine;


namespace HarbCrate.Equipment
{
    public sealed class DivinationDistillate : HarbEquipment
    {
        public new static readonly float Cooldown = 30;
        public  new static readonly string Name = "Divination Distillate";
        public static readonly string BuffName = "DistillateBuff";
        public static readonly float DistillateChance = 5;
        public static readonly float DistillateDuration = 5;

        public new static CustomEquipment Build()
        {
            EquipmentDef myDef = new EquipmentDef
            {
                cooldown = Cooldown,
                pickupModelPath = "Prefabs/PickupModels/PickupSoda",
                pickupIconPath = "Textures/ItemIcons/texSodaIcon",
                nameToken = Name,
                pickupToken = "Heal both health and shield for a short period. While healing, slain enemies can drop items.",
                descriptionToken = "Heal both health and shields for " + DistillateDuration + " seconds. Effects stops at full health and full shields. While under effect, slain enemies have a " + DistillateChance + "% chance to drop an item.",
                canDrop = true,
                enigmaCompatible = true
            };
            return new CustomEquipment(myDef, null, null, null);
        }

        public new static bool Effect(EquipmentSlot slot)
        {
            var ownerBody = slot.GetComponent<CharacterBody>();
            if (!ownerBody)
                return false;
            var ownerHealthComponent = ownerBody.GetComponent<HealthComponent>();
            if (!ownerHealthComponent || (ownerHealthComponent.health == ownerHealthComponent.fullHealth && ownerHealthComponent.shield == ownerHealthComponent.fullShield && ownerHealthComponent.godMode == false))
                return false;
            var distillateComp = ownerBody.gameObject.AddComponent<DistillateBehaviour>();
            distillateComp.hc = ownerHealthComponent;
            distillateComp.cb = ownerBody;
            ownerBody.AddBuff((BuffIndex)ItemLib.ItemLib.GetBuffId(BuffName));
            return true;
        }

        public static CustomBuff Buff()
        {
            var buffDef = new BuffDef
            {
                buffColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                canStack = false
            };
            return new CustomBuff(BuffName, buffDef, null);
        }

        public class DistillateBehaviour : MonoBehaviour
        {
            private float lifeTime = DistillateDuration;
            private readonly float interval = 0.2f;
            private float nextHealIn = 0f;
            private readonly float totalHealthFraction = 0.2f;
            private readonly float totalShieldFraction = 0.5f;

            private float intervalFraction;
            public HealthComponent hc;
            public CharacterBody cb;

#pragma warning disable IDE0051 // Remove unused private members
            private void Awake()
            {
                this.intervalFraction = interval / DistillateDuration;
            }

            private void OnDestroy()
            {
                if (this.cb)
                    this.cb.RemoveBuff((BuffIndex)ItemLib.ItemLib.GetBuffId(BuffName));
            }

            private void FixedUpdate()
            {
                this.nextHealIn -= Time.deltaTime;
                this.lifeTime -= Time.fixedDeltaTime;
                if (this.nextHealIn < 0 || this.lifeTime < 0)
                {
                    this.nextHealIn += this.interval;
                    if (this.hc)
                    {
                        this.hc.RechargeShield(this.hc.fullShield * this.totalShieldFraction * this.intervalFraction);
                        this.hc.Heal(this.hc.fullHealth * this.totalHealthFraction * this.intervalFraction, default, true);
                    }
                }
                if (this.lifeTime <= 0f || (this.hc.fullHealth == this.hc.health && this.hc.shield == this.hc.fullShield && this.hc.godMode == false))
                {
                    Destroy(this);
                }
            }
#pragma warning restore IDE0051 // Remove unused private members
        }

        public static void DistillateQuantEffect(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            CharacterBody attackerBody = damageReport.attackerBody;
            CharacterMaster attackerMaster = damageReport.attackerMaster;
            GameObject victim = damageReport.victim.gameObject;
            if (attackerBody && attackerMaster)
            {
                if (attackerBody.GetComponent<DistillateBehaviour>())//We could also check for the DistillateBuff existing, however, the component is far more reliable.
                {
                    if (Util.CheckRoll(DistillateChance, attackerMaster))
                    {
                        PickupIndex[] list = Run.instance.smallChestDropTierSelector.Evaluate(UnityEngine.Random.value).ToArray();
                        PickupIndex pickupIndex = PickupIndex.none;
                        if (list.Length > 0)
                        {
                            pickupIndex = list[Random.Range(0, list.Length - 1)];
                        }
                        PickupDropletController.CreatePickupDroplet(pickupIndex, victim.transform.position, Vector3.up * 20f);
                    }
                }
            }

        }
    }
}
