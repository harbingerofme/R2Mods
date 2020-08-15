using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InstallationChecker
{
    class InstallationChecker
    {
        public static void Main(string[] args)
        {
            var s = new StringBuilder();
            var exe = new FileInfo(args[0]);
            DisplayAllFilesInFolderRecursively(ref s, 0, exe.DirectoryName);
            File.WriteAllText(Path.Combine(exe.DirectoryName, "tree.txt"), s.ToString());
        }

        static void DisplayAllFilesInFolderRecursively(ref StringBuilder stringBuilder, int depth, string path)
        {
            string prefix = "";
            for (int i = 0; i < depth; i++)
            {
                prefix += " ";
            }

            var dir = new DirectoryInfo(path);
            stringBuilder.AppendLine(prefix + "\\" + dir.Name);
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                stringBuilder.AppendLine(prefix + "| " + file.Name);
            }
            foreach (var directory in dir.GetDirectories())
            {
                DisplayAllFilesInFolderRecursively(ref stringBuilder, depth + 2, directory.FullName);
            }
        }
    }
}
