using RoR2;
using R2API;
using RoR2.Achievements;

namespace Diluvian
{
    class DiluvianCompletedAchievement : BaseEndingAchievement, IModdedUnlockableDataProvider
    {
        public string AchievementIdentifier => "COMPLETE_MAINENDING_DILUVIAN";

        public string UnlockableIdentifier => "COMPLETE_MAINENDING_DILUVIAN_UNLOCKABLE";

        public string PrerequisiteUnlockableIdentifier => "";

        public string AchievementNameToken => "COMPLETE_MAINENDING_DILUVIAN_NAME";

        public string AchievementDescToken => "COMPLETE_MAINENDING_DILUVIAN_DESC";

        public string UnlockableNameToken => "DIFFICULTY_ECLIPSED_DILUVIAN_NAME";

        public string SpritePath => DiluvianPlugin.assetString+ "diluvian-"+DiluvianPlugin.diluvianArtist.Value+".png";


        protected override bool ShouldGrant(RunReport runReport)
        {
            return runReport.gameEnding == RoR2Content.GameEndings.mainEnding && DiluvianPlugin.myDefs.ContainsKey(runReport.ruleBook.FindDifficulty());
        }

        public override void OnGranted()
        {
            base.OnGranted();
            Syzygy.Unlocked();
        }
    }
}
