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
        public static string GetFileType(List<string> ListOfFilesAndFolders)
        {
            bool allFiles = ListOfFilesAndFolders.All(File.Exists);
            bool allFolders = ListOfFilesAndFolders.All(Directory.Exists);

            return allFiles && ListOfFilesAndFolders.All(item => Path.GetExtension(item) == Path.GetExtension(ListOfFilesAndFolders.First()))
                ? GetFileTypeByExtension(Path.GetExtension(ListOfFilesAndFolders.First())) != null ? $"All of type {GetFileTypeByExtension(Path.GetExtension(ListOfFilesAndFolders.First()))}" : $"All of type {GetFileTypeByExtension(Path.GetExtension(ListOfFilesAndFolders.First()))} File"
                : allFolders ? "All of Type File folder" : "Multiple Types";
        }

        public static string GetFileTypeByExtension(string extension)
        {
            string fileType = null;

            // Get the file type from the Windows Registry
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension))
            {
                if (key != null && key.GetValue("") != null)
                {
                    string fileClass = key.GetValue("").ToString();

                    using (RegistryKey classKey = Registry.ClassesRoot.OpenSubKey(fileClass))
                    {
                        if (classKey != null && classKey.GetValue("") != null)
                        {
                            fileType = classKey.GetValue("").ToString();
                        }
                    }
                }
            }
            return fileType ?? $"{extension.Replace(".", "").ToUpperInvariant()} File";
        }

    }
}
