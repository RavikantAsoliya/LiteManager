using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace LiteManager.Helper
{
    internal class IconProvider
    {

        /// <summary>
        /// Retrieves the icon associated with a specific file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>An <see cref="Icon"/> object representing the file's icon, or null if the file does not exist or the file name is empty.</returns>
        public static Icon GetIconByFileName(string fileName)
        {
            // Check if the file name is empty or null, or if the file doesn't exist
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return null;

            // Create an instance of the SHFILEINFO structure to hold file information
            SHFILEINFO shinfo = new SHFILEINFO();

            // Retrieve the file icon using the Win32 API call
            Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

            // Create an Icon object from the obtained icon handle
            return Icon.FromHandle(shinfo.hIcon);
        }


        /// <summary>
        /// Retrieves the icon associated with a specific file type.
        /// </summary>
        /// <param name="fileType">The file type (e.g., ".txt", ".doc").</param>
        /// <param name="isLarge">Specifies whether to retrieve a large or small icon.</param>
        /// <returns>An <see cref="Icon"/> object representing the file type's icon, or null if the file type is empty.</returns>
        public static Icon GetIconByFileType(string fileType, bool isLarge)
        {
            // Check if the file type is empty or null
            if (string.IsNullOrEmpty(fileType))
                return null;

            // Determine the icon string based on the file type
            string regIconString = fileType[0] == '.'
                ? GetDefaultIconStringFromRegistry(fileType)
                : Environment.SystemDirectory + "\\shell32.dll,3";

            // Split the icon string into file name and index components
            string[] fileIcon = regIconString.Split(',');
            if (fileIcon.Length != 2)
                fileIcon = new string[] { Environment.SystemDirectory + "\\shell32.dll", "2" };

            // Prepare arrays to store icon handles
            int[] phiconLarge = new int[1];
            int[] phiconSmall = new int[1];

            // Retrieve the icons using Win32 API call
            uint _ = Win32.ExtractIconEx(fileIcon[0], int.Parse(fileIcon[1]), phiconLarge, phiconSmall, 1);

            // Select the appropriate icon handle based on the 'isLarge' parameter
            IntPtr iconHandle = new IntPtr(isLarge ? phiconLarge[0] : phiconSmall[0]);

            // Create an Icon object from the icon handle
            return Icon.FromHandle(iconHandle);
        }


        /// <summary>
        /// Retrieves the default icon string associated with a file type from the Windows Registry.
        /// </summary>
        /// <param name="fileType">The file type (e.g., ".txt", ".doc").</param>
        /// <returns>A string representing the default icon string, or a default icon string if the file type is not found in the registry.</returns>
        private static string GetDefaultIconStringFromRegistry(string fileType)
        {
            // Open the registry key corresponding to the specified fileType
            using (RegistryKey regVersion = Registry.ClassesRoot.OpenSubKey(fileType, true))
            {
                // Check if the registry key exists
                if (regVersion != null)
                {
                    // Get the value of the default file type associated with the registry key
                    string regFileType = regVersion.GetValue("") as string;

                    // Open the registry key corresponding to the default file type's DefaultIcon subkey
                    using (RegistryKey regDefaultIcon = Registry.ClassesRoot.OpenSubKey(regFileType + @"\DefaultIcon", true))
                    {
                        // Check if the registry key exists
                        if (regDefaultIcon != null)
                        {
                            // Retrieve the value of the default icon string
                            return regDefaultIcon.GetValue("") as string;
                        }
                    }
                }
            }

            // If no icon string is found in the registry, return the default icon string for shell32.dll
            return Environment.SystemDirectory + "\\shell32.dll,0";
        }

    }


    // Struct representing SHFILEINFO for Win32 API calls
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        // Handle to the icon representing the file
        public IntPtr hIcon;

        // System index of the icon
        public IntPtr iIcon;

        // File attributes
        public uint dwAttributes;

        // Display name of the file
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        // Type name of the file
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }


    internal class Win32
    {
        // Icon retrieval flags
        public const uint SHGFI_ICON = 0x100;     // Retrieve the icon for the specified file or file type
        public const uint SHGFI_LARGEICON = 0x0;  // Retrieve a large-sized icon
        public const uint SHGFI_SMALLICON = 0x1;  // Retrieve a small-sized icon


        // Retrieves information about a file
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        // Extracts icons from a file
        [DllImport("shell32.dll")]
        public static extern uint ExtractIconEx(string lpszFile, int nIconIndex, int[] phiconLarge, int[] phiconSmall, uint nIcons);
    }

}
