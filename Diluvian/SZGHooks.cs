using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;

namespace Diluvian
{
    static class SZGHooks
    {
        static SZGHooks()
        {
            
        }

        internal static void StartAMeteorTimer(SceneDirector _)
        {
            var timer = TeleporterInteraction.instance?.gameObject.AddComponent<SimpleMeteorTimer>();
            if(timer)
                timer.startsIn = 300 * Mathf.Pow(0.95f, Run.instance.loopClearCount);
        }

        internal static void RemoveCleansingPools(SceneDirector _, DirectorCardCategorySelection cardCategorySelection)
        {
            cardCategorySelection.RemoveCardsThatFailFilter(card =>
            {
                return card.spawnCard.name != "iscShrineCleanse";
            });
        }

        internal static void LunarsUnConsentional(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdfld<ItemDef>("tier"),
                x => x.MatchLdcI4(3)
                );
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4, -2);
            c.GotoNext(MoveType.After,
                x => x.MatchLdfld<EquipmentDef>("isLunar")
                );
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }

        internal static void AddLunarsToItemPools()
        {
            Run.instance.availableNormalEquipmentDropList.AddRange(Run.instance.availableLunarEquipmentDropList);
            Run.instance.availableTier2DropList.AddRange(Run.instance.availableLunarDropList);
        }

        internal static void GiveScavsLunarsAndBossItems(On.RoR2.ScavengerItemGranter.orig_Start orig, RoR2.ScavengerItemGranter self)
        {
            orig(self);
            Inventory inventory = self.GetComponent<Inventory>();
            if( inventory.GetTotalItemCountOfTier(ItemTier.Lunar) == 0)
            {
                var list = Run.instance.availableLunarDropList.Where(new System.Func<PickupIndex, bool>(PickupIsNonBlacklistedItem)).ToList();
                var randomIndex = Random.Range(0, list.Count);
                inventory.GiveItem(PickupCatalog.GetPickupDef(list[randomIndex]).itemIndex,2);
            }

            if(inventory.GetTotalItemCountOfTier(ItemTier.Boss) == 0)
            {
                var list = PickupCatalog.allPickups.Where(def => def.isBoss).ToList();
                var randomIndex = Random.Range(0, list.Count);
                inventory.GiveItem(list[randomIndex].itemIndex, 1);
            }


            bool PickupIsNonBlacklistedItem(PickupIndex pickupIndex)
            {
                PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef != null ? pickupDef.itemIndex : ItemIndex.None);
                if (itemDef == null)
                    return false;
                return itemDef.DoesNotContainTag(ItemTag.AIBlacklist);
            }
        }



        private class SimpleMeteorTimer : MonoBehaviour
        {
            public float startsIn;

            Transform targetTransform;

            void Start()
            {
                if (TeleporterInteraction.instance)
                {
                    targetTransform = TeleporterInteraction.instance.transform;
                }
                else
                {
                    Destroy(this);
                }

            }

            void FixedUpdate()
            {
                startsIn -= Time.fixedDeltaTime;
                if (startsIn < 0)
                {
                    var storm = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"),targetTransform.position,targetTransform.rotation).GetComponent<MeteorStormController>();
                    storm.ownerDamage = 50;
                    storm.waveCount = int.MaxValue;
                    Destroy(this);
                }
            }
        }
    }

    
}
