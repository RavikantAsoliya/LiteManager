using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LiteManager.Helper
{
    //Tool class for obtaining the corresponding icon of the specified file in the system
    class IconProvider
    {
        //Read the icon according to the file name, if the specified file does not exist, return a null value.
        public static Icon GetIconByFileName(string fileName)
        {
            if (fileName == null || fileName.Equals(string.Empty)) return null;
            if (!File.Exists(fileName)) return null;

            SHFILEINFO shinfo = new SHFILEINFO();
            //Use this to get the small Icon
            Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
            //The icon is returned in the hIcon member of the shinfo struct
            System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
            return myIcon;
        }


        //Given a file extension (.*), the corresponding icon is returned. If it does not start with ".", it will return the icon of the folder.
        public static Icon GetIconByFileType(string fileType, bool isLarge)
        {
            if (fileType == null || fileType.Equals(string.Empty)) return null;
            string regIconString = null;
            string systemDirectory = Environment.SystemDirectory + "\\";

            if (fileType[0] == '.')
            {
                //Read file type information in the system registry
                RegistryKey regVersion = Registry.ClassesRoot.OpenSubKey(fileType, true);
                if (regVersion != null)
                {
                    string regFileType = regVersion.GetValue("") as string;
                    regVersion.Close();
                    regVersion = Registry.ClassesRoot.OpenSubKey(regFileType + @"\DefaultIcon", true);
                    if (regVersion != null)
                    {
                        regIconString = regVersion.GetValue("") as string;
                        regVersion.Close();
                    }
                }
                if (regIconString == null)
                {
                    //The file type registration information was not read, and the icon for an unknown file type was specified
                    regIconString = systemDirectory + "shell32.dll,0";
                }
            }
            else
            {
                //Directly assign as folder icon
                regIconString = systemDirectory + "shell32.dll,3";
            }
            string[] fileIcon = regIconString.Split(new char[] { ',' });
            if (fileIcon.Length != 2)
            {
                //If the icon registered in the system registry cannot be extracted directly, the general icon of the executable file will be returned
                fileIcon = new string[] { systemDirectory + "shell32.dll", "2" };
            }
            Icon resultIcon = null;
            try
            {
                //Call the API method to read the icon
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = Win32.ExtractIconEx(fileIcon[0], Int32.Parse(fileIcon[1]), phiconLarge, phiconSmall, 1);
                IntPtr IconHnd = new IntPtr(isLarge ? phiconLarge[0] : phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }
            catch { }
            return resultIcon;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };


    //Define the API method to call
    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // Large icon
        public const uint SHGFI_SMALLICON = 0x1; // Small icon

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("shell32.dll")]
        public static extern uint ExtractIconEx(string lpszFile, int nIconIndex, int[] phiconLarge, int[] phiconSmall, uint nIcons);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);
    }

}
