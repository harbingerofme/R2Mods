using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static Diluvian.SZGHooks;

namespace Diluvian
{
    class Syzygy : HarbDifficultyDef
    {

        internal static Syzygy def;
        public Syzygy()
        {
            BaseToken = "DIFFICULTY_ECLIPSED_DILUVIAN";
            Color = new Color(0.3f, 0.4f, 0.6f);
            ScalingValue = 3.6f;
            HealthRegenMod = -0.6f;
            MonsterRegenMod = 0.015f;
            EliteModifier = 0.8f;
            MakeUnknown();
        }

        public void MakeUnknown()
        {
            Description = "You do not know what lies beyond yet.";
            Name = "UNKNOWN DIFFICULTY";
            StartMessages = new string[]{ "You are not ready yet.","UNWORTHY"};
            IconPath = "hiddenIcon-"+ DiluvianPlugin.syzygyArtist.Value + ".png";
            resetStuff();
        }

        public void MakeKnown()
        {
            Description = BuildDescription();
            Name = "Syzygy";
            IconPath = $"anotherRoRDifficultyIcon-"+DiluvianPlugin.syzygyArtist.Value+".png";
            StartMessages = new string[] { "And so it begins, again.","Always starting, never ending.","Our candidate moves once more.","Pay attention now. It goes." };
            resetStuff();
        }
    
        private void resetStuff()
        {
            this.DifficultyDef.SetFieldValue<string>("iconPath", IconPath);
            this.DifficultyDef.SetFieldValue<bool>("foundIconSprite", true);
            this.DifficultyDef.SetFieldValue<Sprite>("iconSprite", UnityEngine.Resources.Load<Sprite>(DiluvianPlugin.assetString + IconPath));
        }


        public override Dictionary<string, string> Language => SZGDifficultyDictionary.DictionaryToUse;

        public static void Unlocked()
        {
            var singleton = Syzygy.def;
            if (singleton != null)
            {
                singleton.MakeKnown();
                singleton.RewriteTokens();
            }
        }

        public static void Locked()
        {
            var singleton = Syzygy.def;
            if (singleton != null)
            {
                singleton.MakeUnknown();
                singleton.RewriteTokens();
            }
        }

        private void RewriteTokens()
        {
            var def = DifficultyDef;
            LanguageAPI.Add(def.nameToken, Name);
            LanguageAPI.Add(def.descriptionToken, Description);
        }
        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (obj.isPlayerControlled)
            {
                obj.AddBuff(BuffIndex.HealingDisabled);
                obj.AddBuff(BuffIndex.Weak);
                obj.AddBuff(BuffIndex.Cripple);
                obj.AddBuff(BuffIndex.Pulverized);
                obj.AddBuff(BuffIndex.Nullified);
            }
        }

        private bool illegalAccess;
        public override void ApplyHooks()
        {
            if (NetworkServer.active)
            {
                RoR2.SceneDirector.onGenerateInteractableCardSelection += RemoveCleansingPools;
                IL.RoR2.GenericPickupController.OnTriggerStay += LunarsUnConsentional;
                SceneDirector.onPostPopulateSceneServer += StartAMeteorTimer;
                On.RoR2.ScavengerItemGranter.Start += GiveScavsLunarsAndBossItems;
                if (!EveryoneCanPlay())
                {
                    illegalAccess = true;
                    Diluvian.DiluvianPlugin.GetLogger().LogInfo("Attempted to start game without having unlocked the achievement for it!");
                    RoR2.CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
                }
            }
        }
        public override void UndoHooks()
        {
            if (NetworkServer.active)
            {
                RoR2.SceneDirector.onGenerateInteractableCardSelection -= RemoveCleansingPools;
                IL.RoR2.GenericPickupController.OnTriggerStay -= LunarsUnConsentional;
                SceneDirector.onPostPopulateSceneServer -= StartAMeteorTimer;
                On.RoR2.ScavengerItemGranter.Start -= GiveScavsLunarsAndBossItems;
                if (illegalAccess)
                {
                    illegalAccess = false;
                    CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
                }
            }
        }

        private bool EveryoneCanPlay()
        {
            UnlockableDef def = UnlockableCatalog.GetUnlockableDef("COMPLETE_MAINENDING_DILUVIAN_UNLOCKABLE");
            ReadOnlyCollection<NetworkUser> onlyInstancesList = NetworkUser.readOnlyInstancesList;
            for (int index = 0; index < onlyInstancesList.Count; ++index)
            {
                if (!onlyInstancesList[index].unlockables.Contains(def))
                    return false;
            }
            return true;
        }

        private string BuildDescription()
        {
            string desc = "<color=#224470>With an audience of <color=white>stars</color>, give it your all.</style><style=cStack>\n";
            desc = string.Join("\n",
                desc,
                $">Difficulty Scaling: +{Mathf.RoundToInt(ScalingValue * 50 - 100)}%",
                ">All <style=cArtifact>Diluvian</style> modifiers",
                ">All <style=cIsHealth>Eclipse</style> modifiers",
                "><style=cIsUtility>The destruction of the Moon has influenced the world.</style>"
                );
            desc += "</style>";
            return desc;
        }
    }
}
