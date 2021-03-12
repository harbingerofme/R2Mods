using BepInEx;

using R2API;
using R2API.Utils;

namespace Test
{
    [BepInDependency("com.bepis.r2api")]
	[R2APISubmoduleDependency(new string[]
	{
		"UnlockablesAPI",
		"SurvivorAPI",
		"SoundAPI",
		"ResourcesAPI",
		"R2API",
		"PrefabAPI",
		"OrbAPI",
		"LobbyConfigAPI",
		"LoadoutAPI",
		"LanguageAPI",
		"ItemAPI",
		"ItemDropAPI",
		"InventoryAPI",
		"FontAPI",
		"EliteAPI",
		"EffectAPI",
		"DotAPI",
		"DirectorAPI",
		"DifficultyAPI",
		"BuffAPI",
		"AssetAPI",
		"CommandHelper"
	})]
	[BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class TestPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "r2apitest",
            AUTHOR = "harbingerof",
            GUID = "me." + AUTHOR + "." + MODNAME,
            VERSION = "0.0.0";
    }
}