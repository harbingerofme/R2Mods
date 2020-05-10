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

            SetupDisplayRules();
        }

        private void SetupDisplayRules()
        {
            var Prefab = Resources.Load<GameObject>(AssetPath);
            var Rule = ItemDisplayRuleType.ParentedPrefab;
            var defaultScale = new Vector3(21,21, 21);
            DisplayRules = new ItemDisplayRuleDict(new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3 (0.2f, 0.15f,0),
                localAngles = new Vector3(-0.0001368911f, 90.00013f, 180),
                localScale = defaultScale * 0.3f
            });
            DisplayRules.Add("mdlCroco", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-1.3f, 1.31f, 0.57f),
                localAngles = new Vector3(0.4302638f, 112.6148f, 178.8079f),
                localScale = defaultScale*2
            });
            DisplayRules.Add("mdlEngi", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.17f, 0.18f, -0.07f),
                localAngles = new Vector3(0.1981713f, 89.21727f, 182.4698f),
                localScale = defaultScale * 0.3f
            });
            DisplayRules.Add("mdlHuntress", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(0.129f, 0.043f, 0.105f),
                localAngles = new Vector3(0f,120.74f, 186.3f),
                localScale = defaultScale * 0.25f
            });
            DisplayRules.Add("mdlLoader", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.18f, 0.21f, 0.092f),
                localAngles = new Vector3(352.083f, 112.7895f, 178.7964f),
                localScale = defaultScale * 0.3f
            });
            DisplayRules.Add("mdlMage", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.1f, 0.24f, 0.107f),
                localAngles = new Vector3(354.2113f, 105f, 180f),
                localScale = defaultScale * 0.25f
            });
            DisplayRules.Add("mdlMerc", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.16f,-0.7f, 0),
                localAngles = new Vector3(356.9035f,89.99998f,180),
                localScale = defaultScale * 0.3f
            });
            DisplayRules.Add("mdlScav", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Chest",
                localPos = new Vector3(14.04f, 2.7f, 6.18f),
                localAngles = new Vector3(35f, 280.3f, 358.8f),
                localScale = defaultScale * 3f
            });
            DisplayRules.Add("mdlToolbot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(-0.09f, 0.15f, 0.88f),
                localAngles = new Vector3(0.8589115f, 1.487932f, 240.0112f),
                localScale = defaultScale * 1.5f
            });
            DisplayRules["mdlHAND"] = DisplayRules["mdlToolbot"];
            DisplayRules.Add("mdlTreebot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "PlatformBase",
                localPos = new Vector3(-0.702f, -0.046f, -0.017f),
                localAngles = new Vector3(0, 261.7092f, 0),
                localScale = defaultScale * 0.4f
            });
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
