using R2API;
using RoR2;
using UnityEngine;


namespace HarbCrate.Equipment
{
    [Equipment]
    internal sealed class DivinationDistillate : Equip
    {
        private const int DistillateLuckIncrease = 2;
        private const float DistillateDuration = 7f;
        private const float Interval = 0.2f;
        private const float TotalHealthFraction = 0.2f;
        private const float TotalShieldFraction = 0.5f;

        private static BuffDef DistillateBuff;

        public DivinationDistillate() : base()
        {
            Cooldown = 30;
            Name = new TokenValue("HC_LUCKJUICE", "Divination Distillate");
            PickupText = new TokenValue("HC_LUCKJUICE_PICKUP", "Heal both health and shield for a short period of time. Luk increased while active.");
            Description = new TokenValue("HC_LUCKJUICE_DESC",
                $"Heal both health and shields for {DistillateDuration} seconds. Effects stops at full health and full shields." +
                $" While under effect, your luck is greatly increased.");
            DistillateBuff = new BuffDef()
            {
                buffColor = Color.Lerp(Color.red, Color.yellow, 0.5f),
                canStack = false,
                isDebuff = false,
                name = "HC_LUCKJUICE_BUFF"
            };
            IsLunar = false;
            IsEnigmaCompat = false;
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/DivDistillate/LuckJuice.prefab";
            SpritePath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/DivDistillate/luckjuice.png";
            CustomBuff buff = new CustomBuff(DistillateBuff.name, DistillateBuff);
            ItemAPI.Add(buff);
        }

        public override bool Effect(EquipmentSlot slot)
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
            ownerBody.AddBuff(DistillateBuff.buffIndex);
            return true;
        }

        public class DistillateBehaviour : MonoBehaviour
        {
            private float lifeTime = DistillateDuration;
            private float nextHealIn = 0f;


            private float intervalFraction;
            public HealthComponent hc;
            public CharacterBody cb;

            private void Awake()
            {
                this.intervalFraction = Interval / DistillateDuration;
            }

            private void Start()
            {
                if (cb && cb.master)
                {
                    cb.master.luck += DistillateLuckIncrease;
                }
            }

            private void OnDestroy()
            {
                if (this.cb)
                {
                    this.cb.RemoveBuff(DistillateBuff.buffIndex);
                    if (cb.master)
                        cb.master.luck -= DistillateLuckIncrease;
                }

            }

            private void FixedUpdate()
            {
                this.nextHealIn -= Time.deltaTime;
                this.lifeTime -= Time.fixedDeltaTime;
                if (this.nextHealIn < 0 || this.lifeTime < 0)
                {
                    this.nextHealIn += Interval;
                    if (this.hc)
                    {
                        this.hc.RechargeShield(this.hc.fullShield * TotalShieldFraction * this.intervalFraction);
                        this.hc.Heal(this.hc.fullHealth * TotalHealthFraction * this.intervalFraction, default, true);
                    }
                }
                if (this.lifeTime <= 0f || (this.hc.fullHealth == this.hc.health && this.hc.shield == this.hc.fullShield && this.hc.godMode == false))
                {
                    Destroy(this);
                }
            }
        }

        public override void Hook()
        { }
    }
}
