using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteManager.Helper
{
    internal class SizeFormatter
    {
        public static string FormatSize(long size, bool withBytes = false)
        {
            // Format the size in a human-readable format
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double formattedSize = size;

            while (formattedSize >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                formattedSize /= 1024;
                suffixIndex++;
            }

            string formattedSizeWithUnit = $"{formattedSize:0.##} {suffixes[suffixIndex]}";
            string sizeInBytes = $"{size} bytes";
            if (withBytes == true)
                return $"{formattedSizeWithUnit} ({sizeInBytes})";
            return $"{formattedSize:0.##} {suffixes[suffixIndex]}";

        }
    }
}
