using BepInEx;
using System;
using System.IO;

namespace InstallationChecker
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class InstallationCheckerPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "InstallationChecker",
            AUTHOR = "HarbingerOf",
            GUID = "me." + AUTHOR + "." + MODNAME,
            VERSION = "1.0.0";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Awake is automatically called by Unity")]
        private void Awake() //Called when loaded by BepInEx.
        {
            DisplayAllFilesInFolderRecursively(0,Paths.GameRootPath);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Start is automatically called by Unity")]
        private void Start() //Called at the first frame of the game.
        {

        }

        void DisplayAllFilesInFolderRecursively(int depth, string path)
        {
            string prefix = "";
            for(int i = 0; i < depth; i++)
            {
                prefix += " ";
            }

            var dir = new DirectoryInfo(path);
            Logger.LogMessage(prefix+"\\" + dir.Name);
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                Logger.LogMessage(prefix + "| " + file.Name);
            }
            foreach(var directory in dir.GetDirectories())
            {
                DisplayAllFilesInFolderRecursively(depth+2,directory.FullName);
            }
        }
    }
}
