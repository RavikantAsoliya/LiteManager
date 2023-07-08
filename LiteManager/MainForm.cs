using Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteManager.Helper;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing.Drawing2D;
using LiteManager;
using System.IO.Compression;

namespace LiteManager
{

    public partial class MainForm : Form
    {
        #region Variables

        private string currentDirectory = "recent";
        private readonly List<string> clipboardPathList = new List<string>();
        private bool isCutOperation = false;
        private readonly Stack<string> backStack = new Stack<string>();
        private readonly Stack<string> forwardStack = new Stack<string>();
        private readonly Bitmap noPreviewImage; // Default "No preview available" image

        #endregion

        public MainForm()
        {
            InitializeComponent();
            // Create the "No preview available" image
            noPreviewImage = GenerateNoPreviewImage();
        }

        public enum IconIndex
        {
            Fixed = 0, // Represents the image index for Local disk drives
            CDRom = 1, // Represents the image index for Optical disk drives
            RemovableDisk = 2, // Represents the image index for Removable disk drives
            Folder = 3, // Represents the image index for folders
            Recent = 4 // Represents the image index for Recents
        }

        #region Additional Methods


        /// <summary>
        /// Populates the tree view with drive nodes representing available drives on the system.
        /// Each drive node is assigned an image index based on its drive type and labeled with the drive's name.
        /// The function also populates child nodes for each drive node representing subdirectories of the corresponding drive.
        /// </summary>
        /// <remarks>
        /// It will display all Drives with their directories on the Left side in TreeView.
        /// If there are any unauthorized access exceptions while retrieving the subdirectories, an "Access Denied" node is added for the drive.
        /// </remarks>
        private void PopulateTreeView()
        {

            TreeNode recentFilesNode = driveTreeView.Nodes.Add("Recents");  // Creates a new TreeNode and adds it to the driveTreeView
            recentFilesNode.Tag = "recent";  // Sets the Tag property of the TreeNode to "recent"
            recentFilesNode.ImageIndex = (int)IconIndex.Recent;  // Sets the ImageIndex property of the TreeNode to the index representing the "Recent" icon
            recentFilesNode.SelectedImageIndex = (int)IconIndex.Recent;  // Sets the SelectedImageIndex property of the TreeNode to the index representing the "Recent" icon


            foreach (DriveInfo drive in DriveInfo.GetDrives()) // Iterate over the available drives
            {
                if (drive.IsReady) // Check if the drive is ready
                {
                    IconIndex imageIndex; // Variable to hold the image index for the drive node

                    switch (drive.DriveType) // Determine the drive type
                    {
                        case DriveType.Removable:
                            imageIndex = IconIndex.RemovableDisk; // Set image index to RemovableDisk for removable drives
                            break;
                        case DriveType.Fixed:
                            imageIndex = IconIndex.Fixed; // Set image index to Fixed for fixed drives
                            break;
                        case DriveType.CDRom:
                            imageIndex = IconIndex.CDRom; // Set image index to CDRom for CD-ROM drives
                            break;
                        default:
                            imageIndex = IconIndex.Fixed; // Set image index to Fixed for other drive types
                            break;
                    }

                    TreeNode driveNode = new TreeNode(GetDriveLabel(drive)) // Create a drive node with the drive's label
                    {
                        Tag = drive.RootDirectory, // Assign the drive's root directory as the node's tag
                        ImageIndex = (int)imageIndex, // Assign the image index to the node
                        SelectedImageIndex = (int)imageIndex // Assign the same image index to the selected node appearance
                    };

                    driveTreeView.Nodes.Add(driveNode); // Add the drive node to the tree view control
                    PopulateChildNodes(driveNode); // Populate child nodes for the drive node
                }
            }
        }


        /// <summary>
        /// Generates a label for a given drive based on its type.
        /// </summary>
        /// <param name="drive">The DriveInfo object for which to generate the label.</param>
        /// <returns>The generated drive label.</returns>
        private string GetDriveLabel(DriveInfo drive)
        {
            string driveTypeLabel = ""; // Initialize the drive type label variable

            switch (drive.DriveType) // Check the drive type of the DriveInfo object
            {
                case DriveType.Removable:
                    driveTypeLabel = "Removable Disk"; // Set the label to "Removable Disk" for Removable drive type
                    break;
                case DriveType.Fixed:
                    driveTypeLabel = "Local Disk"; // Set the label to "Local Disk" for Fixed drive type
                    break;
                case DriveType.CDRom:
                    driveTypeLabel = "Optical Disk"; // Set the label to "Optical Disk" for CDRom drive type
                    break;
            }

            return $"{driveTypeLabel} ({drive.Name.Replace("\\", "")})"; // Generate the drive label by combining the drive type label and drive name
        }


        /// <summary>
        /// Adds the child directories of a parent node in the DriveTreeView control.
        /// </summary>
        /// <param name="parentNode">The parent node to add the child directories to.</param>
        private void PopulateChildNodes(TreeNode parentNode)
        {
            // Clear the existing child nodes
            parentNode.Nodes.Clear();
            // Retrieve the directory associated with the parent node
            var directory = (DirectoryInfo)parentNode.Tag;
            try
            {
                // Iterate over the subdirectories of the directory
                foreach (var subDirectory in directory.GetDirectories())
                {
                    // Create a child node for each subdirectory
                    var childNode = new TreeNode(subDirectory.Name)
                    {
                        Tag = subDirectory,
                        ImageIndex = (int)IconIndex.Folder,
                        SelectedImageIndex = (int)IconIndex.Folder,
                    };
                    // Add the child node to the parent node
                    parentNode.Nodes.Add(childNode);
                    // Add a placeholder node for each child node
                    childNode.Nodes.Add("Loading...");
                }
            }
            catch (UnauthorizedAccessException)
            {
                // If access to the directory is denied, add a node indicating the access denial
                parentNode.Nodes.Add("Access Denied");
            }
        }


        /// <summary>
        /// Populates the ListView with the directories and files from the specified path.
        /// Obtains the directories and files within the specified path.
        /// Stores the file information in a dictionary and assigns it to the ListViewItem's tag.
        /// Updates the status label with the total count of folders and files.
        /// Displays an error message if an exception occurs.
        /// </summary>
        /// <param name="path">The path from which to populate the ListView.</param>
        public void PopulateListView(string path)
        {
            //start data update
            fileListView.BeginUpdate();

            //clear listview
            fileListView.Items.Clear();

            if (path == "recent")
            {
                // Get recent files
                var recentFiles = RecentFiles.GetRecentFiles();

                foreach (string file in recentFiles)
                {
                    if (File.Exists(file))
                    {
                        // File exists
                        FileInfo fileInfo = new FileInfo(file);

                        // Add file to list view
                        ListViewItem item = fileListView.Items.Add(fileInfo.Name);

                        if (fileInfo.Extension == ".exe" || fileInfo.Extension == "")
                        {
                            // Get file icon for executable or unknown file type
                            Icon fileIcon = IconProvider.GetIconByFileName(fileInfo.FullName);

                            // Add image to image list
                            imageList.Images.Add(fileInfo.Name, fileIcon);

                            // Set image key for the item
                            item.ImageKey = fileInfo.Name;
                        }
                        else
                        {
                            if (!imageList.Images.ContainsKey(fileInfo.Extension))
                            {
                                // Get file icon for other file types
                                Icon fileIcon = IconProvider.GetIconByFileName(fileInfo.FullName);

                                // Add image to image list
                                imageList.Images.Add(fileInfo.Extension, fileIcon);
                            }

                            // Set image key for the item
                            item.ImageKey = fileInfo.Extension;
                        }

                        // Set tag for the item
                        item.Tag = new Dictionary<string, string>
                {
                    {"FullName", fileInfo.FullName},
                    {"Type", "File" }
                };

                        // Set tooltip for the item
                        item.ToolTipText = $"Type: {FileTypeChecker.GetFileTypeByExtension(fileInfo.Extension.ToString())}\n" +
                            $"Size: {SizeManager.FormatSize(fileInfo.Length)}\n" +
                            $"Date Modified: {fileInfo.LastWriteTime}";

                        // Add sub-items
                        item.SubItems.Add(FileTypeChecker.GetFileTypeByExtension(fileInfo.Extension));
                        item.SubItems.Add($"{SizeManager.FormatSize(fileInfo.Length)}");
                        item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                    }
                    else if (Directory.Exists(file))
                    {
                        // Directory exists
                        DirectoryInfo dirInfo = new DirectoryInfo(file);

                        // Add directory to list view
                        ListViewItem item = fileListView.Items.Add(dirInfo.Name, 3);

                        // Set tag for the item
                        item.Tag = new Dictionary<string, string>
                {
                    {"FullName", dirInfo.FullName},
                    {"Type", "Folder" }
                };

                        // Add sub-items
                        item.SubItems.Add("Folder");
                        item.SubItems.Add("");
                        item.SubItems.Add(dirInfo.LastWriteTime.ToString());
                    }
                }
            }
            else
            {
                try
                {
                    // Get directories and files in the specified path
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    // Remove existing executable icons from image list
                    foreach (ListViewItem item in fileListView.Items)
                    {
                        if (item.Text.EndsWith(".exe"))
                        {
                            imageList.Images.RemoveByKey(item.Text);
                        }
                    }

                    // Add directories to list view
                    foreach (DirectoryInfo dirInfo in directoryInfos)
                    {
                        ListViewItem item = fileListView.Items.Add(dirInfo.Name, 3);

                        // Set tag for the item
                        item.Tag = new Dictionary<string, string>
                {
                    {"FullName", dirInfo.FullName},
                    {"Type", "Folder" }
                };

                        // Add sub-items
                        item.SubItems.Add("Folder");
                        item.SubItems.Add("");
                        item.SubItems.Add(dirInfo.LastWriteTime.ToString());
                    }

                    // Add files to list view
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        ListViewItem item = fileListView.Items.Add(fileInfo.Name);

                        if (fileInfo.Extension == ".exe" || fileInfo.Extension == "")
                        {
                            // Get file icon for executable or unknown file type
                            Icon fileIcon = IconProvider.GetIconByFileName(fileInfo.FullName);

                            // Add image to image list
                            imageList.Images.Add(fileInfo.Name, fileIcon);

                            // Set image key for the item
                            item.ImageKey = fileInfo.Name;
                        }
                        else
                        {
                            if (!imageList.Images.ContainsKey(fileInfo.Extension))
                            {
                                // Get file icon for other file types
                                Icon fileIcon = IconProvider.GetIconByFileName(fileInfo.FullName);

                                // Add image to image list
                                imageList.Images.Add(fileInfo.Extension, fileIcon);
                            }

                            // Set image key for the item
                            item.ImageKey = fileInfo.Extension;
                        }

                        // Set tag for the item
                        item.Tag = new Dictionary<string, string>
                {
                    {"FullName", fileInfo.FullName},
                    {"Type", "File" }
                };

                        // Set tooltip for the item
                        item.ToolTipText = $"Type: {FileTypeChecker.GetFileTypeByExtension(fileInfo.Extension.ToString())}\n" +
                            $"Size: {SizeManager.FormatSize(fileInfo.Length)}\n" +
                            $"Date Modified: {fileInfo.LastWriteTime}";

                        // Add sub-items
                        item.SubItems.Add(FileTypeChecker.GetFileTypeByExtension(fileInfo.Extension));
                        item.SubItems.Add(SizeManager.FormatSize(fileInfo.Length));
                        item.SubItems.Add(fileInfo.LastWriteTime.ToString());
                    }
                }
                catch (Exception e)
                {
                    // Display error message
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Set count status label
            countToolStripStatusLabel.Text = fileListView.Items.Count == 1
               ? $"{fileListView.Items.Count} item   |"
               : $"{fileListView.Items.Count} items   |";

            // Enable ToolTip
            fileListView.ShowItemToolTips = true;
            // End Data Update
            fileListView.EndUpdate();
        }


        /// <summary>
        /// Navigates to the specified directory path. It updates the current directory, manages the backStack and forwardStack, and populates the ListView with the contents of the new directory.
        /// </summary>
        /// <param name="path">The directory path to navigate to.</param>
        private void NavigateToDirectory(string path)
        {
            if (!string.IsNullOrEmpty(currentDirectory))
            {
                backStack.Push(currentDirectory);  // Push the current directory path to the backStack
                backButton.Enabled = true;  // Enable the back button
            }

            currentDirectory = path;  // Set the current directory to the specified path
            forwardStack.Clear();  // Clear the forwardStack
            forwardButton.Enabled = false;  // Disable the forward button

            PopulateListView(currentDirectory);  // Populate the ListView with the contents of the current directory
        }


        /// <summary>
        /// Recursively copies a directory and its contents from the sourcePath to the targetPath.
        /// </summary>
        /// <param name="sourcePath">The path of the source directory to copy.</param>
        /// <param name="targetPath">The path of the target directory where the source directory and its contents will be copied.</param>
        private void CopyDirectory(string sourcePath, string targetPath)
        {
            // Create the target directory if it doesn't exist
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            // Copy files from the source directory to the target directory
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetPath, fileName);
                File.Copy(file, destFile, true);
            }

            // Recursively copy subdirectories from the source directory to the target directory
            foreach (string subdirectory in Directory.GetDirectories(sourcePath))
            {
                string directoryName = Path.GetFileName(subdirectory);
                string destDirectory = Path.Combine(targetPath, directoryName);
                CopyDirectory(subdirectory, destDirectory);
            }
        }


        /// <summary>
        /// Checks if the given filename is valid and does not contain any illegal characters.
        /// </summary>
        /// <param name="filename">The filename to validate.</param>
        /// <returns>True if the filename is valid; otherwise, false.</returns>
        private bool IsFilenameValid(string filename)
        {
            // Define the array of illegal characters
            char[] illegalChars = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };

            // Iterate over the illegal characters
            foreach (char illegalChar in illegalChars)
            {
                // Check if the filename contains the illegal character
                if (filename.Contains(illegalChar))
                {
                    // Display an error message with the list of illegal characters
                    MessageBox.Show("Filenames cannot contain any of the following characters:\r\n" + "\t\\/:*?\"<>|", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // The filename is valid
            return true;
        }


        /// <summary>
        /// Changes the view mode of the ListView and updates the checked state of the corresponding ToolStripMenuItems.
        /// </summary>
        /// <param name="view">The desired View mode to set for the ListView.</param>
        private void ChangeListViewView(View view)
        {
            largeIconsToolStripMenuItem.Checked = false;  // Uncheck the Large Icons ToolStripMenuItem
            smallIconsToolStripMenuItem.Checked = false;  // Uncheck the Small Icons ToolStripMenuItem
            detailedInfoToolStripMenuItem.Checked = false;  // Uncheck the Detailed Info ToolStripMenuItem
            tileToolStripMenuItem.Checked = false;  // Uncheck the Tile ToolStripMenuItem
            listToolStripMenuItem.Checked = false;  // Uncheck the List ToolStripMenuItem

            fileListView.View = view;  // Set the view mode of the ListView control

            // Set the checked state of the corresponding ToolStripMenuItem based on the view mode
            switch (view)
            {
                case View.LargeIcon:
                    largeIconsToolStripMenuItem.Checked = true;  // Check the Large Icons ToolStripMenuItem
                    break;
                case View.SmallIcon:
                    smallIconsToolStripMenuItem.Checked = true;  // Check the Small Icons ToolStripMenuItem
                    break;
                case View.Details:
                    detailedInfoToolStripMenuItem.Checked = true;  // Check the Detailed Info ToolStripMenuItem
                    break;
                case View.Tile:
                    tileToolStripMenuItem.Checked = true;  // Check the Tile ToolStripMenuItem
                    break;
                case View.List:
                    listToolStripMenuItem.Checked = true;  // Check the List ToolStripMenuItem
                    break;
            }
        }


        #endregion


        #region Preview Pane


        /// <summary>
        /// Checks if the given file extension corresponds to a video file.
        /// </summary>
        /// <param name="fileExtension">The file extension to check.</param>
        /// <returns>True if the file extension represents a video file, false otherwise.</returns>
        private bool IsVideoFile(string fileExtension)
        {
            // Define an array of video file extensions
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".mkv" };

            // Check if the given file extension is present in the videoExtensions array
            // The StringComparer.OrdinalIgnoreCase parameter ensures case-insensitive comparison
            return videoExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }


        /// <summary>
        /// Checks if the given file extension corresponds to an image file.
        /// </summary>
        /// <param name="fileExtension">The file extension to check.</param>
        /// <returns>True if the file extension represents an image file, false otherwise.</returns>
        private bool IsImageFile(string fileExtension)
        {
            // Define an array of image file extensions
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            // Check if the given file extension is present in the imageExtensions array
            // The StringComparer.OrdinalIgnoreCase parameter ensures case-insensitive comparison
            return imageExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }


        //TODO: Add document preview
        /// <summary>
        /// Displays a preview of the file at the specified path in the picture box.
        /// </summary>
        /// <param name="path">The path of the file to display the preview for.</param>
        private void DisplayPreview(string path)
        {
            // Clear the current image in the picture box
            pictureBoxPreview.Image = null;

            // Check if the file exists at the specified path
            if (File.Exists(path))
            {
                // Check if the file is a video file based on its extension
                if (IsVideoFile(Path.GetExtension(path)))
                {
                    // Generate a video thumbnail and display it in the picture box
                    GenerateVideoThumbnail(path);
                }
                // Check if the file is an image file based on its extension
                else if (IsImageFile(Path.GetExtension(path)))
                {
                    // Generate an image preview and display it in the picture box
                    GenerateImagePreview(path);
                }
                else
                {
                    // The file is neither a video nor an image file, display a default "no preview" image
                    pictureBoxPreview.Image = noPreviewImage;
                }
            }
        }


        /// <summary>
        /// Generates an image preview from the specified image file and displays it in the picture box.
        /// </summary>
        /// <param name="imagePath">The path of the image file.</param>
        private void GenerateImagePreview(string imagePath)
        {
            try
            {
                // Open the image file for reading using a FileStream
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    // Create an Image object from the stream and assign it to the picture box
                    pictureBoxPreview.Image = Image.FromStream(fs);

                    // Check if the image width is greater than the PictureBox width
                    if (pictureBoxPreview.Image.Width > pictureBoxPreview.Width)
                    {
                        // If the image is wider than the PictureBox, set the PictureBoxSizeMode to Zoom
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // If the image is smaller or equal to the PictureBox, set the PictureBoxSizeMode to CenterImage
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                }
            }
            catch (Exception)
            {
                // Ignore any errors that occur during image generation
            }
        }


        /// <summary>
        /// Generates a thumbnail image for the specified video file and displays it in the picture box.
        /// </summary>
        /// <param name="videoPath">The path of the video file.</param>
        private void GenerateVideoThumbnail(string videoPath)
        {
            try
            {
                // Create a ShellFile instance from the video file path
                ShellFile shellFile = ShellFile.FromFilePath(videoPath);

                // Retrieve the thumbnail bitmap from the ShellFile
                Bitmap thumbnail = shellFile.Thumbnail.Bitmap;

                // Assign the thumbnail image to the picture box
                pictureBoxPreview.Image = thumbnail;

                // Check if the image width is greater than the PictureBox width
                if (pictureBoxPreview.Image.Width > pictureBoxPreview.Width)
                {
                    // If the image is wider than the PictureBox, set the PictureBoxSizeMode to Zoom
                    pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    // If the image is smaller or equal to the PictureBox, set the PictureBoxSizeMode to CenterImage
                    pictureBoxPreview.SizeMode = PictureBoxSizeMode.CenterImage;
                }

            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs during thumbnail generation
                MessageBox.Show("Error generating video thumbnail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Generates a default "No preview available" image with a specified width and height.
        /// </summary>
        /// <returns>A Bitmap object representing the generated image.</returns>
        private Bitmap GenerateNoPreviewImage()
        {
            int width = pictureBoxPreview.Width;
            int height = pictureBoxPreview.Height;

            // Create a new Bitmap object with the specified width and height
            Bitmap image = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(image))
            {
                // Set the graphics smoothing mode to AntiAlias for smoother rendering
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Clear the graphics with a white background
                graphics.Clear(Color.White);

                //If you want border on "no preview image", then you can use this
                //using (Pen pen = new Pen(Color.Gray, 1))
                //{
                //     Draw a rectangle with a gray pen, leaving a 1-pixel border around the image
                //    graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);
                //}

                using (Font font = new Font("Arial", 12))
                {
                    // Create a StringFormat object with center alignment for text rendering
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    // Draw the "No preview available" text in the center of the image
                    graphics.DrawString("No preview available", font, Brushes.Black, new RectangleF(0, 0, width, height), format);
                }
            }

            // Return the generated image
            return image;
        }


        #endregion


        #region System Methods


        // TODO: Dynamic Folder Refresh: Automated Updating for Instant Changes
        /// <summary>
        /// It refreshes the content of the ListView by populating it with the files and directories of the current directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Refresh the content of the ListView by populating it with the files and directories of the current directory.
            PopulateListView(currentDirectory);
        }


        /// <summary>
        /// It copies the selected files or directories to the clipboard for further use.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if any items are selected in the fileListView.
            if (fileListView.SelectedItems.Count > 0)
            {
                // Iterate through the selected items.
                foreach (ListViewItem item in fileListView.SelectedItems)
                {
                    // Get the full path of the item and add it to the clipboardPathList.
                    clipboardPathList.Add(((Dictionary<string, string>)item.Tag)["FullName"]);
                }

                // Set the isCutOperation flag to false, indicating a copy operation.
                isCutOperation = false;
            }
        }


        /// <summary>
        /// It prepares the selected items for a cut operation by adding their file paths to the clipboardPathList and setting the isCutOperation flag to true.<br></br>
        /// The cut operation indicates that the selected files or folders should be moved instead of copied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if there are selected items.
            if (fileListView.SelectedItems.Count > 0)
            {
                // Iterate through the selected items.
                foreach (ListViewItem item in fileListView.SelectedItems)
                {
                    // Retrieve the full file path from the item's tag.
                    string filePath = ((Dictionary<string, string>)item.Tag)["FullName"];

                    // Add the file path to the clipboardPathList.
                    clipboardPathList.Add(filePath);
                }

                // Set the isCutOperation flag to indicate a cut operation.
                isCutOperation = true;
            }
        }


        // TODO: perform copy or move with overwrite, skip or compare both files with another design like Windows 10 "ReplaceOrSkip" form.
        /// <summary>
        /// It performs the paste operation by moving or copying the files/folders from the clipboardPathList to the current directory.<br></br>
        /// If the isCutOperation flag is set to true, the files/folders are moved (cut), otherwise they are copied.<br></br>
        /// After the paste operation, the ListView and TreeView are updated to reflect the changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Checks if the clipboardPathList is null or empty
            if (clipboardPathList == null || clipboardPathList.Count <= 0)
                return;

            // Iterates through each path in the clipboardPathList
            foreach (string path in clipboardPathList)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        // If the path represents a file
                        if (isCutOperation)
                        {
                            // Performs a cut operation by moving the file to the current directory
                            File.Move(path, Path.Combine(currentDirectory, Path.GetFileName(path)));
                        }
                        else
                        {
                            // Performs a copy operation by copying the file to the current directory
                            File.Copy(path, Path.Combine(currentDirectory, Path.GetFileName(path)));
                        }
                    }
                    else if (Directory.Exists(path))
                    {
                        // If the path represents a directory
                        if (isCutOperation)
                        {
                            // Performs a cut operation by moving the directory to the current directory
                            Directory.Move(path, Path.Combine(currentDirectory, new DirectoryInfo(path).Name));
                        }
                        else
                        {
                            // Performs a copy operation by recursively copying the directory to the current directory
                            CopyDirectory(path, Path.Combine(currentDirectory, new DirectoryInfo(path).Name));
                        }
                    }
                }
                catch { }
            }

            // Updates the ListView and TreeView to reflect the changes
            PopulateListView(currentDirectory);
            //PopulateChildNodes(driveTreeView.SelectedNode);
        }


        // TODO: Perform rename with overwrite, skip, or rename with number, with another design like Windows 10 "RenameForm" form.
        /// <summary>
        /// It will edit the label of the focused file or folder in the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if any item is selected in the fileListView
            if (fileListView.SelectedItems.Count > 0)
            {
                // Get the first selected item from the fileListView
                ListViewItem selectedItem = fileListView.SelectedItems[0];

                // Begin the edit mode for the selected item, allowing the user to modify its text
                selectedItem.BeginEdit();
            }
        }


        /// <summary>
        /// It is triggered after the user finishes editing the label of a ListView item.<br></br>
        /// It'll verify if the label containing the filename is non-empty, valid, and different from the previous label, and then it'll proceed to rename the folder or file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            // Get the old name and the new name entered by the user
            string oldName = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"];

            // Check if the new name is empty, the same as the old name, or invalid
            if (string.IsNullOrEmpty(e.Label) || e.Label == Path.GetFileName(oldName) || !IsFilenameValid(e.Label))
            {
                // Cancel the edit if the new name is not valid
                e.CancelEdit = true;
                return;
            }

            // Create the full path of the new name
            string newName = Path.Combine(Path.GetDirectoryName(oldName), e.Label);

            try
            {
                if (File.Exists(oldName))
                {
                    if (File.Exists(newName))
                    {
                        // Display an error message if a file with the new name already exists
                        MessageBox.Show("There is a file with the same name in the current path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                    else
                    {
                        // Rename the file by moving it to the new name
                        File.Move(oldName, newName);
                        ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"] = newName;
                        if (!imageList.Images.ContainsKey(".txt"))
                        {
                            // Get file icon for other file types
                            Icon fileIcon = IconProvider.GetIconByFileName(newName);

                            // Add image to image list
                            imageList.Images.Add(".txt", fileIcon);
                        }
                        fileListView.SelectedItems[0].ImageKey = Path.GetExtension(newName);

                    }
                }
                else if (Directory.Exists(oldName))
                {
                    if (Directory.Exists(newName))
                    {
                        // Display an error message if a folder with the new name already exists
                        MessageBox.Show("There is a folder with the same name in the current path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                    else
                    {
                        // Rename the folder by moving it to the new name
                        Directory.Move(oldName, newName);
                        ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"] = newName;
                        //PopulateChildNodes(driveTreeView.SelectedNode);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs during the renaming operation
                MessageBox.Show($"An error occurred while renaming: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }
        }


        /// <summary>
        /// Deletes the selected files or directories.<br></br>
        /// After the deletion, the status bar is updated with the counts of remaining items in the current directory.<br></br>
        /// Finally, the file list view and the directory tree view are refreshed to reflect the changes made by the deletion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if any items are selected in the file list view
            foreach (ListViewItem item in fileListView.SelectedItems)
            {
                try
                {
                    string fullName = ((Dictionary<string, string>)item.Tag)["FullName"];
                    // Check if it is a file
                    if (File.Exists(fullName))
                    {
                        // Delete the file
                        File.Delete(fullName);
                    }
                    // Check if it is a directory
                    else if (Directory.Exists(fullName))
                    {
                        // Delete the directory and its contents recursively
                        Directory.Delete(fullName, true);
                    }

                    // Remove the item from the file list view
                    item.Remove();
                }
                catch (Exception ex)
                {
                    // Display an error message to inform the user about the deletion error
                    MessageBox.Show($"An error occurred while deleting the file or directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Update the status bar with the new count of items in the file list view
            statusBar.Text = fileListView.Items.Count.ToString();

            // Refresh the file list view and the directory tree view to reflect the changes made by the deletion
            PopulateListView(currentDirectory);
            //PopulateChildNodes(driveTreeView.SelectedNode);
        }


        // TODO: Add Overwrite/merge, create folder, or create folder with name + number automatically with another best design
        /// <summary>
        /// Creates a new folder in the current directory. It generates a unique name for the new folder by appending a number to the base name "New Folder".<br></br>
        /// It iterates over the numbers until it finds a number that results in a non-existing folder name.<br></br>
        /// The ListViewItem is put into edit mode to allow the user to change the folder name if desired.<br></br>
        /// Finally, the directory tree view is updated to reflect the addition of the new folder.
        /// </summary>
        /// <param name="sender">The sender object, which is the NewFolderToolStripMenuItem.</param>
        /// <param name="e">The EventArgs object that contains the event data.</param>
        private void NewFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string baseFolderName = "New Folder";
                string newFolderPath = Path.Combine(currentDirectory, baseFolderName);
                string folderName = baseFolderName;

                // Initialize counter variable
                int counter = 1;

                // Find a unique name for the new folder
                while (Directory.Exists(newFolderPath))
                {
                    folderName = $"New Folder({counter++})";
                    newFolderPath = Path.Combine(currentDirectory, folderName);
                }

                // Create the new folder
                Directory.CreateDirectory(newFolderPath);

                // Add the new folder to the file list view
                ListViewItem item = fileListView.Items.Add(folderName, (int)IconIndex.Folder);
                item.SubItems.AddRange(new[] { "Folder", "", DateTime.Now.ToString() });

                // Set the tag of the item to store folder information
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    {"FullName", newFolderPath },
                    {"Type", "Folder" }
                };

                item.Tag = dict;

                // Put the item in edit mode to allow renaming
                item.BeginEdit();

                // Update the status label with the total count of folders and files
                countToolStripStatusLabel.Text = fileListView.Items.Count == 1
                   ? $"{fileListView.Items.Count} item   |"
                   : $"{fileListView.Items.Count} items   |";
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // TODO: Add Overwrite/merge, create folder, or create folder with name + number automatically with another best design
        /// <summary>
        /// Creates a new text document in the current directory. It generates a unique name for the new text document by appending a number to the base name "New Text Document.txt".<br></br>
        /// It iterates over the numbers until it finds a number that results in a non-existing document name.<br></br>
        /// The ListViewItem is put into edit mode to allow the user to change the folder name if desired.<br></br>
        /// </summary>
        /// <param name="sender">The sender object, which is the NewFolderToolStripMenuItem.</param>
        /// <param name="e">The EventArgs object that contains the event data.</param>
        private void NewFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string baseFileName = "NewFile.txt";
                string newFilePath = Path.Combine(currentDirectory, baseFileName);
                string fileName = baseFileName;

                // Initialize counter variable
                int counter = 1;
                // Find a unique name for the new text document
                while (File.Exists(newFilePath))
                {
                    fileName = $"NewFile({counter++}).txt";
                    newFilePath = Path.Combine(currentDirectory, fileName);
                }

                // Create the new text document
                File.WriteAllText(newFilePath, string.Empty);

                // Add the new file to the file list view
                ListViewItem item = fileListView.Items.Add(fileName);
                item.SubItems.AddRange(new[] { "File", "", DateTime.Now.ToString() });

                // Set the tag of the item to store file information
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    {"FullName", newFilePath },
                    {"Type", "File" }
                };

                if (!imageList.Images.ContainsKey(".txt"))
                {
                    // Get file icon for other file types
                    Icon fileIcon = IconProvider.GetIconByFileName(newFilePath);

                    // Add image to image list
                    imageList.Images.Add(".txt", fileIcon);
                }

                // Set image key for the item
                item.ImageKey = ".txt";

                item.Tag = dict;

                // Put the item in edit mode to allow renaming
                item.BeginEdit();

                // Update the status label with the total count of folders and files
                countToolStripStatusLabel.Text = fileListView.Items.Count == 1
                   ? $"{fileListView.Items.Count} item   |"
                   : $"{fileListView.Items.Count} items   |";
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        /// <summary>
        /// It will select all the files and folders in the current directory.<br></br>
        /// Shortcut => CTRL + A
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Iterate over each item in the file list view
            foreach (ListViewItem item in fileListView.Items)
            {
                // Select the item
                item.Selected = true;
            }
        }


        /// <summary>
        /// It will deselect all the files and folders in the current directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear the SelectedItems collection of the file list view, effectively deselecting all items
            fileListView.SelectedItems.Clear();
        }


        // TODO: Show properties of selected files and folders. If none of the files and folders are selected, then show the properties of the current directory.
        /// <summary>
        /// For now, it will show the basic properties of the first selected file and folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                clipboardPathList.Clear();
                foreach (ListViewItem item in fileListView.SelectedItems)
                {
                    clipboardPathList.Add(((Dictionary<string, string>)item.Tag)["FullName"]);
                }
                PropertiesForm propertiesForm = new PropertiesForm(clipboardPathList);
                propertiesForm.ShowDialog();
            }
        }


        // TODO: Perform some more operations like the user selecting a folder, like Windows 10 File Explorer.
        /// <summary>
        /// It is triggered when a node is selected in the DriveTreeView control.<br></br>
        /// It checks if the selected node's tag represents a DirectoryInfo object, which indicates a directory.<br></br>
        /// If it is a directory, the address bar text is updated with the full path of the selected directory, and the NavigateToDirectory method is called to navigate to the selected directory in the file list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DriveTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Check if the selected node's tag represents recent directory
            if (e.Node.Tag.ToString() == "recent")
            {
                PopulateListView("recent");
            }
            // Check if the selected node's tag represents a DirectoryInfo object
            else if (e.Node.Tag is DirectoryInfo directory)
            {
                // Update the address bar text with the full path of the selected directory
                addressBar.Text = directory.FullName;

                // Navigate to the selected directory in the file list view
                NavigateToDirectory(directory.FullName);
            }
        }


        /// <summary>
        /// It will populate the child directories of the expanded node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DriveTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // If tag of parent node is "recent" then do not add any child node
            if (e.Node.Tag.ToString() == "recent")
            {
                return;
            }
            // Retrieve the parent node being expanded
            var parentNode = e.Node;
            // Clear the existing child nodes
            parentNode.Nodes.Clear();
            // Populate the directories under the expanded node
            PopulateChildNodes(parentNode);
        }


        /// <summary>
        /// It immediately exits the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();


        /// <summary>
        /// When the application is loaded, it will populate the treeview, and the treeview will populate the listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Populate the tree view with directory structure
            PopulateTreeView();
        }


        /// <summary>
        /// When a user double-clicks on a file, it will open in the default viewer, and when the user double-clicks on a folder, the folder will open.<br></br>
        /// It also triggers when the user clicks on "open" in the context menu strip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if there is a focused item in the file list view
                if (fileListView.FocusedItem?.Tag is Dictionary<string, string> itemTag)
                {
                    // Get the full path of the selected item
                    string selectedItemPath = itemTag["FullName"]; //fileListView.SelectedItems[0].Tag.ToString();

                    // Check if the item is a file
                    if (File.Exists(selectedItemPath))
                        // Open the file using the default associated application
                        Process.Start(selectedItemPath);
                    else
                    {
                        // Navigate to the selected directory
                        NavigateToDirectory(selectedItemPath);
                        // Update the address bar text with the selected directory path
                        addressBar.Text = selectedItemPath;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs during the opening process
                MessageBox.Show($"An error occurred while opening the file or directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Navigates back to the previous directory in the file system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, EventArgs e)
        {
            // Check if there are items in the backStack
            if (backStack.Count > 0 || currentDirectory == "recent")
            {
                // Push the current directory to the forwardStack
                forwardStack.Push(currentDirectory);
                // Pop the previous directory from the backStack
                currentDirectory = backStack.Pop();
                // Update the address bar with the current directory
                addressBar.Text = currentDirectory;
                // Populate the file list view with the contents of the current directory
                PopulateListView(currentDirectory);
                // Enable the forwardButton since we can navigate forward now
                forwardButton.Enabled = true;
            }

            // Disable the backButton if there are no more items in the backStack
            backButton.Enabled = backStack.Count > 0;
        }


        /// <summary>
        /// Navigates forward to the next directory in the file system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForwardButton_Click(object sender, EventArgs e)
        {
            // Check if there are items in the forwardStack
            if (forwardStack.Count > 0 || currentDirectory == "recent")
            {
                // Push the current directory to the backStack
                backStack.Push(currentDirectory);
                // Pop the next directory from the forwardStack
                currentDirectory = forwardStack.Pop();
                // Update the address bar with the current directory
                addressBar.Text = currentDirectory;
                // Populate the file list view with the contents of the current directory
                PopulateListView(currentDirectory);
                // Enable the backButton since we can navigate back now
                backButton.Enabled = true;
            }

            // Disable the forwardButton if there are no more items in the forwardStack
            forwardButton.Enabled = forwardStack.Count > 0;
        }


        /// <summary>
        /// Navigates up to the parent directory of the current directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpButton_Click(object sender, EventArgs e)
        {
            // Check if the currentDirectory is not empty
            if (!string.IsNullOrEmpty(currentDirectory))
            {
                if (currentDirectory == "recent")
                    return;
                // Get the parent directory of the current directory
                var parentDirectory = Directory.GetParent(currentDirectory);

                // Check if the parentDirectory is not null, indicating that a parent directory exists
                if (parentDirectory != null)
                {
                    // Navigate to the parent directory
                    NavigateToDirectory(parentDirectory.FullName);
                }
            }
        }


        /// <summary>
        /// Navigates to the directory specified in the address bar when the Enter key is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressBar_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the Enter key is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Check addressBar is blank
                if (addressBar.Text == "")
                {
                    addressBar.Text = currentDirectory;
                }
                // Check the directory exists or adrresBar text is recent
                else if (Directory.Exists(addressBar.Text) || addressBar.Text == "recent")
                {
                    // Push the current directory to the backStack for navigation history
                    backStack.Push(currentDirectory);

                    // Navigate to the directory specified in the address bar
                    NavigateToDirectory(addressBar.Text);
                }
                else
                {
                    MessageBox.Show("Enter a valid path", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }


        // TODO: Add some more key binding functionality
        /// <summary>
        /// If the Delete key is pressed, it deletes the selected files and folders.<br></br>
        /// If the F2 key is pressed, it renames the first item of selected files or folders.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Delete key is pressed
            if (e.KeyCode == Keys.Delete)
            {
                // Call function to delete selected items
                DeleteToolStripMenuItem_Click(sender, e);
            }
            // Check if the F2 key is pressed
            else if (e.KeyCode == Keys.F2)
            {
                // Call function to rename selected items
                RenameToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Back)
            {
                BackButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                SelectAllToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                CopyToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.X && e.Control)
            {
                CutToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                PasteToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                OpenToolStripMenuItem_Click(sender, e);
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.N)
            {
                NewFolderToolStripMenuItem_Click(sender, e);
            }
        }


        // TODO: Add Some more click and enhance functionality
        /// <summary>
        /// If the right mouse button is clicked, it retrieves the selected file item at the clicked location and shows the contextMenuStrip at the clicked location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListView_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if the right mouse button is clicked
            if (e.Button == MouseButtons.Right)
            {
                // Get the ListViewItem at the clicked location
                if (fileListView.GetItemAt(e.X, e.Y) is ListViewItem)
                {
                    // Show the context menu at the clicked location
                    contextMenuStrip.Show(fileListView, e.Location);
                }
            }
        }


        // TODO: Add enable and disabe buttoms according to files and folders and more
        /// <summary>
        /// If the right mouse button is clicked, it retrieves the clicked location and shows the contextMenuStrip at the clicked location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (fileListView.SelectedItems.Count == 0)
            {
                e.Cancel = false;
            }

        }

        //TODO: If current directory is "recent" then drag nad drop should be disabled
        /// <summary>
        /// Handles the DragEnter event of the ListView control to specify the effect when files are dragged into the ListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the dragged data contains file(s)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Set the drag-and-drop effect to Copy
                e.Effect = DragDropEffects.Copy;
            }
        }


        //TODO: Move Files and folder in same drive
        /// <summary>
        /// Handles the DragDrop event of the ListView control to process the dropped files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            // Get the paths of the dropped files
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (droppedFiles != null)
            {
                // Process each dropped file
                foreach (string filePath in droppedFiles)
                {
                    if (File.Exists(filePath))
                    {
                        // If the dropped item is a file, copy it to the current directory
                        string targetPath = Path.Combine(currentDirectory, Path.GetFileName(filePath));
                        try
                        {
                            File.Copy(filePath, targetPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (Directory.Exists(filePath))
                    {
                        // If the dropped item is a directory, recursively copy it to the current directory
                        string targetPath = Path.Combine(currentDirectory, Path.GetFileName(filePath));
                        try
                        {
                            CopyDirectory(filePath, targetPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                // Refresh the ListView to display the updated contents of the current directory
                PopulateListView(currentDirectory);
            }
        }


        /// <summary>
        /// This function is called when the user clicks on the "Batch Rename" option in the context menu.<br></br>
        /// It allows the user to perform a batch rename operation on multiple selected files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a list to store the paths of selected files
            List<string> files = new List<string>();

            // Check if multiple files are selected in the fileListView
            if (fileListView.SelectedItems.Count > 1)
            {
                // Iterate through the selected items in the fileListView
                ListView.SelectedListViewItemCollection selectedItems = fileListView.SelectedItems;
                foreach (ListViewItem item in selectedItems)
                {
                    // Add the combined path of the current directory and the item's text (file name) to the files list
                    files.Add(Path.Combine(currentDirectory, item.Text));
                }
            }

            // Create an instance of the BatchRenameForm and pass the files list and the current form as parameters
            BatchRenameForm batchRename = new BatchRenameForm(files, this);

            // Show the batchRename form as a dialog
            batchRename.ShowDialog();
        }


        /// <summary>
        /// Toggles the visibility of the toolbar and updates the checked state of the toolbar menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle the visibility of the toolbar
            topToolStrip.Visible = !topToolStrip.Visible;

            // Update the checked state of the toolbar menu item
            toolbarToolStripMenuItem.Checked = !toolbarToolStripMenuItem.Checked;
        }


        /// <summary>
        /// Toggles the visibility of the status bar and updates the checked state of the status bar menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle the visibility of the status bar
            statusBar.Visible = !statusBar.Visible;

            // Update the checked state of the status bar menu item
            statusbarToolStripMenuItem.Checked = !statusbarToolStripMenuItem.Checked;
        }


        /// <summary>
        /// Adjusts the width of the address bar based on the form's width.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Adjust the width of the address bar by subtracting the constant value 313 from the form's width, It can be different in your case.
            addressBar.Width = Width - 313;
            // Adjust the size of preview pane
            panelForImagePreview.Width = Width - driveTreeView.Width - fileListView.Width;
        }


        /// <summary>
        /// Toggles the visibility of checkboxes in the file list view control based on the state of the itemCheckBoxesToolStripMenuItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemCheckBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileListView.CheckBoxes = itemCheckBoxesToolStripMenuItem.Checked;
        }


        /// <summary>
        /// Changes the ListView view mode to Tile.
        /// </summary>
        private void TileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewView(View.Tile);  // Call the ChangeListViewView function with the Tile view mode
        }


        /// <summary>
        /// Changes the ListView view mode to List.
        /// </summary>
        private void ListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewView(View.List);  // Call the ChangeListViewView function with the List view mode
        }


        /// <summary>
        /// Changes the ListView view mode to Details.
        /// </summary>
        private void DetailedInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewView(View.Details);  // Call the ChangeListViewView function with the Details view mode
        }


        /// <summary>
        /// Changes the ListView view mode to LargeIcon.
        /// </summary>
        private void LargeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewView(View.LargeIcon);  // Call the ChangeListViewView function with the LargeIcon view mode
        }


        /// <summary>
        /// Changes the ListView view mode to SmallIcon.
        /// </summary>
        private void SmallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeListViewView(View.SmallIcon);  // Call the ChangeListViewView function with the SmallIcon view mode
        }


        /// <summary>
        /// Updates the selectedItemsToolStripStatusLabel based on the number of selected items. and displays a preview of the first selected item, if applicable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the selected items status label based on the number of selected items
            selectedItemsToolStripStatusLabel.Text = fileListView.SelectedItems.Count == 0
                ? "" // No items selected
                : $"{fileListView.SelectedItems.Count} {(fileListView.SelectedItems.Count == 1 ? "item" : "items")}  selected   |"; // Display the count and pluralize "item" if necessary


            // If at least one item is selected
            if (fileListView.SelectedItems.Count > 0)
            {
                // Get the first selected item
                ListViewItem selectedItem = fileListView.SelectedItems[0];

                // Get the image path associated with the selected item from the Tag property
                string filePath = ((Dictionary<string, string>)selectedItem.Tag)["FullName"];

                // Display a preview of the selected item
                DisplayPreview(filePath);
            }
        }


        /// <summary>
        /// Show the tooltip for the item with additional information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FileListView_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (e.Item != null)
            {
                // Get the full path of the item
                string path = ((Dictionary<string, string>)e.Item.Tag)["FullName"];

                // Check if the path represents a directory
                if (Directory.Exists(path))
                {
                    // Calculate the size of the folder asynchronously
                    string folderSize = await SizeManager.GetSize(path);

                    // Update the tooltip of the item with folder information
                    e.Item.ToolTipText = $"Type: Folder\n" +
                        $"Size: {folderSize}\n" +
                        $"Date Modified: {new DirectoryInfo(path).LastWriteTime}";
                }
            }
        }


        /// <summary>
        /// Adjusts the PictureBoxSizeMode based on the image width compared to the PictureBox width.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBoxPreview_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (pictureBoxPreview.Image != null)
                {
                    // Check if the image width is greater than the PictureBox width
                    if (pictureBoxPreview.Image.Width > pictureBoxPreview.Width)
                    {
                        // If the image is wider than the PictureBox, set the PictureBoxSizeMode to Zoom
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // If the image is smaller or equal to the PictureBox, set the PictureBoxSizeMode to CenterImage
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                }
            }
            catch { }

        }


        /// <summary>
        /// Toggles the visibility of the panelForImagePreview control based on the checked state of the PreviewPaneToolStripMenuItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check the checked state of the PreviewPaneToolStripMenuItem
            if (previewPaneToolStripMenuItem.Checked)
            {
                // If the PreviewPaneToolStripMenuItem is checked, make the panelForImagePreview control visible
                panelForImagePreview.Visible = true;
            }
            else
            {
                // If the PreviewPaneToolStripMenuItem is not checked, hide the panelForImagePreview control
                panelForImagePreview.Visible = false;
            }
        }


        /// <summary>
        /// Displays the AboutForm as a dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a new instance of the AboutForm
            AboutForm aboutForm = new AboutForm();

            // Display the AboutForm as a dialog
            aboutForm.ShowDialog();
        }



        #endregion

        //TODO: Some Other Features of Compression and Decompression will be added soon.
        #region Compression and Decompression Functionality

        /// <summary>
        /// Generates a unique file path based on the base file path and the desired file extension.
        /// If the compressed file is already present in the base file path, it appends a counter value to make it unique.
        /// </summary>
        /// <param name="baseFilePath">The base file path to generate a unique file path from.</param>
        /// <param name="fileExtension">The desired file extension to be added to the file path.</param>
        /// <returns>A unique file path with the desired file extension.</returns>
        private string GetUniqueFilePath(string baseFilePath, string fileExtension)
        {
            // Ensure that the file extension starts with a dot, or add it if missing
            fileExtension = fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension;

            // Construct the initial new file path based on the base file path and whether it exists
            string newFilePath = Path.Combine(
                Path.GetDirectoryName(baseFilePath), // Get the directory path of the base file
                File.Exists(baseFilePath) // Check if the base file path represents an existing file
                    ? ( // If it's a file, determine the new file name based on extension presence
                        string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(baseFilePath)) // Check if the file name (without extension) is empty or null
                            ? Path.GetFileName(baseFilePath) // Use the base file name (with extension) as is
                            : Path.GetFileNameWithoutExtension(baseFilePath) // Use the file name without extension
                      )
                    : Path.GetFileName(baseFilePath) // If it's not a file, use the base file name as is
                ) + fileExtension; // Append the file extension to the new file path

            // Initialize a counter for handling duplicate file names
            int counter = 1;

            // Check if the new file path already exists, and if so, modify it to ensure uniqueness
            while (File.Exists(newFilePath))
            {
                newFilePath = Path.Combine(
                    Path.GetDirectoryName(newFilePath), // Get the directory path of the new file path
                    $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter++}){fileExtension}" // Append the counter and file extension to create a unique file name
                );
            }

            // Return the final unique file path
            return newFilePath;
        }


        /// <summary>
        /// Compresses the specified files and folders into a zip archive.
        /// </summary>
        /// <param name="filesAndFolders">The list of file and folder paths to be compressed.</param>
        /// <param name="zipPath">The path of the zip archive to create.</param>
        public void CompressFilesAndFolders(List<string> filesAndFolders, string zipPath)
        {
            // Open a zip archive for writing
            using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                // Iterate through each item in the list of files and folders
                foreach (string itemPath in filesAndFolders)
                {
                    // Check if the item is a file
                    if (File.Exists(itemPath))
                    {
                        // Get the file name
                        string entryName = Path.GetFileName(itemPath);
                        // Create a zip entry from the file and add it to the archive
                        zip.CreateEntryFromFile(itemPath, entryName, CompressionLevel.Optimal);
                    }
                    // Check if the item is a directory
                    else if (Directory.Exists(itemPath))
                    {
                        // Get the directory name
                        string folderName = new DirectoryInfo(itemPath).Name;
                        // Get all files within the directory and its subdirectories
                        string[] files = Directory.GetFiles(itemPath, "*", SearchOption.AllDirectories);

                        // Iterate through each file
                        foreach (string file in files)
                        {
                            // Get the relative path of the file from the directory
                            string entryName = GetRelativePath(itemPath, file);
                            // Create a zip entry from the file with its relative path and add it to the archive
                            zip.CreateEntryFromFile(file, Path.Combine(folderName, entryName), CompressionLevel.Optimal);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets the relative path from a base directory to a full path.
        /// </summary>
        /// <param name="baseDirectory">The base directory path.</param>
        /// <param name="fullPath">The full path to convert to a relative path.</param>
        /// <returns>The relative path from the base directory to the full path.</returns>
        private static string GetRelativePath(string baseDirectory, string fullPath)
        {
            // Create a Uri for the base directory with a trailing directory separator character
            Uri baseUri = new Uri(baseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? baseDirectory
                : baseDirectory + Path.DirectorySeparatorChar);

            // Create a Uri for the full path
            Uri fullUri = new Uri(fullPath);

            // Get the relative Uri from the base Uri to the full Uri
            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            // Convert the relative Uri to a string and unescape any encoded characters
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            // Replace forward slashes with the platform-specific directory separator character
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

            // Return the resulting relative path
            return relativePath;
        }


        //TODO: Add overwrite or skip
        /// <summary>
        /// Decompresses a zip file to the specified extract path.
        /// </summary>
        /// <param name="zipPath">The path of the zip file to decompress.</param>
        /// <param name="extractPath">The path where the zip file contents will be extracted.</param>
        public void DecompressFile(string zipPath, string extractPath)
        {
            try
            {
                // Open the zip file for reading
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    // Iterate over each entry in the zip file
                    foreach (var entry in archive.Entries)
                    {
                        try
                        {
                            // Combine the extract path with the entry's full name to get the entry's extract path
                            string entryPath = Path.Combine(extractPath, entry.FullName);

                            // Create the necessary directory structure for the entry if it doesn't exist
                            Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                            // Extract the entry to the specified extract path, overwriting existing files
                            entry.ExtractToFile(entryPath, overwrite: true);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception for a specific entry
                            Console.WriteLine($"Error extracting entry '{entry.FullName}': {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception for opening the zip file
                //MessageBox.Show($"Error opening zip file '{zipPath}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Adds the selected files nad folders into a new zip file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToZipFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if there are selected items in the file list view
            if (fileListView.SelectedItems.Count > 0)
            {
                // Clear the clipboard path list
                clipboardPathList.Clear();

                // Iterate over each selected item in the file list view
                foreach (ListViewItem item in fileListView.SelectedItems)
                {
                    // Add the full name of the item to the clipboard path list
                    clipboardPathList.Add(((Dictionary<string, string>)item.Tag)["FullName"]);
                }

                // Get a unique file path for the zip file based on the full name of the focused item
                string zipPath = GetUniqueFilePath(((Dictionary<string, string>)fileListView.FocusedItem.Tag)["FullName"], ".zip");

                // Output the zip file path to the console (for testing/debugging purposes)
                Console.WriteLine(zipPath);

                // Compress the selected items to the zip file
                CompressFilesAndFolders(clipboardPathList, zipPath);
            }
        }


        /// <summary>
        /// Decompresses the selected zip file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the full path of the selected zip file from the tag of the selected item in the file list view
            string zipPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"];

            // Get the path for extracting the decompressed files/folders
            string path = Path.Combine(Path.GetDirectoryName(zipPath), Path.GetFileNameWithoutExtension(zipPath));

            // Call the DecompressFile method to decompress the zip file to the specified path
            DecompressFile(zipPath, path);
        }

        #endregion
    }

}


