using BepInEx;
using R2API.Utils;

namespace AssetPlusRequester
{

    [BepInDependency("com.bepis.r2api")]
    [R2APISubmoduleDependency(nameof(R2API.AssetPlus))]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class AssetPlusRequesterPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "AssetPlusRequester",
            AUTHOR = "Harb",
            GUID = MODNAME,
            VERSION = "1.0.0";

    }
}
