using LiteManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteManager
{
    public partial class MainForm : Form
    {
        #region Variables

        private string currentDirectory = string.Empty;
        private string clipboardPath = string.Empty;
        private bool isCutOperation = false;
        private readonly Stack<string> backStack = new Stack<string>();
        private readonly Stack<string> forwardStack = new Stack<string>();

        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        #region Working Additional Methods

        private void PopulateTreeView()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    TreeNode driveNode = new TreeNode(drive.Name)
                    {
                        Tag = drive.RootDirectory
                    };

                    try
                    {
                        DirectoryInfo[] directories = drive.RootDirectory.GetDirectories();
                        foreach (DirectoryInfo directory in directories)
                        {
                            TreeNode directoryNode = new TreeNode(directory.Name)
                            {
                                Tag = directory
                            };
                            driveNode.Nodes.Add(directoryNode);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to access drive {drive.Name}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    driveTreeView.Nodes.Add(driveNode);
                }
            }
        }

        private void PopulateListView(string path)
        {
            try
            {
                fileListView.Items.Clear();
                DirectoryInfo directory = new DirectoryInfo(path);
                DirectoryInfo[] dirs = directory.GetDirectories();
                FileInfo[] files = directory.GetFiles();

                foreach (var item in dirs)
                {
                    ListViewItem listItem = new ListViewItem(item.Name);
                    listItem.SubItems.Add("Folder");
                    listItem.SubItems.Add("");
                    listItem.SubItems.Add(item.LastWriteTime.ToString());
                    Dictionary<string, string> dict = new Dictionary<string, string>
                    {
                        {"FullName", item.FullName },
                        {"Type", "Folder" }
                    };
                    listItem.Tag = dict;
                    fileListView.Items.Add(listItem);
                }

                foreach (var item in files)
                {
                    ListViewItem listItem = new ListViewItem(item.Name);
                    listItem.SubItems.Add("File");
                    listItem.SubItems.Add(item.Length.ToString());
                    listItem.SubItems.Add(item.LastWriteTime.ToString());
                    Dictionary<string, string> dict = new Dictionary<string, string>
                    {
                        {"FullName", item.FullName },
                        {"Type", "File" }
                    };
                    listItem.Tag = dict;
                    fileListView.Items.Add(listItem);
                }

                countToolStripStatusLabel.Text = $"Total: {dirs.Length} Folders, {files.Length} Files";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddDirectories(TreeNode parentNode)
        {
            var directory = (DirectoryInfo)parentNode.Tag;
            try
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    var childNode = new TreeNode(subDirectory.Name)
                    {
                        Tag = subDirectory
                    };
                    parentNode.Nodes.Add(childNode);
                    childNode.Nodes.Add("Loading...");
                }
            }
            catch (UnauthorizedAccessException)
            {
                parentNode.Nodes.Add("Access Denied");
            }
        }

        #endregion

        #region Working System Methods

        // TODO: Dynamic Folder Refresh: Automated Updating for Instant Changes
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopulateListView(currentDirectory);
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                clipboardPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"].ToString();
                isCutOperation = false;
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                clipboardPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"].ToString();
                isCutOperation = true;
            }
        }

        // TODO: perform copy or move with overwrite, skip or compare both files with another design like Windows 10 "ReplaceOrSkip" form.
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(clipboardPath))
            {
                try
                {
                    string targetPath = Path.Combine(currentDirectory, Path.GetFileName(clipboardPath));
                    if (isCutOperation)
                    {
                        if (File.Exists(targetPath))
                        {
                            var result = MessageBox.Show("A file with the same name already exists. Do you want to overwrite it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                File.Copy(clipboardPath, targetPath, true);
                                File.Delete(clipboardPath);
                                isCutOperation = false;
                            }
                        }
                        else
                        {
                            File.Move(clipboardPath, targetPath);
                            isCutOperation = false;
                        }
                    }
                    else
                    {
                        if (File.Exists(targetPath))
                        {
                            var result = MessageBox.Show("A file with the same name already exists. Do you want to overwrite it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                File.Copy(clipboardPath, targetPath, true);
                            }
                        }
                        else
                        {
                            File.Copy(clipboardPath, targetPath);
                        }
                    }
                    PopulateListView(currentDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // TODO: Perform rename with overwrite, skip, or rename with number, with another design like Windows 10 "RenameForm" form.
        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                fileListView.SelectedItems[0].BeginEdit();
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = fileListView.SelectedItems[0];
                string selectedItemPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"].ToString();
                string selectedFileName = Path.GetFileName(selectedItemPath);
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedFileName}'?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(selectedItemPath))
                        {
                            File.Delete(selectedItemPath);
                        }
                        else if (Directory.Exists(selectedItemPath))
                        {
                            Directory.Delete(selectedItemPath, true);
                        }

                        fileListView.Items.Remove(selectedItem);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // TODO: Overwrite, create folder, or create folder with number, with another best design
        private void NewFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (InputDialogForm inputDialog = new InputDialogForm())
            {
                inputDialog.Text = "New Folder";
                inputDialog.lblPrompt.Text = "Enter the name of the new folder:";
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string newFolderName = inputDialog.textBox.Text;
                    if (!string.IsNullOrWhiteSpace(newFolderName))
                    {
                        string newFolderPath = Path.Combine(currentDirectory, newFolderName);
                        try
                        {
                            Directory.CreateDirectory(newFolderPath);
                            PopulateListView(currentDirectory);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to create the new folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in fileListView.Items)
            {
                item.Selected = true;
            }
        }

        private void DeselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in fileListView.Items)
            {
                item.Selected = false;
            }
        }

        // TODO: Show properties of current folder when file or folder is not selected.
        private void PropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                string selectedItemPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"].ToString();
                if (File.Exists(selectedItemPath))
                {
                    using (var propertiesDialog = new PropertiesDialog(selectedItemPath))
                    {
                        propertiesDialog.ShowDialog();
                    }
                }
                else if (Directory.Exists(selectedItemPath))
                {
                    using (var propertiesDialog = new PropertiesDialog(selectedItemPath))
                    {
                        propertiesDialog.ShowDialog();
                    }
                }
            }
        }

        private void DriveTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is DirectoryInfo directory)
            {
                addressBar.Text = directory.FullName;
                NavigateToDirectory(directory.FullName);

            }
        }

        private void DriveTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var parentNode = e.Node;
            parentNode.Nodes.Clear();
            AddDirectories(parentNode);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            PopulateListView(currentDirectory);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PopulateTreeView();
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                string selectedItemPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"].ToString();
                if (File.Exists(selectedItemPath))
                {
                    System.Diagnostics.Process.Start(selectedItemPath);
                }
                else
                {
                    NavigateToDirectory(selectedItemPath);
                    addressBar.Text = selectedItemPath;
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                string selectedItemPath = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"].ToString();
                if (File.Exists(selectedItemPath))
                {
                    Process.Start(selectedItemPath);
                }
                else
                {
                    NavigateToDirectory(selectedItemPath);
                    addressBar.Text = selectedItemPath;
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (backStack.Count > 0)
            {
                forwardStack.Push(currentDirectory);
                currentDirectory = backStack.Pop();
                addressBar.Text = currentDirectory;
                PopulateListView(currentDirectory);
                forwardButton.Enabled = true;
            }
            backButton.Enabled = backStack.Count > 0;
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (forwardStack.Count > 0)
            {
                backStack.Push(currentDirectory);
                currentDirectory = forwardStack.Pop();
                addressBar.Text = currentDirectory;
                PopulateListView(currentDirectory);
                backButton.Enabled = true;
            }

            forwardButton.Enabled = forwardStack.Count > 0;
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentDirectory))
            {
                var parentDirectory = Directory.GetParent(currentDirectory);

                if (parentDirectory != null)
                {
                    NavigateToDirectory(parentDirectory.FullName);
                }
            }
        }

        private void AddressBar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string directory = addressBar.Text;
                backStack.Push(currentDirectory);
                NavigateToDirectory(directory);
            }
        }

        private void NavigateToDirectory(string path)
        {
            if (!string.IsNullOrEmpty(currentDirectory))
            {
                backStack.Push(currentDirectory);
                backButton.Enabled = true;
            }
            currentDirectory = path;
            forwardStack.Clear();
            forwardButton.Enabled = false;
            PopulateListView(currentDirectory);
        }

        // TODO: Add some more key binding functionality
        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F2)
            {
                RenameToolStripMenuItem_Click(sender, e);
            }
        }

        private void FileListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem selectedFile = fileListView.GetItemAt(e.X, e.Y);
                if (selectedFile != null)
                {
                    contextMenuStrip.Show(fileListView, e.Location);
                }
            }
        }

        // TODO: Shows the options in the context menu strip according to the selected files and folders. If none of the files and folders are selected, then only some selected options should be shown.
        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (fileListView.SelectedItems.Count == 0)
            {
                e.Cancel = false;
            }
        }

        private void BatchRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> files = new List<string>();
            if (fileListView.SelectedItems.Count > 1)
            {
                ListView.SelectedListViewItemCollection selecteditems = fileListView.SelectedItems;
                foreach (ListViewItem item in selecteditems)
                {
                    files.Add(Path.Combine(addressBar.Text, item.Text));
                }
            }
            BatchRenameForm batchRename = new BatchRenameForm(files);
            batchRename.ShowDialog();
        }

        #region Drag and Drop files and folders

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("I am inside drag enter");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (droppedFiles != null)
            {
                foreach (string filePath in droppedFiles)
                {
                    if (File.Exists(filePath))
                    {
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

                PopulateListView(currentDirectory);
            }
        }

        private void CopyDirectory(string sourcePath, string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetPath, fileName);
                File.Copy(file, destFile, true);
            }

            foreach (string subdirectory in Directory.GetDirectories(sourcePath))
            {
                string directoryName = Path.GetFileName(subdirectory);
                string destDirectory = Path.Combine(targetPath, directoryName);
                CopyDirectory(subdirectory, destDirectory);
            }
        }

        #endregion

        #endregion

        private bool IsFilenameValid(string filename)
        {
            // Define a list of illegal characters
            char[] illegalChars = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };

            // Check if the filename contains any illegal characters
            foreach (char illegalChar in illegalChars)
            {
                if (filename.Contains(illegalChar.ToString()))
                {
                    MessageBox.Show("Filenames cannot contain any of the following characters:\r\n" + "\t\\/:*?\"<>|", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private void fileListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            string oldName = ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"];
            string newName = e.Label;
            if (string.IsNullOrEmpty(newName) || newName == Path.GetFileName(oldName) || !IsFilenameValid(newName))
            {
                e.CancelEdit = true;
            }
            else
            {
                newName = Path.Combine(Path.GetDirectoryName(oldName), newName);
                if (File.Exists(oldName))
                {
                    // Todo: Add Overwrite Functionality
                    if (File.Exists(newName))
                    {
                        MessageBox.Show("There is a file with the same name in the current path！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                    else
                    {
                        File.Move(oldName, newName);
                        ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"] = newName;
                    }
                }
                else if (Directory.Exists(oldName))
                {
                    if (Directory.Exists(newName))
                    {
                        MessageBox.Show("There is a folder with the same name in the current path！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                    else
                    {
                        Directory.Move(oldName, newName);
                        ((Dictionary<string, string>)fileListView.SelectedItems[0].Tag)["FullName"] = newName;
                        AddDirectories(driveTreeView.SelectedNode);
                    }
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            addressBar.Width = this.Width - 313;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            addressBar.Width = this.Width - 313;
        }

        private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            topToolStrip.Visible = !topToolStrip.Visible;
            toolbarToolStripMenuItem.Checked = !toolbarToolStripMenuItem.Checked;
        }

        private void statusbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusBar.Visible = !statusBar.Visible;
            statusbarToolStripMenuItem.Checked = !statusbarToolStripMenuItem.Checked;
        }
    }
}
