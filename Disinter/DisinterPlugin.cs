using BepInEx;
using BepInEx.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Disinter
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class DisinterPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "Disinter",
            AUTHOR = "HarbingerOf",
            GUID = "me." + AUTHOR + "." + MODNAME,
            VERSION = "1.0.0";

        private ConfigEntry<bool> export;
        public bool ShouldExport => export.Value;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Awake is automatically called by Unity")]
        private void Awake() //Called when loaded by BepInEx.
        {
            On.RoR2.MorgueManager.HistoryFileInfo.LoadRunReport += HistoryFileInfo_LoadRunReport1;
            export = Config.Bind(new ConfigDefinition("", "export"), false, new ConfigDescription("Export the files instead of deleting them. This option is meant for developers to see what wrong output they produced."));
        }

        private void HistoryFileInfo_LoadRunReport1(On.RoR2.MorgueManager.HistoryFileInfo.orig_LoadRunReport orig, ref RoR2.MorgueManager.HistoryFileInfo self, RoR2.RunReport dest)
        {
            try
            {
                orig(ref self, dest);
            }
            catch (XmlException _)
            {
                Logger.LogWarning("Caught an xml exception in a runreport. Moving it out of the history.");
                var entry = self.fileEntry;
                if (ShouldExport)
                {
                    var destPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), entry.Name);
                    File.Copy(entry.FileSystem.ConvertPathToInternal(entry.Path), destPath);
                }
                entry.Delete();
            }
        }
    }
}