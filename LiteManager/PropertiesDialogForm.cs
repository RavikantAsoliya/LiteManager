using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteManager
{
    public partial class PropertiesDialog : Form
    {
        private string filePath;
        private FileAttributes originalAttributes;

        public PropertiesDialog(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
        }

        private void PropertiesDialog_Load(object sender, EventArgs e)
        {
            LoadProperties();
            LoadAttributes();
        }

        private void LoadProperties()
        {
            try
            {
                listViewProperties.Items.Clear();

                // Get the file or directory info
                FileSystemInfo info;
                if (Directory.Exists(filePath))
                    info = new DirectoryInfo(filePath);
                else
                    info = new FileInfo(filePath);

                // Add properties to the ListView
                listViewProperties.Items.Add(new ListViewItem(new string[] { "Name", info.Name }));
                listViewProperties.Items.Add(new ListViewItem(new string[] { "Path", info.FullName }));
                listViewProperties.Items.Add(new ListViewItem(new string[] { "Type", (info is DirectoryInfo) ? "Folder" : "File" }));
                listViewProperties.Items.Add(new ListViewItem(new string[] { "Size", GetFormattedSize(info) }));
                listViewProperties.Items.Add(new ListViewItem(new string[] { "Created", info.CreationTime.ToString() }));
                listViewProperties.Items.Add(new ListViewItem(new string[] { "Modified", info.LastWriteTime.ToString() }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFormattedSize(FileSystemInfo info)
        {
            if (info is FileInfo file)
                return FormatSize(file.Length);
            else
                return string.Empty;
        }

        private string FormatSize(long size)
        {
            const int scale = 1024;
            string[] orders = { "TB", "GB", "MB", "KB", "Bytes" };
            double len = (double)size;
            int order = 0;
            while (len >= scale && order < orders.Length - 1)
            {
                len /= scale;
                order++;
            }
            return string.Format("{0:0.##} {1}", len, orders[order]);
        }

        private void LoadAttributes()
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                originalAttributes = attributes;

                checkBoxReadOnly.Checked = (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                checkBoxHidden.Checked = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(filePath);

                if (checkBoxReadOnly.Checked)
                    attributes |= FileAttributes.ReadOnly;
                else
                    attributes &= ~FileAttributes.ReadOnly;

                if (checkBoxHidden.Checked)
                    attributes |= FileAttributes.Hidden;
                else
                    attributes &= ~FileAttributes.Hidden;

                if (attributes != originalAttributes)
                    File.SetAttributes(filePath, attributes);

                MessageBox.Show("Attributes applied successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

