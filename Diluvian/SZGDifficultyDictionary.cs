using System.Collections.Generic;

namespace Diluvian
{
    class SZGDifficultyDictionary : Dictionary<string,string>
    {
        private static Dictionary<string, string> instance = null;

        public static Dictionary<string, string> DictionaryToUse
        {
            get
            {
                if (instance == null)
                {
                    instance = new SZGDifficultyDictionary();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public SZGDifficultyDictionary()
        {
            BuildInteractibles();
            BuildItems();
            BuildObjectives();
            BuildPause();
            BuildStats();
        }

        public void BuildInteractibles()
        {
            Add("COST_PERCENTHEALTH_FORMAT", "?");//hide the cost for bloodshrines.
            Add("SHRINE_BLOOD_USE_MESSAGE_2P", "<color=#707>Look, it hurt itself.</color><style=cShrine> You have gained {1} gold.</color>");
            Add("SHRINE_BLOOD_USE_MESSAGE", "<color=#401>{0} has shown us their blood. </color><style=cShrine> They gained {1} gold.</color>");
            Add("SHRINE_HEALING_USE_MESSAGE_2P", "<color=#405>A beacon?</color><color=#840> To the woods. </color><color=#046>Primitive.</color> <color=#000>Effective.</color>");
            Add("SHRINE_HEALING_USE_MESSAGE", "<color=#055>A haven for {0}.</color><color=#800> Curious.</color>");
            Add("SHRINE_BOSS_BEGIN_TRIAL", "<color=#061>Time to see what their mettle is worth.</color>");
            Add("SHRINE_BOSS_END_TRIAL", "<color=#204> And our test goes on.</color>");
            Add("SHRINE_RESTACK_USE_MESSAGE_2P", "<color=#333>Refined.<color=#008> Sharpened.<color=#000> Aligned.");
            Add("SHRINE_RESTACK_USE_MESSAGE", "<color=#248>{0} is one step closer.</color>");
            Add("SHRINE_COMBAT_USE_MESSAGE_2P", "<color=#420>The threat widens. <color=#014>The struggle tightens.");
            Add("SHRINE_COMBAT_USE_MESSAGE", "<color=#040>{0} is acting odd.<color=#804> Inviting {1}s, vexing.");
        }

        public void BuildItems()
        {
            Add("ITEM_BEAR_PICKUP", "Chance to block incoming damage. Rolled <style=cDeath>negatively</style>.");
        }

        public void BuildPause()
        {
            Add("PAUSE_RESUME", "Continue.");
            Add("PAUSE_SETTINGS", "Calibrate your senses.");
            Add("PAUSE_QUIT_TO_MENU", "Stop the simulation.");
            Add("PAUSE_QUIT_TO_DESKTOP", "End the universe.");
            Add("QUIT_RUN_CONFIRM_DIALOG_BODY_SINGLEPLAYER", "Boring.");
            Add("QUIT_RUN_CONFIRM_DIALOG_BODY_CLIENT", "A figment of our imagination. It will continue without you.");
            Add("QUIT_RUN_CONFIRM_DIALOG_BODY_HOST", "With you, this world ends. The others aren't real.");
        }

        public void BuildObjectives()
        {
            Add("OBJECTIVE_FIND_TELEPORTER", "Continue the simulation.");
            Add("OBJECTIVE_DEFEAT_BOSS", "Complete the Act!");
        }

        public void BuildStats()
        {
            Add("STAT_KILLER_NAME_FORMAT", "SImulation ended by: <color=#FFFF7F>{0}</color>.");
            Add("STAT_POINTS_FORMAT", "");//Delete points
            Add("STAT_TOTAL", "");//delete more points
            Add("STAT_CONTINUE", "Begin anew.");

            Add("STATNAME_TOTALTIMEALIVE", "Screentime");
            Add("STATNAME_TOTALDEATHS", "Stepped out");
            Add("STATNAME_HIGHESTLEVEL", "Steps taken towards us");
            Add("STATNAME_TOTALGOLDCOLLECTED", "Wealth gotten");
            Add("STATNAME_TOTALDISTANCETRAVELED", "Stage stretched out");
            Add("STATNAME_TOTALMINIONDAMAGEDEALT", "Commandeered clean up");
            Add("STATNAME_TOTALITEMSCOLLECTED", "Props put on");
            Add("STATNAME_TOTALSTAGESCOMPLETED", "Acts progressed");
            Add("STATNAME_TOTALPURCHASES", "Amount of assets acquired");

            Add("GAME_RESULT_LOST", "DISAPPOINTING.");
            Add("GAME_RESULT_WON", "TIME REPEATS.");
            Add("GAME_RESULT_UNKNOWN", "WELCOME.");
        }
    }
}
