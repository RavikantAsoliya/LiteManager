namespace LiteManager
{
    partial class BatchRenameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.startNumberTextBox = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.extensionTextBox = new System.Windows.Forms.TextBox();
            this.rbPlusNumber = new System.Windows.Forms.RadioButton();
            this.rbPlusOriginalFileName = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBox1.Location = new System.Drawing.Point(7, 11);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(298, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.TabStop = false;
            this.textBox1.Text = "Input a new name";
            this.textBox1.Enter += new System.EventHandler(this.TextBox1_Enter);
            this.textBox1.Leave += new System.EventHandler(this.TextBox1_Leave);
            // 
            // startNumberTextBox
            // 
            this.startNumberTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startNumberTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.startNumberTextBox.Location = new System.Drawing.Point(93, 41);
            this.startNumberTextBox.Name = "startNumberTextBox";
            this.startNumberTextBox.Size = new System.Drawing.Size(308, 23);
            this.startNumberTextBox.TabIndex = 0;
            this.startNumberTextBox.TabStop = false;
            this.startNumberTextBox.Text = "1,01,001...";
            this.startNumberTextBox.Enter += new System.EventHandler(this.StartNumberTextBox_Enter);
            this.startNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartNumberTextBox_KeyPress);
            this.startNumberTextBox.Leave += new System.EventHandler(this.StartNumberTextBox_Leave);
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBox3.Location = new System.Drawing.Point(7, 91);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(254, 23);
            this.textBox3.TabIndex = 0;
            this.textBox3.TabStop = false;
            this.textBox3.Text = "Input a new name";
            this.textBox3.Enter += new System.EventHandler(this.TextBox3_Enter);
            this.textBox3.Leave += new System.EventHandler(this.TextBox3_Leave);
            // 
            // extensionTextBox
            // 
            this.extensionTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.extensionTextBox.Location = new System.Drawing.Point(158, 140);
            this.extensionTextBox.Name = "extensionTextBox";
            this.extensionTextBox.Size = new System.Drawing.Size(243, 23);
            this.extensionTextBox.TabIndex = 0;
            this.extensionTextBox.TabStop = false;
            // 
            // rbPlusNumber
            // 
            this.rbPlusNumber.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbPlusNumber.Checked = true;
            this.rbPlusNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPlusNumber.Location = new System.Drawing.Point(311, 11);
            this.rbPlusNumber.Name = "rbPlusNumber";
            this.rbPlusNumber.Size = new System.Drawing.Size(90, 24);
            this.rbPlusNumber.TabIndex = 0;
            this.rbPlusNumber.TabStop = true;
            this.rbPlusNumber.Text = "+ Number";
            this.rbPlusNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPlusNumber.UseVisualStyleBackColor = true;
            this.rbPlusNumber.CheckedChanged += new System.EventHandler(this.RbPlusNumber_CheckedChanged);
            // 
            // rbPlusOriginalFileName
            // 
            this.rbPlusOriginalFileName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbPlusOriginalFileName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPlusOriginalFileName.ForeColor = System.Drawing.SystemColors.GrayText;
            this.rbPlusOriginalFileName.Location = new System.Drawing.Point(267, 91);
            this.rbPlusOriginalFileName.Name = "rbPlusOriginalFileName";
            this.rbPlusOriginalFileName.Size = new System.Drawing.Size(134, 24);
            this.rbPlusOriginalFileName.TabIndex = 0;
            this.rbPlusOriginalFileName.Text = "+ Original Filename";
            this.rbPlusOriginalFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPlusOriginalFileName.UseVisualStyleBackColor = true;
            this.rbPlusOriginalFileName.CheckedChanged += new System.EventHandler(this.RbPlusOriginalFileName_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Number";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Rename Extension Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.label3.Location = new System.Drawing.Point(7, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(395, 1);
            this.label3.TabIndex = 0;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.label4.Location = new System.Drawing.Point(7, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(395, 1);
            this.label4.TabIndex = 0;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.label5.Location = new System.Drawing.Point(7, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(395, 1);
            this.label5.TabIndex = 0;
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(77, 185);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(105, 25);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.TabStop = false;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(234, 185);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(97, 25);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.TabStop = false;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(206, 186);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 20);
            this.panel1.TabIndex = 13;
            // 
            // BatchRenameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 219);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbPlusOriginalFileName);
            this.Controls.Add(this.rbPlusNumber);
            this.Controls.Add(this.extensionTextBox);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.startNumberTextBox);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BatchRenameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BatchRenameForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox startNumberTextBox;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox extensionTextBox;
        private System.Windows.Forms.RadioButton rbPlusNumber;
        private System.Windows.Forms.RadioButton rbPlusOriginalFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel1;
    }
}