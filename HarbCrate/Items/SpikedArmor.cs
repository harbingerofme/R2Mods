using ItemLib;
using RoR2;
using UnityEngine;

namespace HarbCrate.Items
{
    class SpikedArmor
    {
        public static readonly string Name = "Synthetic Thorns";
        public static readonly float Stacking = 0.05f;
        public static readonly float procStacking = 1f;

        public static CustomItem Build()
        {
            ItemDef mydef = new ItemDef
            {
                tier = ItemTier.Tier1,
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/AchievementIcons/texMageClearGameMonsoonIcon",
                nameToken = Name,
                pickupToken = "Reflect damage back to attackers.",
                descriptionToken = "Reflect 1% + <style=cStack>(+" + Stacking * 100 + "%)</style> of damage taken back to attackers. Proc coefficient is " + procStacking + " <style=cStack>(+" + procStacking / 100 + ")</style> "
            };

            return new CustomItem(mydef, null, null, null);
        }

        public static void Hooks(ItemIndex Thorns)
        {
            //Look Ma! No hooks! Really, I'm just adding an eventhandler.
            GlobalEventManager.onServerDamageDealt += delegate (DamageReport report)
            {
                var victim = report.victimBody;
                if (victim && victim.inventory)
                {
                    if (report.attackerBody)
                    {

                            if (report.attackerTeamIndex != report.victimTeamIndex)
                        {
                            int count = victim.inventory.GetItemCount(Thorns);
                            if (count > 0)
                            {
                                if (report.attackerBody.inventory && report.attackerBody.inventory.GetItemCount(Thorns) > 0)
                                {
                                    Debug.LogWarning("Both attacker and defender will reflect damage, aborting!");
                                    return;
                                }
                                var myInfo = new DamageInfo()
                                {
                                    damage = report.damageDealt * count * Stacking,
                                    crit = false,
                                    attacker = victim.gameObject,
                                    position = report.attacker.transform.position,
                                    procChainMask = report.damageInfo.procChainMask,
                                    procCoefficient = procStacking * count,
                                    force = Vector3.zero,
                                    damageType = DamageType.Generic,
                                    damageColorIndex = DamageColorIndex.Bleed
                                };
                                var aHC = report.attackerBody.GetComponent<HealthComponent>();
                                if (aHC)
                                {
                                    aHC.TakeDamage(myInfo);
                                    GlobalEventManager.instance.OnHitEnemy(myInfo, report.attacker);
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}