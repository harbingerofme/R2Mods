using R2API;
using RoR2;
using System;
using UnityEngine;


namespace HarbCrate.Equipment
{
    [R2API.Utils.R2APISubmoduleDependency(nameof(BuffAPI))]
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
            const string cShield = "<style=cIsHealing>shield</style>";
            const string cHealth = "<style=cIsHealing>health</style>";

            Cooldown = 30;
            Name = new TokenValue("HC_LUCKJUICE", "Divination Distillate");
            PickupText = new TokenValue("HC_LUCKJUICE_PICKUP", 
                $"<style=cIsHealing>Heal</style> both {cHealth} and {cShield} for a short period of time. <style=cIsUtility>Luck</style> increased while active.");
            Description = new TokenValue("HC_LUCKJUICE_DESC",
                $"Heal both {cHealth} and {cShield} for {DistillateDuration} seconds. Effects stops at full {cHealth} and full {cShield}." +
                $" While under effect, your <style=cIsUtility>luck</style> is greatly increased.");
            Lore = new TokenValue("HC_LUCKJUICE_LORE",
                "Knowledge is fermented in pain and loss\nDistilled with reflection\nTo quench the thirst of those\nWho dream of enlightenment\n\nOpportunity is ripened in risk and rain\nRefined in a single moment\nTo fuel the hunger of those\nWho crave for avarice\n\n<i>Divination Distillate's model, effect and lore are deratives of Grinding Gear Games' version.</i>");
            var cbuff = new CustomBuff(
                name: "HC_LUCKJUICE_BUFF",
                iconPath: HarbCratePlugin.assetPrefix + "Assets/HarbCrate/DivDistillate/texBuffLuck",
                buffColor: Color.Lerp(Color.red, Color.yellow, 0.5f),
                canStack: false,
                isDebuff: false
                );
            BuffAPI.Add(cbuff);
            DistillateBuff = cbuff.BuffDef;
            IsLunar = false;
            IsEnigmaCompat = false;
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/DivDistillate/LuckJuice.prefab";
            SpritePath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/DivDistillate/luckjuice.png";
            DefaultScale *= 21;
            SetupDisplayRules();
        }

        

        public override bool Effect(EquipmentSlot slot)
        {
            var ownerBody = slot.GetComponent<CharacterBody>();
            if (!ownerBody)
                return false;
            var ownerHealthComponent = ownerBody.GetComponent<HealthComponent>();
            if (!ownerHealthComponent || (ownerHealthComponent.health == ownerHealthComponent.fullHealth && ownerHealthComponent.shield == ownerHealthComponent.fullShield && ownerHealthComponent.godMode == false))
                return false;
            var distillateComp = ownerBody.gameObject.GetComponent<DistillateBehaviour>();
            if (!distillateComp)
            {
                distillateComp = ownerBody.gameObject.AddComponent<DistillateBehaviour>();
                distillateComp.hc = ownerHealthComponent;
                distillateComp.cb = ownerBody;
            }
            distillateComp.lifeTime = DistillateDuration;
            ownerBody.AddBuff(DistillateBuff.buffIndex);
            return true;
        }

        public class DistillateBehaviour : MonoBehaviour
        {
            public float lifeTime = DistillateDuration;
            private float nextHealIn = 0f;


            private float intervalFraction;
            public HealthComponent hc;
            public CharacterBody cb;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
            private void Awake()
            {
                this.intervalFraction = Interval / DistillateDuration;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
            private void Start()
            {
                if (cb && cb.master)
                {
                    cb.master.luck += DistillateLuckIncrease;
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
            private void OnDestroy()
            {
                if (this.cb)
                {
                    this.cb.RemoveBuff(DistillateBuff.buffIndex);
                    if (cb.master)
                        cb.master.luck -= DistillateLuckIncrease;
                }

            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
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
