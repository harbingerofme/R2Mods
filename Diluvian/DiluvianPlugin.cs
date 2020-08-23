using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Diluvian
{

    [R2APISubmoduleDependency(nameof(DifficultyAPI),nameof(ResourcesAPI),nameof(LanguageAPI),nameof(UnlockablesAPI))]
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.jarlyk.eso", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, NAME, VERSION)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod,VersionStrictness.EveryoneNeedSameModVersion)]
    public class DiluvianPlugin : BaseUnityPlugin
    {
        public const string
            NAME = "Diluvian",
            GUID = "com.harbingerofme." + NAME,
            VERSION = "2.0.3";

        internal static DiluvianPlugin instance;

        public static ManualLogSource GetLogger() { if (instance != null) return instance.Logger; return null; }

        //ResourceAPI stuff.
        internal const string assetPrefix = "@HarbDiluvian";
        internal const string assetString = assetPrefix + ":Assets/Diluvian/";
        //The Assetbundle bundled into the dll has the path above to the icon.
        //When replicating make sure the icon is saved as a sprite.

        //Hold for the old language so that we can restore it.
        private readonly Dictionary<string, string> DefaultLanguage;
        //Keeping track of ESO
        internal static bool ESOenabled = false;
        //Keeping track of internal state.
        private bool HooksApplied = false;
        //Remember vanilla values.

        internal static Dictionary<DifficultyIndex, HarbDifficultyDef> myDefs;

        internal static DifficultyIndex EDindex;
        internal static DifficultyIndex Dindex;
        internal static RuleChoiceDef EDrule;

        internal static ConfigEntry<String> diluvianArtist;
        internal static ConfigEntry<String> syzygyArtist;

        private DiluvianPlugin()
        {
            instance = this;
            DefaultLanguage = new Dictionary<string, string>();
            myDefs = new Dictionary<DifficultyIndex, HarbDifficultyDef>();
        }

        public void Awake()
        {
            //acquire my assetbundle and give it to the resource api.
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Diluvian.Resources.diluvian"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new R2API.AssetBundleResourcesProvider(assetPrefix, bundle);
                ResourcesAPI.AddProvider(provider);
            }
            //Check ESO existence.
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.jarlyk.eso"))
            {
                ESOenabled = true;
                Logger.LogWarning("Using ESO's elite cards!");
            }

            diluvianArtist = Config.Bind(new ConfigDefinition("Art", "Diluvian"), "avizvul",new ConfigDescription("The artist for the diluvian icon. Options: \"harb\",\"avizvul\",\"horus\"",new AcceptableValueList<string>(new string[] { "avizvul","harb","horus"})));
            syzygyArtist = Config.Bind(new ConfigDefinition("Art", "OTHER"), "avizvul",new ConfigDescription("The artist for the other icon.Options: \"harb\",\"avizvul\"",new AcceptableValueList<string>(new string[] { "avizvul", "harb" })));

            UnlockablesAPI.AddUnlockable<DiluvianCompletedAchievement>(true);
            LanguageAPI.Add("COMPLETE_MAINENDING_DILUVIAN_NAME", "Unobscured. Unblinking. Unrelenting.");
            LanguageAPI.Add("COMPLETE_MAINENDING_DILUVIAN_DESC", "Completed the game on Diluvian.");
            LanguageAPI.Add("DIFFICULTY_ECLIPSED_DILUVIAN_NAME", "what is this field for?");

            DiluvianDifficulty.def = new DiluvianDifficulty();
            Dindex = DifficultyAPI.AddDifficulty(DiluvianDifficulty.def.DifficultyDef);
            myDefs.Add(Dindex, DiluvianDifficulty.def);
            Syzygy.def = new Syzygy();
            EDindex  = DifficultyAPI.AddDifficulty(Syzygy.def.DifficultyDef,true);
            myDefs.Add(EDindex, Syzygy.def);

            On.RoR2.RuleDef.FromDifficulty += RuleDef_FromDifficulty;
            On.RoR2.UI.RuleCategoryController.SetData += RuleCategoryController_SetData;


            //This is where my hooks live. They themselves are events, not ONhooks
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
        }

        private void RuleCategoryController_SetData(On.RoR2.UI.RuleCategoryController.orig_SetData orig, RoR2.UI.RuleCategoryController self, RuleCategoryDef categoryDef, RuleChoiceMask availability, RuleBook ruleBook)
        {
            if(categoryDef.displayToken == "RULE_HEADER_DIFFICULTY" && EDrule !=null)
            {
                try
                {
                    var localUser1 = LocalUserManager.GetFirstLocalUser();
                    if (localUser1 != null)
                    {
                        bool knowsED = AchievementManager.GetUserAchievementManager(localUser1).userProfile.HasAchievement("COMPLETE_MAINENDING_DILUVIAN");
                        if (knowsED)
                        {
                            Syzygy.Unlocked();
                        }
                        else
                        {
                            Syzygy.Locked();
                        }
                        EDrule.spritePath = assetString + Syzygy.def.IconPath;
                        reloadLanguage();
                    }
                }
                catch(Exception e)
                {
                    Logger.LogWarning(e);
                }
            }
            orig(self, categoryDef, availability, ruleBook);
        }

        private RuleDef RuleDef_FromDifficulty(On.RoR2.RuleDef.orig_FromDifficulty orig)
        {
            var origReturn = orig();
            EDrule = origReturn.choices.First((def) =>
           {
               return def.difficultyIndex == EDindex;
           });
            return origReturn;
        }


        private void Run_onRunStartGlobal(Run run)
        {
            Logger.LogDebug("Run started on difficulty index: "+run.selectedDifficulty);
            //Only do stuff when Diluvian is selected.
            if (myDefs.TryGetValue(run.selectedDifficulty, out var def) && HooksApplied == false)
            {
                HooksApplied = true;

                if (NetworkServer.active)
                {
                    var message = def.StartMessages[Run.instance.runRNG.RangeInt(0, def.StartMessages.Length)];
                    ChatMessage.SendColored(message, def.Color);
                    //Make our hooks.
                    SharedHooks.ApplyBears();
                    SharedHooks.SetupCombatDirectorChanges(def.EliteModifier);
                    SharedHooks.ApplyBloodShrineRandom();
                }

                
                SharedHooks.ApplyRegenChanges(def.HealthRegenMod, def.MonsterRegenMod);
                SharedHooks.ApplyOSPHook();

                def.ApplyHooks();
                BuildLanguage(def.Language,true);
            }
        }


        private void Run_onRunDestroyGlobal(Run run)
        {
            if (myDefs.TryGetValue(run.selectedDifficulty, out var def) && HooksApplied)
            {
                //Remove all of our hooks on run end and restore elite tables to their original value.
                if (NetworkServer.active)
                {

                    SharedHooks.UndoBears();
                    SharedHooks.UndoCombatDirectorChanges();

                    SharedHooks.UndoBloodShrineRandom();
                }

                SharedHooks.UndoRegenChanges();
                SharedHooks.UndoOSPHook();
                BuildLanguage(DefaultLanguage,false);
                DefaultLanguage.Clear();

                def.UndoHooks();
                HooksApplied = false;
            }
        }

        private void BuildLanguage(Dictionary<string, string> dict, bool storeValue)
        {
            dict.ForEachTry((pair) =>
            {
                if(storeValue)
                    DefaultLanguage.Add(pair.Key, Language.currentLanguage.GetLocalizedStringByToken(pair.Key));
                LanguageAPI.Add(pair.Key, pair.Value);
            });
            reloadLanguage();
        }
       
        private static void reloadLanguage()
        {
            Language.CCLanguageReload(new ConCommandArgs());
        }
       
    }
}
