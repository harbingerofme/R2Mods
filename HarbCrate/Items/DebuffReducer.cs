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
        //Todo: Make it hook into shields and give some shield boost. 


        public static float GetMulti(int stackSize)
        {
            return 1 / (1 + Scaling * stackSize);
        }
    }

}
