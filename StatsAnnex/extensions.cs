using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatsAnnex
{
    internal static class Extensions
    {
        public static int GetCount(this ItemIndex itemIndex, CharacterBody characterBody)
        {
            return characterBody.inventory.GetItemCount(itemIndex);
        }
    }
}
