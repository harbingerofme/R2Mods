using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EliteDef = RoR2.CombatDirector.EliteTierDef;

namespace Diluvian
{

    //[R2APISubmoduleDependency("DifficultyAPI", "ResourcesAPI", "AssetPlus")]

    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.jarlyk.eso", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Diluvian : BaseUnityPlugin
    {
        public const string
            NAME = "Diluvian",
            GUID = "com.harbingerofme." + NAME,
            VERSION = "1.0.0";

        private readonly Color DiluvianColor;
        private readonly DifficultyDef DiluvianDef;
        private DifficultyIndex DelugeIndex;
        private readonly float DelugeEliteModifier = 0.8f;


        private const string assetPrefix = "@HarbDiluvian";
        private const string assetString = assetPrefix + ":Assets/Diluvian/DiluvianIcon.png";

        private Dictionary<string, string> DefaultLanguage;
        private bool ESOenabled = false;
        private bool HooksApplied = false;
        private float[] vanillaEliteMultipliers;
        private EliteDef[] CombatDirectorTierDefs;

        private Diluvian()
        {
            DiluvianColor = new Color(0.61f, 0.07f, 0.93f);
            DiluvianDef = new DifficultyDef(
                            3.5f,
                            "DIFFICULTY_DILUVIAN_NAME",
                            assetString,
                            "DIFFICULTY_DILUVIAN_DESCRIPTION",
                            DiluvianColor
                            );
            DefaultLanguage = new Dictionary<string, string>();
        }

        public void Awake()
        {

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Diluvian.diluvian"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new R2API.AssetBundleResourcesProvider(assetPrefix, bundle);
                R2API.ResourcesAPI.AddProvider(provider);
            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.jarlyk.eso"))
            {
                ESOenabled = true;
                Logger.LogWarning("ESO detected: Delegating Elite modifications to them. Future support planned.");
            }

            Logger.LogWarning("This is a prerelease!");

            CombatDirectorTierDefs = (EliteDef[])typeof(CombatDirector).GetFieldCached("eliteTiers").GetValue(null);
            vanillaEliteMultipliers = new float[CombatDirectorTierDefs.Length];

            DelugeIndex = R2API.DifficultyAPI.AddDifficulty(DiluvianDef);

            R2API.AssetPlus.Languages.AddToken("DIFFICULTY_DILUVIAN_NAME", "Diluvian");
            string description = "For those found wanting. <style=cDeath>N'Kuhana</style> watches with interest.<style=cStack>\n";
            description = string.Join("\n",
                description,
                $">Difficulty Scaling: +{DiluvianDef.scalingValue * 50 - 100}%",
                ">Player Health Regeneration: -50%",
                ">Monster Health Regeneration: +1% of MaxHP per second",
                ">Oneshot Protection: Also applies to monsters",
                ">Oneshot Protection: Hits do a maximum of 99%",
                ">Teleporter: Enemies don't stop after charge completion",
                $">Elites: {(1 - DelugeEliteModifier) * 100}% cheaper.",
                ">Shrine of Blood: Always 99%."
                );
            description += "</style>";
            R2API.AssetPlus.Languages.AddToken("DIFFICULTY_DILUVIAN_DESCRIPTION", description);



            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;

        }


        private void ReplaceString(string token, string newText)
        {
            DefaultLanguage[token] = Language.GetString(token);
            R2API.AssetPlus.Languages.AddToken(token, newText);
        }


        private void Run_onRunStartGlobal(Run run)
        {
            if (run.selectedDifficulty == DelugeIndex && HooksApplied == false)
            {
                ChatMessage.SendColored("A storm is brewing...", DiluvianColor);
                HooksApplied = true;
                IL.RoR2.HealthComponent.TakeDamage += ChangeOSP;
                IL.RoR2.CharacterBody.RecalculateStats += AdjustRegen;
                IL.RoR2.TeleporterInteraction.StateFixedUpdate += NoSafetyAfterFinishCharging;
                TeleporterInteraction.onTeleporterFinishGlobal += MakeSureBonusDirectorDiesOnStageFinish;
                if (!ESOenabled)
                {
                    for (int i = 0; i < CombatDirectorTierDefs.Length; i++)
                    {
                        EliteDef tierDef = CombatDirectorTierDefs[i];
                        vanillaEliteMultipliers[i] = tierDef.costMultiplier;
                        tierDef.costMultiplier *= DelugeEliteModifier;
                    }
                }
                On.RoR2.ShrineBloodBehavior.FixedUpdate += BloodShrinesCost100Percent;

                escapeCounter = 0;
                ReplaceInteractibles();
                ReplaceObjectives();
                ReplacePause();
                ReplaceStats();
                RoR2.Console.instance.SubmitCmd(null, "language_reload");
            }
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            if (HooksApplied)
            {
                IL.RoR2.HealthComponent.TakeDamage -= ChangeOSP;
                IL.RoR2.CharacterBody.RecalculateStats -= AdjustRegen;
                IL.RoR2.TeleporterInteraction.StateFixedUpdate -= NoSafetyAfterFinishCharging;
                TeleporterInteraction.onTeleporterFinishGlobal -= MakeSureBonusDirectorDiesOnStageFinish;
                if (!ESOenabled)
                {
                    for (int i = 0; i < CombatDirectorTierDefs.Length; i++)
                    {
                        EliteDef tierDef = CombatDirectorTierDefs[i];
                        tierDef.costMultiplier = vanillaEliteMultipliers[i];
                    }
                }
                On.RoR2.ShrineBloodBehavior.FixedUpdate -= BloodShrinesCost100Percent;
                DefaultLanguage.ForEachTry((pair) =>
                {
                    //Debug.Log($"Restoring {pair.Key}:{pair.Value} from {Language.GetString(pair.Key)}");
                    R2API.AssetPlus.Languages.AddToken(pair.Key, pair.Value);
                });
                RoR2.Console.instance.SubmitCmd(null, "language_reload");
                HooksApplied = false;
            }
        }

        private void ReplaceInteractibles()
        {
            ReplaceString("MSOBELISK_CONTEXT", "Escape the madness");
            ReplaceString("MSOBELISK_CONTEXT_CONFIRMATION", "Take the cowards way out");
            ReplaceString("SHRINE_BLOOD_USE_MESSAGE_2P", "<style=cDeath>N'Kuhana</style>: This pleases me. <style=cShrine>({1})</color>");
            ReplaceString("SHRINE_BLOOD_USE_MESSAGE", "<style=cDeath>N'Kuhana</style>: {0} has paid their respects. Will you do the same? <style=cShrine>({1})</color>");
            ReplaceString("SHRINE_HEALING_USE_MESSAGE_2P", "<style=cDeath>N'Kuhana</style>: Bask in my embrace.");
            ReplaceString("SHRINE_HEALING_USE_MESSAGE", "<style=cDeath>N'Kuhana</style>: Bask in my embrace.");
            ReplaceString("SHRINE_BOSS_BEGIN_TRIAL", "<style=cShrine>Show me your courage.</style>");
            ReplaceString("SHRINE_BOSS_END_TRIAL", "<style=cShrine>Your effort entertains me.</style>");
            ReplaceString("PORTAL_MYSTERYSPACE_CONTEXT", "Hide in another realm.");
        }

        private void ReplaceObjectives()
        {
            ReplaceString("OBJECTIVE_FIND_TELEPORTER", "Flee");
            ReplaceString("OBJECTIVE_DEFEAT_BOSS", "Defeat the <style=cDeath>Anchor</style>");
        }
        private void ReplacePause()
        {
            ReplaceString("PAUSE_RESUME", "Entertain me");
            ReplaceString("PAUSE_SETTINGS", "Change your perspective");
            ReplaceString("PAUSE_QUIT_TO_MENU", "Give up");
            ReplaceString("PAUSE_QUIT_TO_DESKTOP", "Don't come back");
        }
        private void ReplaceStats()
        {
            ReplaceString("STAT_KILLER_NAME_FORMAT", "Released by: <color=#FFFF7F>{0}</color>");
            ReplaceString("STAT_POINTS_FORMAT", "");
            ReplaceString("STAT_TOTAL", "");
            ReplaceString("STAT_CONTINUE", "Try again");

            ReplaceString("STATNAME_TOTALTIMEALIVE", "Wasted time");
            ReplaceString("STATNAME_TOTALDEATHS", "Times Blessed");
            ReplaceString("STATNAME_HIGHESTLEVEL", "Strength Acquired");
            ReplaceString("STATNAME_TOTALGOLDCOLLECTED", "Greed");
            ReplaceString("STATNAME_TOTALDISTANCETRAVELED", "Ground laid to waste");
            ReplaceString("STATNAME_TOTALITEMSCOLLECTED", "Trash cleaned up");
            ReplaceString("STATNAME_HIGHESTITEMSCOLLECTED", "Most trash held");
            ReplaceString("STATNAME_TOTALSTAGESCOMPLETED", "Times fled");
            ReplaceString("STATNAME_HIGHESTSTAGESCOMPLETED", "Times fled");
            ReplaceString("STATNAME_TOTALPURCHASES", "Offered");
            ReplaceString("STATNAME_HIGHESTPURCHASES", "Offered");

            ReplaceString("GAME_RESULT_LOST", "PATHETHIC");
            ReplaceString("GAME_RESULT_WON", "IMPRESSIVE");
            ReplaceString("GAME_RESULT_UNKNOWN", "where are you?");
        }

        private void BloodShrinesCost100Percent(On.RoR2.ShrineBloodBehavior.orig_FixedUpdate orig, ShrineBloodBehavior self)
        {
            orig(self);
            self.GetFieldValue<PurchaseInteraction>("purchaseInteraction").Networkcost = 100;
        }


        private void MakeSureBonusDirectorDiesOnStageFinish(TeleporterInteraction obj)
        {
            if (obj.bonusDirector && obj.bonusDirector.enabled)
            {
                obj.bonusDirector.enabled = false;
            }
        }

        private void NoSafetyAfterFinishCharging(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdfld<TeleporterInteraction>("bonusDirector")
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<TeleporterInteraction>("bonusDirector"),
                x => x.MatchLdcI4(0)
                );
            c.Index += 1;
            c.RemoveRange(2);
            c.EmitDelegate<Action<CombatDirector>>((director) =>
            {
                director.expRewardCoefficient = 0f;
            }
            );
        }

        private void AdjustRegen(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int monsoonPlayerHelperCountIndex = 25;
            c.GotoNext(
                x => x.MatchLdcI4((int)ItemIndex.MonsoonPlayerHelper),
                x => x.MatchCallvirt<Inventory>("GetItemCount"),
                x => x.MatchStloc(out monsoonPlayerHelperCountIndex)
                );
            int regenMultiIndex = 37;
            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out regenMultiIndex),
                x => x.MatchLdloc(monsoonPlayerHelperCountIndex)
                );
            c.Index += 1;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, regenMultiIndex);
            c.EmitDelegate<Func<CharacterBody, float, float>>((self, regenMulti) =>
            {
                if (self.isPlayerControlled)
                {
                    regenMulti -= 0.5f;
                    Debug.Log("Reduced RegenMulti to: " + regenMulti);
                }
                return regenMulti;
            });
            c.Emit(OpCodes.Stloc, regenMultiIndex);
            int regenIndex = 36;
            c.GotoPrev(MoveType.Before,
                x => x.MatchStloc(out regenIndex),
                x => x.MatchLdcR4(1),
                x => x.MatchStloc(regenMultiIndex)
                ); ;
            c.Index += 1;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, regenIndex);
            c.EmitDelegate<Func<CharacterBody, float, float>>((self, regen) =>
            {
                if (self.teamComponent.teamIndex == TeamIndex.Monster)
                {
                    regen += self.maxHealth * 0.01f;
                }
                return regen;
            });
            c.Emit(OpCodes.Stloc, regenIndex);
        }
        private void ChangeOSP(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.Before,
                    x => x.MatchCallvirt<HealthComponent>("get_hasOneshotProtection")
                    );
                c.RemoveRange(2);
                c.Emit(OpCodes.Pop);
                c.GotoNext(MoveType.Before,
                        x => x.MatchLdcR4(0.9f)
                        );
                c.Remove();
                c.Emit(OpCodes.Ldc_R4, 0.99f);
            }
            catch (Exception e)
            {
                Logger.LogWarning("Couldn't modify OneShotProtection. Maybe you have a mod interfering. Game might act weird.");
                Logger.LogError(e);
            }
        }
    }
}
