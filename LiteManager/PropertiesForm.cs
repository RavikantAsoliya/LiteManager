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

    /// <summary>
    /// Represents a form that displays properties of files and folders.
    /// </summary>
    public partial class PropertiesForm : Form
    {

        /// <summary>
        /// The list of files and folders to display properties for.
        /// </summary>
        private readonly List<string> ListOfFilesAndFolders;

        /// <summary>
        /// The CancellationTokenSource used to cancel the ongoing folder size calculation.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// The total size of all files and folders.
        /// </summary>
        long fullTotalSize = 0;


        /// <summary>
        /// Initializes a new instance of the PropertiesForm class.
        /// </summary>
        /// <param name="ListOfFilesAndFolders">The list of files and folders.</param>
        public PropertiesForm(List<string> ListOfFilesAndFolders)
        {
            InitializeComponent();

            // Set the ListOfFilesAndFolders property
            this.ListOfFilesAndFolders = ListOfFilesAndFolders;

            // Set the border style of the text boxes to None
            typeTextBox.BorderStyle = locationTextBox.BorderStyle = sizeTextBox.BorderStyle = sizeOnDiskTextBox.BorderStyle = BorderStyle.None;
        }


        /// <summary>
        /// Load Properties Form with Type, Size, etc...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PropertiesForm_LoadAsync(object sender, EventArgs e)
        {
            // Set the check state of the hidden checkbox based on the attributes of the files and folders
            hiddenCheckBox.CheckState = GetHiddenAttributeCheckState(ListOfFilesAndFolders);

            // Set the check state of the read-only checkbox based on the attributes of the files and folders
            readOnlyCheckBox.CheckState = GetReadOnlyAttributeCheckState(ListOfFilesAndFolders);

            // Check if all files have the hidden attribute set
            ListOfFilesAndFolders.All(file => new FileInfo(file).Attributes == FileAttributes.Hidden);

            // Set the text of the count label to display the number of files and folders
            countLabel.Text = $"{ListOfFilesAndFolders.Count(File.Exists)} Files, {ListOfFilesAndFolders.Count(Directory.Exists)} Folders";

            // Get the file type for the files and folders and set it in the type textbox
            typeTextBox.Text = FileTypeChecker.GetFileType(ListOfFilesAndFolders);

            // Get the directory path of the first file or folder and set it in the location textbox
            locationTextBox.Text = Path.GetDirectoryName(ListOfFilesAndFolders.FirstOrDefault());

            // Calculate and display the size of the files and folders asynchronously
            await GetSizeOfFileAndFolder();
        }


        /// <summary>
        /// Gets the size of the files and folders asynchronously.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task GetSizeOfFileAndFolder()
        {
            // Iterate through each item path in the list of files and folders
            foreach (string itemPath in ListOfFilesAndFolders) //listOfFolders
            {
                try
                {
                    // Check if the item is a file
                    if (File.Exists(itemPath))
                    {
                        // Get information about the file
                        FileInfo fileInfo = new FileInfo(itemPath);

                        // Add the file size to the total size
                        fullTotalSize += fileInfo.Length;

                        // Display the formatted size in the size textbox
                        sizeTextBox.Text = SizeManager.FormatSize(fullTotalSize, true);
                    }
                    // Check if the item is a directory
                    else if (Directory.Exists(itemPath))
                    {
                        // Create a new cancellation token source for the folder size calculation
                        cancellationTokenSource = new CancellationTokenSource();

                        // Get the cancellation token from the cancellation token source
                        CancellationToken cancellationToken = cancellationTokenSource.Token;

                        // Calculate the size of the directory asynchronously
                        long folderSize = await GetItemSize(itemPath, cancellationToken);

                        // Add the folder size to the total size
                        fullTotalSize += folderSize;

                        // Display the formatted size in the size textbox
                        sizeTextBox.Text = SizeManager.FormatSize(fullTotalSize, true);
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception if necessary
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Close The Properties Form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertiesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel the ongoing folder size calculation if it's running
            cancellationTokenSource?.Cancel();
        }


        /// <summary>
        /// Gets the size of Folder asynchronously.
        /// </summary>
        /// <param name="itemPath">The path of the item.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The total size of the item.</returns>
        private async Task<long> GetItemSize(string itemPath, CancellationToken cancellationToken)
        {
            // Initialize the total size
            long totalSize = 0;

            // Create a stopwatch to measure the elapsed time
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Run the folder size calculation asynchronously
            await Task.Run(() =>
            {
                try
                {
                    // Enumerate all files in the specified directory and its subdirectories in parallel
                    Parallel.ForEach(Directory.EnumerateFiles(itemPath, "*", SearchOption.AllDirectories),
                        new ParallelOptions { CancellationToken = cancellationToken }, file =>
                        {
                            // Check if cancellation has been requested
                            if (cancellationToken.IsCancellationRequested)
                            {
                                // Handle cancellation if necessary
                                Console.WriteLine("Calculation canceled.");
                                return;
                            }

                            try
                            {
                                // Get information about the file
                                FileInfo fileInfo = new FileInfo(file);

                                // Add the file size to the total size atomically
                                Interlocked.Add(ref totalSize, fileInfo.Length);

                                // Check if enough time has elapsed and the size textbox is accessible
                                if (stopwatch.ElapsedMilliseconds > 10 && sizeTextBox.IsHandleCreated)
                                {
                                    stopwatch.Restart();

                                    // Update the size textbox in the UI thread
                                    sizeTextBox.Invoke(new Action(() =>
                                    {
                                        // Add the accumulated total size to the overall total size
                                        fullTotalSize += totalSize;

                                        // Reset the total size counter
                                        totalSize = 0;

                                        // Display the formatted size in the size textbox
                                        sizeTextBox.Text = SizeManager.FormatSize(fullTotalSize, true);
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

            // Update the size textbox with the final total size if it is accessible
            if (sizeTextBox.IsHandleCreated)
            {
                sizeTextBox.Invoke(new Action(() =>
                {
                    sizeTextBox.Text = SizeManager.FormatSize(fullTotalSize, true);
                }));
            }

            // Return the total size
            return totalSize;
        }


        /// <summary>
        /// It cancels the ongoing folder size calculation, if any, and closes the properties form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Cancel the ongoing folder size calculation if it's running
            cancellationTokenSource?.Cancel();
            // Close Form
            this.Close();
        }


        /// <summary>
        /// when the state of the hidden checkbox changed. It enables the apply button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiddenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Enable the applyButton when the checked state of the hiddenCheckBox changes
            applyButton.Enabled = true;
        }


        /// <summary>
        /// It applies the changes made to the hidden and read-only attributes of files and folders.And closes the Properties Form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            // Set the hidden attribute of files and folders based on the checked state of hiddenCheckBox
            SetHiddenAttribute(ListOfFilesAndFolders, hiddenCheckBox.Checked);

            // Set the read-only attribute of files and folders based on the checked state of readOnlyCheckBox
            SetReadOnlyAttribute(ListOfFilesAndFolders, readOnlyCheckBox.Checked);

            // Cancel the ongoing folder size calculation if it's running by cancelling the cancellationTokenSource
            cancellationTokenSource?.Cancel();

            // Close the form
            this.Close();
        }


        /// <summary>
        /// Gets the CheckState for the hidden attribute of files and folders.
        /// </summary>
        /// <param name="filesAndFolders">The list of files and folders.</param>
        /// <returns>The CheckState for the hidden attribute.</returns>
        public CheckState GetHiddenAttributeCheckState(List<string> filesAndFolders)
        {
            // Check if all items in the list are hidden
            bool allHidden = filesAndFolders.All(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.Hidden) == FileAttributes.Hidden) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));

            // Check if any item in the list is hidden
            bool anyHidden = filesAndFolders.Any(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.Hidden) == FileAttributes.Hidden) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden));

            // Return the appropriate CheckState based on the hidden attribute
            return allHidden ? CheckState.Checked : anyHidden ? CheckState.Indeterminate : CheckState.Unchecked;
        }


        /// <summary>
        /// Gets the CheckState for the read-only attribute of files and folders.
        /// </summary>
        /// <param name="filesAndFolders">The list of files and folders.</param>
        /// <returns>The CheckState for the read-only attribute.</returns>
        public CheckState GetReadOnlyAttributeCheckState(List<string> filesAndFolders)
        {
            // Check if all items in the list are read-only
            bool allReadOnly = filesAndFolders.All(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly));

            // Check if any item in the list is read-only
            bool anyReadOnly = filesAndFolders.Any(itemPath =>
                (File.Exists(itemPath) && (File.GetAttributes(itemPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) ||
                (Directory.Exists(itemPath) && (new DirectoryInfo(itemPath).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly));

            // Return the appropriate CheckState based on the read-only attribute
            return allReadOnly ? CheckState.Checked :
                   anyReadOnly ? CheckState.Indeterminate :
                   CheckState.Unchecked;
        }


        /// <summary>
        /// Sets the hidden attribute of files and folders.
        /// </summary>
        /// <param name="filesAndFolders">The list of files and folders.</param>
        /// <param name="isChecked">A flag indicating whether the hidden attribute should be set or cleared.</param>
        private void SetHiddenAttribute(List<string> filesAndFolders, bool isChecked)
        {
            // Loop through each item in the filesAndFolders list
            filesAndFolders.ForEach(itemPath =>
            {
                if (File.Exists(itemPath))
                {
                    // If the item is a file, get its attributes
                    FileAttributes attributes = File.GetAttributes(itemPath);

                    // Modify the attributes based on the isChecked value
                    attributes = isChecked ? attributes | FileAttributes.Hidden : attributes & ~FileAttributes.Hidden;

                    // Set the modified attributes for the file
                    File.SetAttributes(itemPath, attributes);
                }
                else if (Directory.Exists(itemPath))
                {
                    // If the item is a directory, get its DirectoryInfo object
                    DirectoryInfo directoryInfo = new DirectoryInfo(itemPath);

                    // Modify the attributes based on the isChecked value
                    directoryInfo.Attributes = isChecked ? directoryInfo.Attributes | FileAttributes.Hidden : directoryInfo.Attributes & ~FileAttributes.Hidden;
                }
            });
        }


        /// <summary>
        /// Sets the read-only attribute of files and folders.
        /// </summary>
        /// <param name="filesAndFolders">The list of files and folders.</param>
        /// <param name="isChecked">A flag indicating whether the read-only attribute should be set or cleared.</param>
        private void SetReadOnlyAttribute(List<string> filesAndFolders, bool isChecked)
        {
            // Loop through each item in the filesAndFolders list
            filesAndFolders.ForEach(itemPath =>
            {
                if (File.Exists(itemPath))
                {
                    // If the item is a file, get its attributes
                    FileAttributes attributes = File.GetAttributes(itemPath);

                    // Modify the attributes based on the isChecked value
                    attributes = isChecked ? attributes | FileAttributes.ReadOnly : attributes & ~FileAttributes.ReadOnly;

                    // Set the modified attributes for the file
                    File.SetAttributes(itemPath, attributes);
                }
                else if (Directory.Exists(itemPath))
                {
                    // If the item is a directory, get its DirectoryInfo object
                    DirectoryInfo directoryInfo = new DirectoryInfo(itemPath);

                    // Modify the attributes based on the isChecked value
                    directoryInfo.Attributes = isChecked ? directoryInfo.Attributes | FileAttributes.ReadOnly : directoryInfo.Attributes & ~FileAttributes.ReadOnly;
                }
            });
        }


        /// <summary>
        /// It applies the changes made to the hidden and read-only attributes of files and folders without closing the properties form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            // Set the hidden attribute of files and folders
            SetHiddenAttribute(ListOfFilesAndFolders, hiddenCheckBox.Checked);

            // Set the read-only attribute of files and folders
            SetReadOnlyAttribute(ListOfFilesAndFolders, readOnlyCheckBox.Checked);

            // Disable the "Apply" button
            applyButton.Enabled = false;
        }


    }
}
