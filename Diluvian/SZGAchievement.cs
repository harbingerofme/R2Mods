using R2API;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diluvian
{
    class SZGAchievement : BaseEndingAchievement, IModdedUnlockableDataProvider 
    {
        public string AchievementIdentifier => "COMPLETE_AAO_SYZYGY";

        public string UnlockableIdentifier => "Complete_AAO_SYZYGY_UNLOCKABLE";

        public string PrerequisiteUnlockableIdentifier => "COMPLETE_MAINENDING_DILUVIAN_UNLOCKABLE";

        public string AchievementNameToken => "COMPLETE_AAO_SYZYGY_NAME";

        public string AchievementDescToken => "COMPLETE_AAO_SYZYGY_DESC";

        public string UnlockableNameToken => "DIFFICULTY_ECLIPSED_DILUVIAN_NAME";

        public string SpritePath => DiluvianPlugin.assetString + "AnotherRoRDifficultyIcon-" + DiluvianPlugin.syzygyArtist.Value + ".png";

        protected override bool ShouldGrant(RunReport runReport)
        {

            bool isSyzygy = DiluvianPlugin.EDindex == runReport.ruleBook.FindDifficulty();
            bool isObliteration = runReport.gameEnding == RoR2Content.GameEndings.obliterationEnding || runReport.gameEnding ==  RoR2Content.GameEndings.limboEnding;
            bool allArtifactsEnabled = true;
            var mask = runReport.ruleBook.GenerateArtifactMask();
            for(int i=0; i < ArtifactCatalog.artifactCount; i++)
            {
                allArtifactsEnabled &= mask.HasArtifact((ArtifactIndex) i);
            }
            return isSyzygy && isObliteration && allArtifactsEnabled;
        }
    }
}
