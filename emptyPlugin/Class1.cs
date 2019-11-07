using BepInEx;


namespace testPlugin
{
    [PluginMetadata(GUID,NAME,VERSION)]
    public class emptyPlugin : BaseUnityPlugin
    {
        public const string
            NAME = "emptyPlugin",
            GUID = "test.harbingerofme." + NAME,
            VERSION = "0.0.0";

        public void Awake()
        {

        }
    }
}
