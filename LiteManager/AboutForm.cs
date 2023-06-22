using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteManager
{
    public partial class AboutForm : Form
    {

        private readonly string description = @"LiteManager is a user-friendly file manager that makes it easy to organize and manage your files. With LiteManager, you can navigate through your computer's drives and folders effortlessly. It allows you to perform essential file operations like copying, moving, renaming, and deleting files and folders with just a few clicks. You can also select multiple files at once for faster operations like batch renaming.

LiteManager provides a preview feature that allows you to view images right within the application. This eliminates the need to open external programs to view your files. The application also keeps track of your navigation history, making it convenient to go back to previously visited directories.

Experience hassle-free file management with LiteManager. Download the application now and enjoy efficient organization and management of your files on your computer.
";

        public AboutForm()
        {
            InitializeComponent();
        }
        

        private void AboutForm_Load(object sender, EventArgs e)
        {
            richTextBoxAboutDescription.Text = description;
        }
    }
}
