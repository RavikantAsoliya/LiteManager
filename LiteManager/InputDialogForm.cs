using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LiteManager
{
    public partial class InputDialogForm : Form
    {
        public string InputValue { get; private set; }
        public bool Accepted { get; private set; }
        public string Title { get; set; }
        public string Prompt { get; set; }
        public string InputText { get; private set; }


        public InputDialogForm()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            InputValue = textBox.Text;
            Accepted = true;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Accepted = false;
            Close();
        }
    }
}
