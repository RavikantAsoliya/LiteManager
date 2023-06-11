using LiteManager;
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
        /// <summary>
        /// The list of files to be renamed.
        /// </summary>
        public List<string> ListOfFiles { get; set; }

        /// <summary>
        /// The MainForm instance.
        /// </summary>
        public MainForm mainForm;

        /// <summary>
        /// Initializes a new instance of the BatchRenameForm class.
        /// </summary>
        public BatchRenameForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Initializes a new instance of the BatchRenameForm class.
        /// </summary>
        /// <param name="listOfFiles">The list of files to be renamed.</param>
        /// <param name="mainForm">The MainForm instance.</param>
        public BatchRenameForm(List<string> listOfFiles, MainForm mainForm) : this()
        {
            // Assign the provided list of files to the ListOfFiles property
            ListOfFiles = listOfFiles;

            // Store the reference to the MainForm instance
            this.mainForm = mainForm;
        }


        #region Additional Methods
        #endregion

        #region System Methods

        /// <summary>
        /// Handles the CheckedChanged event of the rbPlusNumber RadioButton. Enables or disables related controls based on the checked state.
        /// </summary>
        /// <param name="sender">The sender object, which is the rbPlusNumber RadioButton.</param>
        /// <param name="e">The EventArgs object that contains the event data.</param>
        private void RbPlusNumber_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = rbPlusNumber.Checked;

            // Enable or disable the related controls based on the checked state
            textBox1.Enabled = isChecked;
            startNumberTextBox.Enabled = isChecked;
            label1.Enabled = isChecked;

            // Set the foreground color of rbPlusNumber based on the checked state
            rbPlusNumber.ForeColor = isChecked ? SystemColors.ControlText : SystemColors.GrayText;
        }


        /// <summary>
        /// Handles the CheckedChanged event of the rbPlusOriginalFileName RadioButton. Enables or disables related controls based on the checked state.
        /// </summary>
        /// <param name="sender">The sender object, which is the rbPlusOriginalFileName RadioButton.</param>
        /// <param name="e">The EventArgs object that contains the event data.</param>
        private void RbPlusOriginalFileName_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = rbPlusOriginalFileName.Checked;

            // Enable or disable the related controls based on the checked state
            textBox3.Enabled = isChecked;

            // Set the foreground color of rbPlusOriginalFileName based on the checked state
            rbPlusOriginalFileName.ForeColor = isChecked ? SystemColors.ControlText : SystemColors.GrayText;
        }


        /// <summary>
        /// Renames the selected files based on the chosen renaming option and closes the form.
        /// </summary>
        /// <param name="sender">The sender object, which is the ButtonOK.</param>
        /// <param name="e">The EventArgs object that contains the event data.</param>
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            // Check if the ListOfFiles is empty
            if (ListOfFiles == null || ListOfFiles.Count == 0)
            {
                MessageBox.Show("List of Files is Empty", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Check the selected renaming option
                if (rbPlusNumber.Checked)
                {
                    // Get the starting number for renaming
                    string startingNumber = string.IsNullOrEmpty(startNumberTextBox.Text) || startNumberTextBox.Text == "1,01,001..." ? "1" : startNumberTextBox.Text;
                    int intValue = int.Parse(startingNumber);

                    // Rename each file in the ListOfFiles
                    foreach (string oldFileName in ListOfFiles)
                    {
                        // Get the new name, file extension, and create the new file name
                        string name = string.IsNullOrEmpty(textBox1.Text) || textBox1.Text == "Input a new name" ? oldFileName : textBox1.Text;
                        string fileExtension = string.IsNullOrEmpty(extensionTextBox.Text) ? new FileInfo(oldFileName).Extension : extensionTextBox.Text;
                        string newFileName = Path.Combine(Path.GetDirectoryName(oldFileName), Path.GetFileNameWithoutExtension(name)) + startingNumber + (fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension);

                        // Rename the file
                        File.Move(oldFileName, newFileName);

                        // Increment the starting number
                        intValue++;
                        startingNumber = intValue.ToString().PadLeft(startingNumber.Length, '0');
                    }
                }
                else if (rbPlusOriginalFileName.Checked)
                {
                    // Rename each file in the ListOfFiles using the original file name as a prefix
                    foreach (string fileName in ListOfFiles)
                    {
                        // Get the prefix, file extension, and create the new file name
                        string prefix = string.IsNullOrEmpty(textBox3.Text) || textBox3.Text == "Input a new name" ? "" : textBox3.Text;
                        string fileExtension = string.IsNullOrEmpty(extensionTextBox.Text) ? new FileInfo(fileName).Extension : extensionTextBox.Text;
                        string newFileName = Path.Combine(Path.GetDirectoryName(fileName), prefix + Path.GetFileNameWithoutExtension(fileName)) + (fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension);

                        // Rename the file
                        File.Move(fileName, newFileName);
                    }
                }

                // Get the last folder path from the ListOfFiles and populate the ListView
                string lastFolderPath = Path.GetDirectoryName(ListOfFiles[ListOfFiles.Count - 1]);
                mainForm.PopulateListView(lastFolderPath);
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Close the form
            this.Close();
        }


        /// <summary>
        /// Allows only digits and backspace key to be entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartNumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the entered character is a digit or backspace key
            // If not a digit or backspace key, mark the event as handled to prevent the character from being entered
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == '\b');
        }


        #region PlaceHolder For TextBox1, StartNumberTextBox, and TextBox3

        /// <summary>
        /// Clears the text if it is set to the default value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox1_Enter(object sender, EventArgs e)
        {
            // Check if the TextBox1 text is set to the default value
            if (textBox1.Text == "Input a new name")
            {
                // Clear the text
                textBox1.Text = string.Empty;
                // Change the text color to the default color
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }


        /// <summary>
        /// Sets the default value if the text is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox1_Leave(object sender, EventArgs e)
        {
            // Check if the TextBox1 text is empty
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                // Set the default value
                textBox1.Text = "Input a new name";
                // Set the text color to gray
                textBox1.ForeColor = SystemColors.GrayText;
            }
        }


        /// <summary>
        /// Clears the text if it is set to the default value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartNumberTextBox_Enter(object sender, EventArgs e)
        {
            // Check if the StartNumberTextBox text is set to the default value
            if (startNumberTextBox.Text == "1,01,001...")
            {
                // Clear the text
                startNumberTextBox.Text = string.Empty;
                // Change the text color to the default color
                startNumberTextBox.ForeColor = SystemColors.WindowText;
            }
        }


        /// <summary>
        /// Sets the default value if the text is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartNumberTextBox_Leave(object sender, EventArgs e)
        {
            // Check if the StartNumberTextBox text is empty
            if (string.IsNullOrEmpty(startNumberTextBox.Text))
            {
                // Set the default value
                startNumberTextBox.Text = "1,01,001...";
                // Set the text color to gray
                startNumberTextBox.ForeColor = SystemColors.GrayText;
            }
        }


        /// <summary>
        /// Clears the text if it is set to the default value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox3_Enter(object sender, EventArgs e)
        {
            // Check if the TextBox3 text is set to the default value
            if (textBox3.Text == "Input a new name")
            {
                // Clear the text
                textBox3.Text = string.Empty;
                // Change the text color to the default color
                textBox3.ForeColor = SystemColors.WindowText;
            }
        }


        /// <summary>
        /// Sets the default value if the text is empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox3_Leave(object sender, EventArgs e)
        {
            // Check if the TextBox3 text is empty
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                // Set the default value
                textBox3.Text = "Input a new name";
                // Set the text color to gray
                textBox3.ForeColor = SystemColors.GrayText;
            }
        }

        #endregion


        /// <summary>
        /// Closes the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, EventArgs e) => Close();

        #endregion

    }
}
