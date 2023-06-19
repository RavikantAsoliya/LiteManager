using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiteManager.Helper
{
    internal class SizeManager
    {

        /// <summary>
        /// Formats the given size into a human-readable format.
        /// </summary>
        /// <param name="size">The size to be formatted.</param>
        /// <param name="withBytes">Optional. Determines whether to include the size in bytes.</param>
        /// <returns>The formatted size string.</returns>
        public static string FormatSize(long size, bool withBytes = false)
        {
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double formattedSize = size;

            // Convert the size into a human-readable format
            while (formattedSize >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                formattedSize /= 1024;
                suffixIndex++;
            }

            // Construct the formatted size string with the appropriate unit
            string formattedSizeWithUnit = $"{formattedSize:0.##} {suffixes[suffixIndex]}";
            string sizeInBytes = $"{size} bytes";

            // Optionally include the size in bytes
            return withBytes ? $"{formattedSizeWithUnit} ({sizeInBytes})" : formattedSizeWithUnit;
        }


        /// <summary>
        /// Retrieves the size of a folder asynchronously.
        /// </summary>
        /// <param name="folder">The path to the folder.</param>
        /// <returns>A Task representing the asynchronous operation with the folder size formatted as a string.</returns>
        public static async Task<string> GetSize(string folder)
        {
            long totalSize = 0;
            await Task.Run(() =>
            {
                try
                {
                    // Iterate over all files in the folder and its subdirectories in parallel
                    Parallel.ForEach(Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories),
                        new ParallelOptions { }, file =>
                        {
                            try
                            {
                                // Get the size of each file and add it to the totalSize variable
                                FileInfo fileInfo = new FileInfo(file);
                                Interlocked.Add(ref totalSize, fileInfo.Length);
                            }
                            catch (Exception ex)
                            {
                                // Handle any exceptions that occur during file processing
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        });
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during folder enumeration
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });

            // Format the totalSize as a string representing the folder size
            return SizeManager.FormatSize(totalSize);
        }

    }
}
