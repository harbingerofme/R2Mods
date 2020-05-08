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
            Cooldown = 30;
            Name = new TokenValue("HC_LUCKJUICE", "Divination Distillate");
            PickupText = new TokenValue("HC_LUCKJUICE_PICKUP", "Heal both health and shield for a short period of time. Luk increased while active.");
            Description = new TokenValue("HC_LUCKJUICE_DESC",
                $"Heal both health and shields for {DistillateDuration} seconds. Effects stops at full health and full shields." +
                $" While under effect, your luck is greatly increased.");
            var cbuff = new CustomBuff(
                name: "HC_LUCKJUICE_BUFF",
                iconPath: HarbCratePlugin.assetPrefix + "Assets/HarbCrate/DivDistillate/texBuffLuck.png",
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
            var oneCubed = new Vector3(1, 1, 1);
            DisplayRules = new ItemDisplayRuleDict(new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3 (0.117f, 0,0),
                localAngles = new Vector3(-0.0001368911f, 90.00013f, 180),
                localScale = oneCubed * 0.3f
            });
            DisplayRules.Add("mdlCroco", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-1.3f, 1.31f, 0.57f),
                localAngles = new Vector3(0.4302638f, 112.6148f, 178.8079f),
                localScale = oneCubed*2
            });
            DisplayRules.Add("mdlEngi", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.175f, 0.063f, -0.07f),
                localAngles = new Vector3(0.1981713f, 89.21727f, 182.4698f),
                localScale = oneCubed * 0.3f
            });
            DisplayRules.Add("mdlHuntress", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(0.129f, 0.043f, 0.105f),
                localAngles = new Vector3(6.425195f,100.7368f, 152.0132f),
                localScale = oneCubed * 0.25f
            });
            DisplayRules.Add("mdlLoader", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.118f, 0.112f, 0.092f),
                localAngles = new Vector3(352.083f, 112.7895f, 178.7964f),
                localScale = oneCubed * 0.3f
            });
            DisplayRules.Add("mdlMage", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.1f, 0.04f, 0.107f),
                localAngles = new Vector3(354.2113f, 105f, 180f),
                localScale = oneCubed * 0.25f
            });
            DisplayRules.Add("mdlMerc", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighR",
                localPos = new Vector3(-0.126f,-0.021f, 0),
                localAngles = new Vector3(356.9035f,89.99998f,180),
                localScale = oneCubed * 0.3f
            });
            DisplayRules.Add("mdlScav", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "Chest",
                localPos = new Vector3(15.04f, 3.46f, 6.18f),
                localAngles = new Vector3(335.505f, 189.7875f, 142.3523f),
                localScale = oneCubed * 3f
            });
            DisplayRules.Add("mdlToolbot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(-0.09f, 0.15f, 0.88f),
                localAngles = new Vector3(0.8589115f, 1.487932f, 240.0112f),
                localScale = oneCubed * 1.5f
            });
            DisplayRules["mdlHAND"] = DisplayRules["mdlToolbot"];
            DisplayRules.Add("mdlTreebot", new ItemDisplayRule
            {
                followerPrefab = Prefab,
                ruleType = Rule,
                childName = "ThighL",
                localPos = new Vector3(-0.702f, -0.046f, -0.017f),
                localAngles = new Vector3(0, 261.7092f, 0),
                localScale = oneCubed * 0.4f
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
