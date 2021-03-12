using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace Diluvian
{
    internal static class SharedHooks
    {
        //Cache the fieldinfo for bloodshrines.
        private static readonly FieldInfo BloodShrineWaitingForRefresh;
        private static readonly FieldInfo BloodShrinePurchaseInteraction;
        static SharedHooks()
        {
            BloodShrineWaitingForRefresh = typeof(ShrineBloodBehavior).GetField("waitingForRefresh", BindingFlags.Instance | BindingFlags.NonPublic);
            BloodShrinePurchaseInteraction = typeof(ShrineBloodBehavior).GetField("purchaseInteraction", BindingFlags.Instance | BindingFlags.NonPublic);
            if (!DiluvianPlugin.ESOenabled)
            {
                CombatDirectorTierDefs = (CombatDirector.EliteTierDef[])typeof(RoR2.CombatDirector).GetFieldCached("eliteTiers").GetValue(null);
                //Init array because we now know the size.
                vanillaEliteMultipliers = new float[CombatDirectorTierDefs.Length];
            }
        }

        public static void ApplyOSPHook() => On.RoR2.CharacterBody.RecalculateStats += ChangeOSP;
        public static void UndoOSPHook() => On.RoR2.CharacterBody.RecalculateStats -= ChangeOSP;

        private static void ChangeOSP(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            self.SetPropertyValue("hasOneShotProtection", false);
        }

        private static bool BearsApplied = false;
        public static void ApplyBears()
        {
            if (!BearsApplied)
            {
                BearsApplied = true;
                IL.RoR2.HealthComponent.TakeDamage += UnluckyBears;
            }
        }
        public static void UndoBears(){
            if (BearsApplied)
            {
                BearsApplied = false;
                IL.RoR2.HealthComponent.TakeDamage -= UnluckyBears;
            }
}

        private static void UnluckyBears(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdflda(typeof(HealthComponent), "itemCounts"),
                x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "bear")
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0),//This is the luck value used in CheckRoll for toughertimes.
                x => x.MatchLdnull(),
                x => x.MatchCall("RoR2.Util", "CheckRoll")
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, -1f);//We replace it with -1. Because no mercy.
        }

        private static float HealthRegenMultiplier;
        private static float MonsterRegen;
        public static void ApplyRegenChanges(float regenModifier, float monsterRegen)
        {
            HealthRegenMultiplier = regenModifier;
            MonsterRegen = monsterRegen;
            IL.RoR2.CharacterBody.RecalculateStats += AdjustRegen;
        }
        public static void UndoRegenChanges() => IL.RoR2.CharacterBody.RecalculateStats -= AdjustRegen;

        private static void AdjustRegen(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int monsoonPlayerHelperCountIndex = 25;//This value doesn't need to be set, but I set it to keep track of what it's expected to be.
            c.GotoNext(//Since this is only to move the cursor, no explicit movetyp is given as I don't care.
                x => x.MatchLdcI4((int)ItemIndex.MonsoonPlayerHelper),//Find where the itemcount of the monsoon regen modifier is called
                x => x.MatchCallvirt<RoR2.Inventory>("GetItemCount"),
                x => x.MatchStloc(out monsoonPlayerHelperCountIndex)//Then use the `out` keyword to store the index of the local holding the count.
                );
            int regenMultiIndex = 44;//Again this is the expected index,  but I'll be overwriting it.
            c.GotoNext(MoveType.Before,//Movetype defaults to before, but I like to make it epxlicit.
                x => x.MatchStloc(out regenMultiIndex),//The regen multiplier local field is stored right before...
                x => x.MatchLdloc(monsoonPlayerHelperCountIndex)//the itemcount of the monsoonmultiplier item is checked.
                );
            c.Index += 2;//Since the previous cursor location will get skipped over by if statements, we need to move into a slightly weird place that is always called
            c.Emit(OpCodes.Ldarg_0);//emit the characterbody
            c.Emit(OpCodes.Ldloc, regenMultiIndex);//emit the local regen multiplier
            c.EmitDelegate<Func<CharacterBody, float, float>>(newRegenMultiplier);//emit a function taking a charbody and a float that returns a float.
            c.Emit(OpCodes.Stloc, regenMultiIndex);//store the result.
            int regenIndex = 36;//As usual with indexes, I overwrite this later with the 'out' keyword. I just put the expected value here
            c.GotoPrev(MoveType.Before,//We need to go back, but I couldn't find these instructions earlier becase I hadn't found the regenMultiIndex yet.
                x => x.MatchStloc(out regenIndex),//The base+level regen i stored with this instruction.
                x => x.MatchLdcR4(1),//And the regen multiplier is initialised.
                x => x.MatchStloc(regenMultiIndex)
                ); ;
            c.Index += 1;//go to right before the init of the regen multi
            c.Emit(OpCodes.Ldarg_0);//emit the characterbody
            c.Emit(OpCodes.Ldloc, regenIndex);//emit the value of the regen
            c.EmitDelegate<Func<CharacterBody, float, float>>(monsterFlatRegen);//emit a function taking a characterbody an a float that returns a float
            c.Emit(OpCodes.Stloc, regenIndex);//store the new regen.


            float newRegenMultiplier(CharacterBody self, float regenMulti)
            {
                if (self.isPlayerControlled)
                {
                    regenMulti += HealthRegenMultiplier+0.4f;//note that a + -b == a - b; 0.4 is to offset the monsoon degen.
                }
                return regenMulti;
            }

            float monsterFlatRegen(CharacterBody self, float regen)
            {
                //Check if this is a monster and if it's not been hit for 5 seconds.
                if (self.teamComponent.teamIndex == TeamIndex.Monster && self.outOfDanger && self.baseNameToken != "ARTIFACTSHELL_BODY_NAME" && self.baseNameToken != "TITANGOLD_BODY_NAME")
                {
                    regen += self.maxHealth * MonsterRegen;
                }
                return regen;
            }
        }


        private static bool CombatDirectorModifiersApplied = false;
        private static readonly CombatDirector.EliteTierDef[] CombatDirectorTierDefs;
        private static readonly float[] vanillaEliteMultipliers;

        public static void SetupCombatDirectorChanges(float multiplier)
        {
            if (CombatDirectorModifiersApplied)
                return;
            CombatDirectorModifiersApplied = true;
            if (DiluvianPlugin.ESOenabled)
            {
                ESOTweaking(multiplier, false);
            }
            else
            {
                for (int i = 0; i < CombatDirectorTierDefs.Length; i++)
                {
                    CombatDirector.EliteTierDef tierDef = CombatDirectorTierDefs[i];
                    vanillaEliteMultipliers[i] = tierDef.costMultiplier;
                    tierDef.costMultiplier *= multiplier;
                }
            }
        }

        public static void UndoCombatDirectorChanges()
        {
            if (!CombatDirectorModifiersApplied)
                return;
            CombatDirectorModifiersApplied = false;
            if (DiluvianPlugin.ESOenabled)
            {
                ESOTweaking(0, true);
            }
            else
            {
                for (int i = 0; i < CombatDirectorTierDefs.Length; i++)
                {
                    CombatDirector.EliteTierDef tierDef = CombatDirectorTierDefs[i];
                    tierDef.costMultiplier = vanillaEliteMultipliers[i];
                }
            }
        }

        private static readonly Dictionary<EliteIndex, float> EsoVals = new Dictionary<EliteIndex, float>();
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ESOTweaking(float multiplier, bool undo)
        {

            EliteSpawningOverhaul.EsoLib.Cards.ForEach(card =>
            {
                if (undo)
                {
                    card.costMultiplier = EsoVals[card.eliteType];
                }
                else
                {
                    EsoVals[card.eliteType] = card.costMultiplier;
                    card.costMultiplier *= multiplier;
                }
            });
        }


        private static bool BloodShrinesApplied = false;
        public static void ApplyBloodShrineRandom()
        {
            if (BloodShrinesApplied)
                return;
            BloodShrinesApplied = true;
            On.RoR2.ShrineBloodBehavior.FixedUpdate += BloodShrinePriceRandom;
        }

        public static void UndoBloodShrineRandom()
        {
            if (!BloodShrinesApplied)
                return;
            BloodShrinesApplied = false;
            On.RoR2.ShrineBloodBehavior.FixedUpdate -= BloodShrinePriceRandom;
        }

        private static void BloodShrinePriceRandom(On.RoR2.ShrineBloodBehavior.orig_FixedUpdate orig, RoR2.ShrineBloodBehavior self)
        {
            //This 'if' is essentially the same as the first line of the orig method(, save that they don't need to do reflection).
            if ((bool)BloodShrineWaitingForRefresh.GetValue(self))
            {
                orig(self);
                //reflect to get the purchaseinteractioncomponent. I then get a random number to change it's price. The price is hidden by the language modification.
                RoR2.PurchaseInteraction pi = ((RoR2.PurchaseInteraction)BloodShrinePurchaseInteraction.GetValue(self));
                if (pi)
                {
                    pi.Networkcost = RoR2.Run.instance.stageRng.RangeInt(50, 100);
                }
            }
        }

    }
}
