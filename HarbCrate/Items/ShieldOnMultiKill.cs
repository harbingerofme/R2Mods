using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class ShieldOnMultiKill : Item
    {

        internal const float ShieldPerMK = 10f;
        internal const int MultikillCountNeeded = 3;
        internal const int MultKillsNeededForMaxValue = 15;


        public ShieldOnMultiKill() : base()
        {
            Tier = ItemTier.Tier2;
            Name = new TokenValue("HC_MAXSHIELDONMULTIKILL", "Obsidian Bouche");
            Description = new TokenValue(
                "HC_MAXSHIELDONMULTIKILL_DESC",
                $" Gain {ShieldPerMK} additional maximum <style=cIsHealing>shield</style> on multikill."
                + $" Maximum <style=cIsHealing>shield</style> tops of at an aditional {MultKillsNeededForMaxValue * ShieldPerMK}<style=cStack>(+{MultKillsNeededForMaxValue * ShieldPerMK} per stack)</style>.");
            PickupText = new TokenValue("HC_MAXSHIELDONMULTIKILL_PICKUP", "Gain maximum <style=cIsHealing>shield</style> on <style=cIsDamage>multikill</style>.");
            Lore = new TokenValue("HC_MAXSHIELDONMULTIKILL_LORE",
                "Obsidian is an interesting material.\nUnlike what a popular games would have you believe, obsidian is relatively light and brittle.\nIt's a terrible material for shields.\nYou'll cut yourself on the edges and it will shatter on the slightest impact.\n\nHowever, a material made from the inside of a volcano has a certain charm to it.\nTo not acknowledge its mystical properties would be unwise.\nTherefore, we've build this shield.\nIt yearns to be back inside the volcano, to be inside the carnage, the heat.\n\nWill you fuel it, or break and shatter away?"
                );
            AssetPath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/Obsidian_Shield/GhorsWay.prefab";
            SpritePath = HarbCratePlugin.assetPrefix + "Assets/HarbCrate/Obsidian_Shield/Bouche.png";
            Tags = new ItemTag[2]
            {
                ItemTag.Utility,
                ItemTag.OnKillEffect
            };

            SetupDisplayRules();
            HarbCratePlugin.Started += HarbCratePlugin_Started;
        }

        private void SetupDisplayRules()
        {
            var Prefab = Resources.Load<GameObject>(AssetPath);
            DisplayRules = new ItemDisplayRuleDict(
                new ItemDisplayRule()
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Prefab,
                    childName = "HandR",
                    localPos = new Vector3(0f, 0f, 0.2f),
                    localAngles = new Vector3(210, 0, 0f),
                    localScale = new Vector3(25, 25, 25)
                }
            );
            DisplayRules.Add("mdlHuntress", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "Chest",
                localPos = new Vector3(0f, 0f, -0.3f),
                localAngles = new Vector3(330, 0, 180f),
                localScale = new Vector3(25, 25, 25)
            });

            DisplayRules.Add("mdlMerc", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "HandR",
                localPos = new Vector3(0f, 0f, 0.25f),
                localAngles = new Vector3(210, 0, 0f),
                localScale = new Vector3(25, 25, 25)
            });
            DisplayRules.Add("mdlToolbot", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "LowerArmR",
                localPos = new Vector3(-1f, 2.4f, 1.6f),
                localAngles = new Vector3(8f, 180f, 67),
                localScale = new Vector3(200, 200, 200)//wtf MULT
            });
            DisplayRules.Add("mdlTreebot", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "HandL",
                localPos = new Vector3(-0.48f, 1.31f, 0.55f),
                localAngles = new Vector3(338.5f, 228.5f, 280),
                localScale = new Vector3(50, 50, 50)
            });
            DisplayRules.Add("mdlLoader", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "MechLowerArmL",
                localPos = new Vector3(0.07f, 0.31f, -0.25f),
                localAngles = new Vector3(23.14f, 0, 0),
                localScale = new Vector3(25, 25, 25)
            });
            DisplayRules.Add("mdlMage", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "LowerArmR",
                localPos = new Vector3(-0.14f, 0.2f, 0.2f),
                localAngles = new Vector3(6, 160, 92),
                localScale = new Vector3(25, 25, 25)
            });
            DisplayRules.Add("mdlCroco", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "LowerArmR",
                localPos = new Vector3(2.5f, -0.59f, 1.67f),
                localAngles = new Vector3(0, 255, 0),
                localScale = new Vector3(200, 200, 200)//again, Acrid, wtf
            });
            DisplayRules.Add("mdlEngiTurret", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "LegBar3",
                localPos = new Vector3(0f, 0.18f, 0.6f),
                localAngles = new Vector3(30, 180, 0),
                localScale = new Vector3(75, 75, 75)
            });
            DisplayRules["mdlEngiWalkerTurret"] = DisplayRules["mdlEngiTurret"];//The bases are the same.... hopefully?
            DisplayRules.Add("mdlScav", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "Head",
                localPos = new Vector3(0, -10, -11),
                localAngles = new Vector3(0, 0, 0),
                localScale = new Vector3(620, 620, 620)
            });
            DisplayRules.Add("mdlHAND", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "UpperArmL",
                localPos = new Vector3(-0.97f, 0.53f, 0.37f),
                localAngles = new Vector3(49f, 92f, 0),
                localScale = new Vector3(135, 135, 135)//Guess all robots are tiny?
            });
            DisplayRules.Add("mldBandit", new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Prefab,
                childName = "UpperArmR",
                localPos = new Vector3(-1f, 2.4f, 1.6f),
                localAngles = new Vector3(0f, 0f, 0),
                localScale = new Vector3(25, 25, 25)
            });
        }

        private void HarbCratePlugin_Started(object sender, EventArgs e)
        {
            //helperIndex = ((Item)HarbCratePlugin.AllPickups[nameof(ShieldInfusionHelper)]).Definition.itemIndex;
            NetworkedSI.ShieldOnMultiKillIndex = Definition.itemIndex;
        }

        public override void Hook()
        {
            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            IL.RoR2.CharacterBody.RecalculateStats += AddFlatShields;
            On.RoR2.Inventory.ResetItem += Inventory_ResetItem;
            On.RoR2.CharacterBody.AddMultiKill += CharacterBodyOnAddMultiKill;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();
            foreach(CharacterBody cb in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                cb.gameObject.AddComponent<NetworkedSI>();
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self.inventory.HasItem(this, out int itemCount))
            {
                NetworkedSI si = self.GetComponent<NetworkedSI>();
                if (!si)
                {
                    Log("Missing si component!");
                    return;
                }
                si.SetItemCount(itemCount);
                self.OnLevelChanged();//hack to avoid reflecting for a bool.
            }
        }

        private void AddFlatShields(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int shieldsLoc = 33;
            c.GotoNext(
                MoveType.Before,
                x => x.MatchLdloc(out shieldsLoc),
                x => x.MatchCallvirt<CharacterBody>("set_maxShield")
            );
            c.Emit(OpCodes.Ldloc, shieldsLoc);
            c.EmitDelegate<Func<CharacterBody, float, float>>((self, shields) =>
            {
                if (!self.inventory)
                    return shields;
                var si = self.GetComponent<NetworkedSI>();
                if (si)
                    shields +=  si.multikills * ShieldPerMK;
                return shields;
            });
            c.Emit(OpCodes.Stloc, shieldsLoc);
            c.Emit(OpCodes.Ldarg_0);
        }

        private void Inventory_ResetItem(On.RoR2.Inventory.orig_ResetItem orig, Inventory inventory, ItemIndex itemIndex)
        {
            if (itemIndex == Definition.itemIndex)
            {
                NetworkedSI si = inventory.GetComponentInParent<CharacterBody>()?.GetComponent<NetworkedSI>();
                if (si != null)
                {
                    si.SetItemCount(0);
                }
            }
        }

        private void CharacterBodyOnAddMultiKill(On.RoR2.CharacterBody.orig_AddMultiKill orig, CharacterBody self, int kills)
        {
            orig(self, kills);
            if (self.inventory && self.multiKillCount % MultikillCountNeeded == 0 && self.inventory.GetItemCount(Definition.itemIndex) > 0)
            {
                var si = self.GetComponent<NetworkedSI>();
                if (!si)
                    return;
                si.AddMultiKill();
                self.OnLevelChanged();
            }
        }
    }

    
    public class NetworkedSI : NetworkBehaviour
    {
        [SyncVar]
        public int multikills = 0;

        public int cachedItemCount = 0;

        public static ItemIndex ShieldOnMultiKillIndex;

        public void AddMultiKill()
        {
            multikills++;
            VerifyIntegrity();
        }

        public void SetItemCount(int newCount)
        {
            cachedItemCount = newCount;
            VerifyIntegrity();
        }

        private void VerifyIntegrity()
        {
            int max = cachedItemCount * ShieldOnMultiKill.MultKillsNeededForMaxValue;
            multikills = Math.Min(Math.Max(0, multikills), max);
        }
    }
}
