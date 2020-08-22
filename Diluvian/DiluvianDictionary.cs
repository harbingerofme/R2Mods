using System;
using System.Collections.Generic;

namespace Diluvian
{
    public class DiluvianDictionary : Dictionary<string, string> { 


        private static Dictionary<string, string> instance = null;
        public static Dictionary<string, string> DictionaryToUse
        {
            get
            {
                if (instance == null)
                {
                    instance = new DiluvianDictionary();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        DiluvianDictionary() : base()
        {
            BuildInteractibles();
            BuildItems();
            BuildObjectives();
            BuildPause();
            BuildStats();
        }

        public void BuildInteractibles()
        {
            Add("MSOBELISK_CONTEXT", "Escape the madness");
            Add("MSOBELISK_CONTEXT_CONFIRMATION", "Take the cowards way out");
            Add("COST_PERCENTHEALTH_FORMAT", "?");//hide the cost for bloodshrines.
            Add("SHRINE_BLOOD_USE_MESSAGE_2P", "<style=cDeath>N'Kuhana</style>: This pleases me. <style=cShrine>({1})</color>");
            Add("SHRINE_BLOOD_USE_MESSAGE", "<style=cDeath>N'Kuhana</style>: {0} has paid their respects. Will you do the same? <style=cShrine>({1})</color>");
            Add("SHRINE_HEALING_USE_MESSAGE_2P", "<style=cDeath>N'Kuhana</style>: Bask in my embrace.");
            Add("SHRINE_HEALING_USE_MESSAGE", "<style=cDeath>N'Kuhana</style>: Bask in my embrace.");
            Add("SHRINE_BOSS_BEGIN_TRIAL", "<style=cShrine>Show me your courage.</style>");
            Add("SHRINE_BOSS_END_TRIAL", "<style=cShrine>Your effort entertains me.</style>");
            Add("PORTAL_MYSTERYSPACE_CONTEXT", "Hide in another realm.");
            Add("SCAVLUNAR_BODY_SUBTITLE", "The weakling");
            Add("MAP_LIMBO_SUBTITLE", "Hideaway");
        }

        public void BuildItems()
        {
            Add("ITEM_BEAR_PICKUP", "Chance to block incoming damage. They think you are <style=cDeath>unlucky</style>.");
        }

        public void BuildObjectives()
        {
            Add("OBJECTIVE_FIND_TELEPORTER", "Flee the area");
            Add("OBJECTIVE_DEFEAT_BOSS", "Defeat the <style=cDeath>Anchor</style>");
        }
        public void BuildPause()
        {
            Add("PAUSE_RESUME", "Entertain me");
            Add("PAUSE_SETTINGS", "Change your perspective");
            Add("PAUSE_QUIT_TO_MENU", "Give up");
            Add("PAUSE_QUIT_TO_DESKTOP", "Don't come back");
            Add("QUIT_RUN_CONFIRM_DIALOG_BODY_SINGLEPLAYER", "You are a disappointment.");
            Add("QUIT_RUN_CONFIRM_DIALOG_BODY_CLIENT", "Leave these weaklings with me?");
            Add("QUIT_RUN_CONFIRM_DIALOG_BODY_HOST", "You are my main interest, <style=cDeath>end the others?</style>");
        }
        public void BuildStats()
        {
            Add("STAT_KILLER_NAME_FORMAT", "Released by: <color=#FFFF7F>{0}</color>");
            Add("STAT_POINTS_FORMAT", "");//Delete points
            Add("STAT_TOTAL", "");//delete more points
            Add("STAT_CONTINUE", "Try again");

            Add("STATNAME_TOTALTIMEALIVE", "Wasted time");
            Add("STATNAME_TOTALDEATHS", "Times Blessed");
            Add("STATNAME_HIGHESTLEVEL", "Strength Acquired");
            Add("STATNAME_TOTALGOLDCOLLECTED", "Greed");
            Add("STATNAME_TOTALDISTANCETRAVELED", "Ground laid to waste");
            Add("STATNAME_TOTALMINIONDAMAGEDEALT", "Pacification delegated");
            Add("STATNAME_TOTALITEMSCOLLECTED", "Trash cleaned up");
            Add("STATNAME_HIGHESTITEMSCOLLECTED", "Most trash held");
            Add("STATNAME_TOTALSTAGESCOMPLETED", "Times fled");
            Add("STATNAME_HIGHESTSTAGESCOMPLETED", "Times fled");
            Add("STATNAME_TOTALPURCHASES", "Offered");
            Add("STATNAME_HIGHESTPURCHASES", "Offered");

            Add("GAME_RESULT_LOST", "PATHETHIC");
            Add("GAME_RESULT_WON", "IMPRESSIVE");
            Add("GAME_RESULT_UNKNOWN", "where are you?");
        }
    }
}
