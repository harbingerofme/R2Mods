using ItemLib;
using RoR2;
using UnityEngine;
using R2API.Utils;

namespace HarbCrate.Items
{
    class DebuffReducer
    {
        public readonly static string Name = "Perfect Timepiece";
        public static float Scaling = 0.1f;

        public static CustomItem Build()
        {
            ItemDef mydef = new ItemDef
            {
                tier = ItemTier.Tier2,
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/AchievementIcons/texHuntressClearGameMonsoonIcon",
                nameToken = Name,
                pickupToken = "Reduces the duration of most negative effects by 10 percent*.",
                descriptionToken = "Most negative durations are reduced by 10% (+10% per stack)*."
            };

            return new CustomItem(mydef, null, null, null);
        }

        public static void Hooks()
        {
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.DotController.AddDot += DotController_AddDot;
            On.RoR2.SetStateOnHurt.SetFrozen += SetStateOnHurt_SetFrozen;
            On.RoR2.SetStateOnHurt.SetStun += SetStateOnHurt_SetStun;
        }

        private static void SetStateOnHurt_SetFrozen(On.RoR2.SetStateOnHurt.orig_SetFrozen orig, SetStateOnHurt self, float duration)
        {
            var cb = self.GetComponent<CharacterBody>();
            float newduration = GetNewtime(cb, duration);
            orig(self, newduration);
        }

        private static void SetStateOnHurt_SetStun(On.RoR2.SetStateOnHurt.orig_SetStun orig, SetStateOnHurt self, float duration)
        {
            var cb = self.GetComponent<CharacterBody>();
            float newduration = GetNewtime(cb, duration);
            orig(self, newduration);
        }

        private static void DotController_AddDot(On.RoR2.DotController.orig_AddDot orig, DotController self, GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier)
        {
            var cb = self.GetPropertyValue<CharacterBody>("victimBody");
            float newduration = GetNewtime(cb, duration);
            orig(self, attackerObject, newduration, dotIndex, damageMultiplier);
        }

        private static void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            var newDuration = duration;
            var buffDef = BuffCatalog.GetBuffDef(buffType);
            if (buffDef.isDebuff)
            {
                newDuration = GetNewtime(self, duration);
            }
            orig(self, buffType, newDuration);
        }

        private static float GetNewtime(CharacterBody cb,float oldDuration)
        {
            if(cb && cb.inventory)
            {
                int num = cb.inventory.GetItemCount((ItemIndex)ItemLib.ItemLib.GetItemId(Name));
                if (num > 0)
                {
                    var newDuration = oldDuration * GetMulti(num);
                    //Debug.LogWarning("Reduced " + oldDuration + " to " + newDuration);
                    return newDuration;
                }
            }
            return oldDuration;
        }

        private static float GetMulti(int stackSize)
        {
            return 1 / (1 + Scaling * stackSize);
        }
    }




}
