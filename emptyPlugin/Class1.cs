using BepInEx;


namespace testPlugin
{
    [BepInPlugin(GUID,NAME,VERSION)]
    public class EmptyPlugin : BaseUnityPlugin
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
