using BepInEx;
using RoR2;
using System.Reflection;
using R2API.Utils;
using UnityEngine;

namespace Deluge
{
    [BepInPlugin(GUID,NAME,VERSION)]
    public class Deluge : BaseUnityPlugin
    {
        public const string
            NAME = "Deluge",
            GUID = "test.harbingerofme." + NAME,
            VERSION = "0.0.1";

        public static int DiffIndexOverride = 4;


        private DifficultyDef DelugeDef;
        private DifficultyDef RiskOfRain2DiscordDef;

        private Deluge()
        {
            DelugeDef = new DifficultyDef(2.5f, "DIFFICULTY_DELUGE_NAME", "Textures/AchievementIcons/texLoaderClearGameMonsoonIcon", "DIFFICULTY_DELUGE_DESCRIPTION", new Color(0.1f, 1, 0.2f));
            RiskOfRain2DiscordDef = new DifficultyDef(0.5f, "DIFFICULTY_RISKOFRAIN2DISCORD_NAME", "Textures/AchievementIcons/texCommandoClearGameMonsoonIcon", "DIFFICULTY_RISKOFRAIN2DISCORD_DESCRIPTION", new Color(0.6f, 0.4f, 1f));
        }

        public void Awake()
        {
            R2API.DifficultyAPI.AddDifficulty(DelugeDef);
            R2API.DifficultyAPI.AddDifficulty(RiskOfRain2DiscordDef);
        }

    }
}
