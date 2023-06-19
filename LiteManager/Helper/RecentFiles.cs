using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteManager.Helper
{
    //About the tools in the "recent" folder in the computer, Win+R, enter Recent to call out
    class RecentFilesUtil
    {
        //Obtain the target path (real path) of the shortcut according to the shortcut name (full path)
        public static string GetShortcutTargetFilePath(string shortcutFilename)
        {
            //Get WScript.Shell type
            var type = Type.GetTypeFromProgID("WScript.Shell");

            //Create an instance of this type
            object instance = Activator.CreateInstance(type);

            var result = type.InvokeMember("CreateShortCut", BindingFlags.InvokeMethod, null, instance, new object[] { shortcutFilename });

            //Get the target path (real path)
            var targetFilePath = result.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, result, null) as string;

            return targetFilePath;
        }

        //Get an enumerated collection of paths to recently used files
        public static IEnumerable<string> GetRecentFilesorg()
        {
            //Get the Recent path
            var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            //Get the file name (full path) under the "Recent" folder of the computer
            return from file in Directory.EnumerateFiles(recentFolder)

                       //Only take shortcut type files
                   where Path.GetExtension(file) == ".lnk"

                   //Get the corresponding real path
                   select GetShortcutTargetFilePath(file);
        }
        // Get an enumerated collection of paths to recently used files, filtering out duplicates
        public static IEnumerable<string> GetRecentFiles()
        {
            // Get the Recent path
            var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            // Get the file name (full path) under the "Recent" folder of the computer
            var recentFiles = from file in Directory.EnumerateFiles(recentFolder)
                                  // Only take shortcut type files
                              where Path.GetExtension(file) == ".lnk"
                              // Get the corresponding real path
                              select GetShortcutTargetFilePath(file);

            // Remove duplicates using Distinct
            var uniqueFiles = recentFiles.Distinct();

            return uniqueFiles;
        }

        public static IEnumerable<string> GetRecentFilesShortcut()
        {
            var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            var shell = new Shell();
            var folder = shell.NameSpace(recentFolder);

            var recentFiles = new List<string>();

            foreach (FolderItem file in folder.Items())
            {
                recentFiles.Add(file.Path);
            }

            return recentFiles;
        }
    }
}
