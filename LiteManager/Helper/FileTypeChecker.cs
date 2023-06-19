using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteManager.Helper
{
    internal class FileTypeChecker
    {
        /// <summary>
        /// Gets the file type based on a list of files and folders.
        /// </summary>
        /// <param name="listOfFilesAndFolders">The list of files and folders.</param>
        /// <returns>The file type based on the list.</returns>
        public static string GetFileType(List<string> ListOfFilesAndFolders)
        {
            // Check if all items are files or all items are folders
            bool allFiles = ListOfFilesAndFolders.All(File.Exists);
            bool allFolders = ListOfFilesAndFolders.All(Directory.Exists);

            // If all items are files and have the same extension, return the file type based on the extension
            // If all items are folders, return the folder type message
            // If the list contains multiple types, return the message for multiple types
            return allFiles && ListOfFilesAndFolders.All(item => Path.GetExtension(item) == Path.GetExtension(ListOfFilesAndFolders.First()))
                ? GetFileTypeByExtension(Path.GetExtension(ListOfFilesAndFolders.First())) != null ? $"All of type {GetFileTypeByExtension(Path.GetExtension(ListOfFilesAndFolders.First()))}" : $"All of type {GetFileTypeByExtension(Path.GetExtension(ListOfFilesAndFolders.First()))} File"
                : allFolders ? "All of Type File folder" : "Multiple Types";
        }

        /// <summary>
        /// Gets the file type based on the file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The file type based on the extension.</returns>
        public static string GetFileTypeByExtension(string extension)
        {
            string fileType = null;

            // Open the Windows Registry to retrieve the file type based on the extension
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension))
            {
                if (key != null && key.GetValue("") != null)
                {
                    string fileClass = key.GetValue("").ToString();

                    // Open the registry key for the file class
                    using (RegistryKey classKey = Registry.ClassesRoot.OpenSubKey(fileClass))
                    {
                        if (classKey != null && classKey.GetValue("") != null)
                        {
                            fileType = classKey.GetValue("").ToString();
                        }
                    }
                }
            }

            // If the file type is not found, construct a generic file type based on the extension
            return fileType ?? $"{extension.Replace(".", "").ToUpperInvariant()} File";
        }

    }
}
