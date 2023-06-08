using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LiteManager
{
    public partial class BatchRenameForm : Form
    {
        public List<string> ListOfFiles { get; set; }
        public BatchRenameForm()
        {
            InitializeComponent();
        }
        public BatchRenameForm(List<string> ListOfFiles)
        {
            InitializeComponent();
            this.ListOfFiles = ListOfFiles;
        }

        private void RbPlusNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPlusNumber.Checked == true)
            {
                textBox1.Enabled = true;
                startNumberTextBox.Enabled = true;
                label1.Enabled = true;
                rbPlusNumber.ForeColor = SystemColors.ControlText;
            }
            else
            {
                textBox1.Enabled = false;
                startNumberTextBox.Enabled = false;
                label1.Enabled = false;
                rbPlusNumber.ForeColor = Color.FromArgb(122, 122, 122);
            }
        }

        private void RbPlusOriginalFileName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPlusOriginalFileName.Checked == true)
            {
                textBox3.Enabled = true;
                rbPlusOriginalFileName.ForeColor = SystemColors.ControlText;
            }
            else
            {
                textBox3.Enabled = false;
                rbPlusOriginalFileName.ForeColor = Color.FromArgb(122, 122, 122);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (rbPlusNumber.Checked)
            {
                if (ListOfFiles != null || ListOfFiles.Count <= 0)
                {
                    try
                    {
                        string startingNumber = startNumberTextBox.Text == "" || startNumberTextBox.Text == "1,01,001..." ? "1" : startNumberTextBox.Text;
                        int intValue = int.Parse(startingNumber);
                        foreach (string fileName in ListOfFiles)
                        {
                            string name = textBox1.Text == "" || textBox1.Text == "Input a new name" ? fileName : textBox1.Text;
                            string fileExtension = extensionTextBox.Text == "" ? new FileInfo(fileName).Extension : extensionTextBox.Text;
                            File.Move(fileName, Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(name)) + startingNumber + (fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension));
                            intValue++;
                            startingNumber = intValue.ToString().PadLeft(startingNumber.Length, '0');
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("List of Files is Empty", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (rbPlusOriginalFileName.Checked)
            {
                if (ListOfFiles != null || ListOfFiles.Count <= 0)
                {
                    try
                    {
                        foreach (string fileName in ListOfFiles)
                        {
                            string prefix = textBox3.Text == "" || textBox3.Text == "Input a new name" ? "" : textBox3.Text;
                            string fileExtension = extensionTextBox.Text == "" ? new FileInfo(fileName).Extension : extensionTextBox.Text;
                            File.Move(fileName, Path.Combine(Path.GetDirectoryName(fileName), prefix + Path.GetFileNameWithoutExtension(fileName)) + (fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("List of Files is Empty", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Input a new name")
            {
                textBox1.Text = string.Empty;
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Input a new name";
            }
        }

        private void StartNumberTextBox_Enter(object sender, EventArgs e)
        {
            if (startNumberTextBox.Text == "1,01,001...")
            {
                startNumberTextBox.Text = string.Empty;
            }
        }

        private void StartNumberTextBox_Leave(object sender, EventArgs e)
        {
            if (startNumberTextBox.Text == "")
            {
                startNumberTextBox.Text = "1,01,001...";
            }
        }

        private void TextBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Input a new name")
            {
                textBox3.Text = string.Empty;
            }
        }

        private void TextBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "Input a new name";
            }
        }
    }
}
