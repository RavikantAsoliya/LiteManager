using Shell32;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

namespace LiteManager.Helper
{
    class RecentFiles
    {
        /// <summary>
        /// Retrieves the target file path of a shortcut.
        /// </summary>
        /// <param name="shortcutFilename">The path of the shortcut file.</param>
        /// <returns>The target file path of the shortcut.</returns>
        public static string GetShortcutTargetFilePath(string shortcutFilename)
        {
            // Dynamically creates an instance of the WScript.Shell object
            var type = Type.GetTypeFromProgID("WScript.Shell");
            object instance = Activator.CreateInstance(type);

            // Invokes the CreateShortcut method to create the shortcut object
            var result = type.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, instance, new object[] { shortcutFilename });

            // Retrieves the TargetPath property from the shortcut object
            var targetFilePath = result.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, result, null) as string;

            return targetFilePath;
        }


        /// <summary>
        /// Retrieves a collection of recent files by parsing .lnk files in the Recent folder and removes duplicates.
        /// </summary>
        /// <returns>A collection of unique recent file paths.</returns>
        public static IEnumerable<string> GetRecentFiles()
        {
            // Gets the path of the Recent folder
            var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            // Selects the target file paths from .lnk files in the Recent folder
            var recentFiles = from file in Directory.EnumerateFiles(recentFolder)
                              where Path.GetExtension(file) == ".lnk"
                              select GetShortcutTargetFilePath(file);

            // Removes duplicate file paths
            var uniqueFiles = recentFiles.Distinct();

            return uniqueFiles;
        }


        /// <summary>
        /// Retrieves a collection of recent file paths by accessing the Recent folder using the Shell32 COM object.
        /// </summary>
        /// <returns>A collection of recent file paths.</returns>
        public static IEnumerable<string> GetRecentFilesShortcut()
        {
            // Gets the path of the Recent folder
            var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            // Creates an instance of the Shell32 COM object and accesses the Recent folder
            var shell = new Shell();
            var folder = shell.NameSpace(recentFolder);

            var recentFiles = new List<string>();

            // Iterates through the items in the Recent folder and adds their paths to the collection
            foreach (FolderItem file in folder.Items())
            {
                recentFiles.Add(file.Path);
            }

            return recentFiles;
        }
    }
}

