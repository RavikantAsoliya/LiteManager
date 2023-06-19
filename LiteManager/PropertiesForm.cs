using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteManager.Helper;
using Microsoft.Win32;

namespace LiteManager
{
    public partial class PropertiesForm : Form
    {
        public Stopwatch stopwatch1;
        private readonly List<string> ListOfFilesAndFolders;
        private CancellationTokenSource cancellationTokenSource;
        long fullTotalSize = 0;


        public PropertiesForm(List<string> ListOfFilesAndFolders)
        {
            InitializeComponent();
            this.ListOfFilesAndFolders = ListOfFilesAndFolders;
            typeTextBox.BorderStyle = locationTextBox.BorderStyle = sizeTextBox.BorderStyle = sizeOnDiskTextBox.BorderStyle = BorderStyle.None;
        }

        private async void TestForm_LoadAsync(object sender, EventArgs e)
        {


            hiddenCheckBox.CheckState = GetHiddenAttributeCheckState(ListOfFilesAndFolders);
            readOnlyCheckBox.CheckState = GetReadOnlyAttributeCheckState(ListOfFilesAndFolders);


            ListOfFilesAndFolders.All(file => new FileInfo(file).Attributes == FileAttributes.Hidden);
            countLabel.Text = $"{ListOfFilesAndFolders.Count(File.Exists)} Files, {ListOfFilesAndFolders.Count(Directory.Exists)} Folders";

            typeTextBox.Text = FileTypeChecker.GetFileType(ListOfFilesAndFolders);

            locationTextBox.Text = Path.GetDirectoryName(ListOfFilesAndFolders.FirstOrDefault());
            await GetSizeOfFileAndFolder();
        }


        private async Task GetSizeOfFileAndFolder()
        {
            foreach (string itemPath in ListOfFilesAndFolders) //listOfFolders
            {
                try
                {
                    if (File.Exists(itemPath))
                    {
                        FileInfo fileInfo = new FileInfo(itemPath);
                        fullTotalSize += fileInfo.Length;
                        sizeTextBox.Text = SizeFormatter.FormatSize(fullTotalSize, true);
                    }
                    else if (Directory.Exists(itemPath))
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        CancellationToken cancellationToken = cancellationTokenSource.Token;
                        long folderSize = await GetItemSize(itemPath, cancellationToken);
                        fullTotalSize += folderSize;
                        sizeTextBox.Text = SizeFormatter.FormatSize(fullTotalSize, true);
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception if necessary
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }








        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel the ongoing folder size calculation if it's running
            cancellationTokenSource?.Cancel();
        }


        private async Task<long> GetItemSize(string itemPath, CancellationToken cancellationToken)
        {
            long totalSize = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();

            await Task.Run(() =>
            {
                try
                {
                    Parallel.ForEach(Directory.EnumerateFiles(itemPath, "*", SearchOption.AllDirectories),
                        new ParallelOptions { CancellationToken = cancellationToken }, file =>
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                // Handle cancellation if necessary
                                Console.WriteLine("Calculation canceled.");
                                return;
                            }

                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                Interlocked.Add(ref totalSize, fileInfo.Length);

                                if (stopwatch.ElapsedMilliseconds > 10 && sizeTextBox.IsHandleCreated)
                                {
                                    stopwatch.Restart();

                                    sizeTextBox.Invoke(new Action(() =>
                                    {
                                        fullTotalSize += totalSize;
                                        totalSize = 0;
                                        sizeTextBox.Text = SizeFormatter.FormatSize(fullTotalSize, true);
                                    }));
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle the exception if necessary
                                Console.WriteLine($"Error: {ex.Message}");
                                return;
                            }
                        });

                }
                catch (Exception)
                {
                    // Handle cancellation if necessary
                    Console.WriteLine("Calculation canceled.");
                    return;
                }
            }, cancellationToken);

            if (sizeTextBox.IsHandleCreated)
            {
                sizeTextBox.Invoke(new Action(() =>
                {
                    sizeTextBox.Text = SizeFormatter.FormatSize(fullTotalSize, true);
                }));
            }

            return totalSize;
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Cancel the ongoing folder size calculation if it's running
            cancellationTokenSource?.Cancel();
            this.Close();
        }


        private void HiddenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
        }


        private void OkButton_Click(object sender, EventArgs e)
        {
            SetHiddenAttribute(ListOfFilesAndFolders, hiddenCheckBox.Checked);
            SetReadOnlyAttribute(ListOfFilesAndFolders, readOnlyCheckBox.Checked);
            // Cancel the ongoing folder size calculation if it's running
            cancellationTokenSource?.Cancel();
            this.Close();
        }


        public CheckState GetHiddenAttributeCheckState(List<string> filesAndFolders)
        {
            bool allHidden = filesAndFolders.All(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.Hidden) == FileAttributes.Hidden) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));

            bool anyHidden = filesAndFolders.Any(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.Hidden) == FileAttributes.Hidden) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));

            return allHidden ? CheckState.Checked : anyHidden ? CheckState.Indeterminate : CheckState.Unchecked;
        }


        public CheckState GetReadOnlyAttributeCheckState(List<string> filesAndFolders)
        {
            bool allReadOnly = filesAndFolders.All(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly));

            bool anyReadOnly = filesAndFolders.Any(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly));

            return allReadOnly ? CheckState.Checked :
                   anyReadOnly ? CheckState.Indeterminate :
                   CheckState.Unchecked;
        }


        private void SetHiddenAttribute(List<string> filesAndFolders, bool isChecked)
        {
            filesAndFolders.ForEach(itemPath =>
            {
                if (File.Exists(itemPath))
                {
                    FileAttributes attributes = File.GetAttributes(itemPath);
                    attributes = isChecked ? attributes | FileAttributes.Hidden : attributes & ~FileAttributes.Hidden;
                    File.SetAttributes(itemPath, attributes);
                }
                else if (Directory.Exists(itemPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(itemPath);
                    directoryInfo.Attributes = isChecked ? directoryInfo.Attributes | FileAttributes.Hidden : directoryInfo.Attributes & ~FileAttributes.Hidden;
                }
            });
        }


        private void SetReadOnlyAttribute(List<string> filesAndFolders, bool isChecked)
        {
            filesAndFolders.ForEach(itemPath =>
            {
                if (File.Exists(itemPath))
                {
                    FileAttributes attributes = File.GetAttributes(itemPath);
                    attributes = isChecked ? attributes | FileAttributes.ReadOnly : attributes & ~FileAttributes.ReadOnly;
                    File.SetAttributes(itemPath, attributes);
                }
                else if (Directory.Exists(itemPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(itemPath);
                    directoryInfo.Attributes = isChecked ? directoryInfo.Attributes | FileAttributes.ReadOnly : directoryInfo.Attributes & ~FileAttributes.ReadOnly;
                }
            });
        }


        private void ApplyButton_Click(object sender, EventArgs e)
        {
            SetHiddenAttribute(ListOfFilesAndFolders, hiddenCheckBox.Checked);
            SetReadOnlyAttribute(ListOfFilesAndFolders, readOnlyCheckBox.Checked);
            applyButton.Enabled = false;
        }

    }
}
