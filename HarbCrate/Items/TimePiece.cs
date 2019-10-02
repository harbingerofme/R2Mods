using ItemLib;
using RoR2;
using UnityEngine;
using MonoMod.Cil;
using R2API.Utils;
using System;
using Mono.Cecil.Cil;

namespace HarbCrate.Items
{
    class TimePiece
    {
        public readonly static string Name = "Perfect Timepiece";
        public static float Scaling = 2.5f;//Scaling in percent.
        public static float ShieldFrac = 0.08f;//shield fraction in actual numbers.

        public static CustomItem Build()
        {
            ItemDef mydef = new ItemDef
            {
                tier = ItemTier.Tier2,
                pickupModelPath = "Prefabs/PickupModels/PickupMystery",
                pickupIconPath = "Textures/AchievementIcons/texHuntressClearGameMonsoonIcon",
                nameToken = Name,
                pickupToken = "Get some of your max health as shield. Reduces the duration of most negative effects.",
                descriptionToken = "<style=cIsHealing>Shields</style> are increased by <style=cIsHealing>" + ShieldFrac*100+ "%</style> <style=cStack>(+" + ShieldFrac * 100 + "%)</style> of your <style=cIsHealing>maximum health</style>. Most <style=cIsUtility>negative durations</style> are reduced by <style=cIsUtility>" + Scaling+ "%</style> <style=cStack>(+" + Scaling+ "% per stack)</style>*."
            };

            return new CustomItem(mydef, null, null, null);
        }
        //Todo: Make it hook into shields and give some shield boost. 

        public static void Hooks(ItemIndex id)
        {
            IL.RoR2.CharacterBody.RecalculateStats += delegate(ILContext il)
            {
                ILCursor c = new ILCursor(il);
                int shieldsLoc = 33;
                c.GotoNext(
                    MoveType.Before,
                    x => x.MatchLdloc(out shieldsLoc),
                    x => x.MatchCallvirt(typeof(CharacterBody).GetMethodCached("set_maxShield"))
                    );
                c.Emit(OpCodes.Ldloc, shieldsLoc);
                c.EmitDelegate<Func<CharacterBody, float, float>>((self, shields) =>
                {
                    if (self.inventory)
                    {
                        int num = self.inventory.GetItemCount(id);
                        shields += self.maxHealth * ShieldFrac * num;
                    }
                    return shields;
                });
                c.Emit(OpCodes.Stloc, shieldsLoc);
                c.Emit(OpCodes.Ldarg_0);
            };
        }
    }

}
