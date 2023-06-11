
using System;
using System.Windows.Forms;

namespace LiteManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.driveTreeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.extraLargeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mediumIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailedInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCheckBoxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.countToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.selectedItemsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.topToolStrip = new System.Windows.Forms.ToolStrip();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.upButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addressBar = new System.Windows.Forms.ToolStripComboBox();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.searchToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileListView = new System.Windows.Forms.ListView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTextDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.topToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // driveTreeView
            // 
            this.driveTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.driveTreeView.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.driveTreeView.ImageIndex = 0;
            this.driveTreeView.ImageList = this.imageList;
            this.driveTreeView.Location = new System.Drawing.Point(0, 49);
            this.driveTreeView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.driveTreeView.Name = "driveTreeView";
            this.driveTreeView.SelectedImageIndex = 0;
            this.driveTreeView.Size = new System.Drawing.Size(233, 390);
            this.driveTreeView.TabIndex = 0;
            this.driveTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.DriveTreeView_BeforeExpand);
            this.driveTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DriveTreeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "disk_light.png");
            this.imageList.Images.SetKeyName(1, "cd-rom_light.png");
            this.imageList.Images.SetKeyName(2, "pendrive_light.png");
            this.imageList.Images.SetKeyName(3, "folder.png");
            this.imageList.Images.SetKeyName(4, "recent_light.png");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.viewToolStripMenuItem1,
            this.refreshToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.batchRenameToolStripMenuItem,
            this.newToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(181, 318);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_Opening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem1
            // 
            this.viewToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extraLargeIconsToolStripMenuItem,
            this.largeIconsToolStripMenuItem,
            this.mediumIconsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.tileToolStripMenuItem,
            this.listToolStripMenuItem,
            this.detailedInfoToolStripMenuItem});
            this.viewToolStripMenuItem1.Name = "viewToolStripMenuItem1";
            this.viewToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.viewToolStripMenuItem1.Text = "View";
            // 
            // extraLargeIconsToolStripMenuItem
            // 
            this.extraLargeIconsToolStripMenuItem.Name = "extraLargeIconsToolStripMenuItem";
            this.extraLargeIconsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.extraLargeIconsToolStripMenuItem.Text = "Extra Large Icons";
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.largeIconsToolStripMenuItem.Text = "Large Icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
            // 
            // mediumIconsToolStripMenuItem
            // 
            this.mediumIconsToolStripMenuItem.Name = "mediumIconsToolStripMenuItem";
            this.mediumIconsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.mediumIconsToolStripMenuItem.Text = "Medium Icons";
            // 
            // smallIconsToolStripMenuItem
            // 
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.smallIconsToolStripMenuItem.Text = "Small Icons";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
            // 
            // tileToolStripMenuItem
            // 
            this.tileToolStripMenuItem.CheckOnClick = true;
            this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
            this.tileToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.tileToolStripMenuItem.Text = "Tile";
            this.tileToolStripMenuItem.Click += new System.EventHandler(this.TileToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.listToolStripMenuItem.Text = "List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // detailedInfoToolStripMenuItem
            // 
            this.detailedInfoToolStripMenuItem.Name = "detailedInfoToolStripMenuItem";
            this.detailedInfoToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.detailedInfoToolStripMenuItem.Text = "Detailed Info";
            this.detailedInfoToolStripMenuItem.Click += new System.EventHandler(this.detailedInfoToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.RefreshToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect All";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.DeselectAllToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.PropertiesToolStripMenuItem_Click);
            // 
            // batchRenameToolStripMenuItem
            // 
            this.batchRenameToolStripMenuItem.Name = "batchRenameToolStripMenuItem";
            this.batchRenameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.batchRenameToolStripMenuItem.Text = "Batch Rename";
            this.batchRenameToolStripMenuItem.Click += new System.EventHandler(this.BatchRenameToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.White;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.homeToolStripMenuItem,
            this.shareToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(984, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // homeToolStripMenuItem
            // 
            this.homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            this.homeToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.homeToolStripMenuItem.Text = "Home";
            // 
            // shareToolStripMenuItem
            // 
            this.shareToolStripMenuItem.Name = "shareToolStripMenuItem";
            this.shareToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.shareToolStripMenuItem.Text = "Share";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarToolStripMenuItem,
            this.statusbarToolStripMenuItem,
            this.itemCheckBoxesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // toolbarToolStripMenuItem
            // 
            this.toolbarToolStripMenuItem.Checked = true;
            this.toolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolbarToolStripMenuItem.Name = "toolbarToolStripMenuItem";
            this.toolbarToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.toolbarToolStripMenuItem.Text = "Toolbar";
            this.toolbarToolStripMenuItem.Click += new System.EventHandler(this.ToolbarToolStripMenuItem_Click);
            // 
            // statusbarToolStripMenuItem
            // 
            this.statusbarToolStripMenuItem.Checked = true;
            this.statusbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusbarToolStripMenuItem.Name = "statusbarToolStripMenuItem";
            this.statusbarToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.statusbarToolStripMenuItem.Text = "Statusbar";
            this.statusbarToolStripMenuItem.Click += new System.EventHandler(this.StatusbarToolStripMenuItem_Click);
            // 
            // itemCheckBoxesToolStripMenuItem
            // 
            this.itemCheckBoxesToolStripMenuItem.CheckOnClick = true;
            this.itemCheckBoxesToolStripMenuItem.Name = "itemCheckBoxesToolStripMenuItem";
            this.itemCheckBoxesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.itemCheckBoxesToolStripMenuItem.Text = "Item Check Boxes";
            this.itemCheckBoxesToolStripMenuItem.Click += new System.EventHandler(this.ItemCheckBoxesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.countToolStripStatusLabel,
            this.selectedItemsToolStripStatusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 439);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusBar.Size = new System.Drawing.Size(984, 22);
            this.statusBar.TabIndex = 3;
            this.statusBar.Text = "statusStrip";
            // 
            // countToolStripStatusLabel
            // 
            this.countToolStripStatusLabel.Name = "countToolStripStatusLabel";
            this.countToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // selectedItemsToolStripStatusLabel
            // 
            this.selectedItemsToolStripStatusLabel.Name = "selectedItemsToolStripStatusLabel";
            this.selectedItemsToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // topToolStrip
            // 
            this.topToolStrip.BackColor = System.Drawing.Color.White;
            this.topToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backButton,
            this.forwardButton,
            this.upButton,
            this.toolStripSeparator2,
            this.addressBar,
            this.refreshButton,
            this.searchToolStripComboBox});
            this.topToolStrip.Location = new System.Drawing.Point(0, 24);
            this.topToolStrip.Name = "topToolStrip";
            this.topToolStrip.Size = new System.Drawing.Size(984, 25);
            this.topToolStrip.TabIndex = 4;
            this.topToolStrip.Text = "toolStrip1";
            // 
            // backButton
            // 
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backButton.Enabled = false;
            this.backButton.Image = global::LiteManager.Properties.Resources.back;
            this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(23, 22);
            this.backButton.Text = "toolStripButton1";
            this.backButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Enabled = false;
            this.forwardButton.Image = global::LiteManager.Properties.Resources.forward;
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(23, 22);
            this.forwardButton.Text = "toolStripButton2";
            this.forwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // upButton
            // 
            this.upButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.upButton.Image = global::LiteManager.Properties.Resources.up;
            this.upButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.upButton.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(23, 22);
            this.upButton.Text = "toolStripButton3";
            this.upButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // addressBar
            // 
            this.addressBar.AutoSize = false;
            this.addressBar.BackColor = System.Drawing.Color.White;
            this.addressBar.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.addressBar.ForeColor = System.Drawing.Color.Black;
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(687, 23);
            this.addressBar.ToolTipText = "AddressBar";
            this.addressBar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddressBar_KeyPress);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::LiteManager.Properties.Resources.refresh;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "toolStripButton4";
            this.refreshButton.Click += new System.EventHandler(this.RefreshToolStripMenuItem_Click);
            // 
            // searchToolStripComboBox
            // 
            this.searchToolStripComboBox.BackColor = System.Drawing.Color.White;
            this.searchToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.searchToolStripComboBox.Name = "searchToolStripComboBox";
            this.searchToolStripComboBox.Size = new System.Drawing.Size(170, 25);
            this.searchToolStripComboBox.Text = "Search";
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 100;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.Width = 100;
            // 
            // columnHeaderModified
            // 
            this.columnHeaderModified.Text = "Modified";
            this.columnHeaderModified.Width = 150;
            // 
            // fileListView
            // 
            this.fileListView.AllowDrop = true;
            this.fileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderType,
            this.columnHeaderSize,
            this.columnHeaderModified});
            this.fileListView.ContextMenuStrip = this.contextMenuStrip;
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.fileListView.FullRowSelect = true;
            this.fileListView.HideSelection = false;
            this.fileListView.LabelEdit = true;
            this.fileListView.Location = new System.Drawing.Point(233, 49);
            this.fileListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(751, 390);
            this.fileListView.SmallImageList = this.imageList;
            this.fileListView.TabIndex = 1;
            this.fileListView.TileSize = new System.Drawing.Size(100, 100);
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.Details;
            this.fileListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.FileListView_AfterLabelEdit);
            this.fileListView.SelectedIndexChanged += new System.EventHandler(this.fileListView_SelectedIndexChanged);
            this.fileListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.fileListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView_DragEnter);
            this.fileListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView_KeyDown);
            this.fileListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseClick);
            this.fileListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Location = new System.Drawing.Point(233, 49);
            this.splitter1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 390);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.newTextDocumentToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newFolderToolStripMenuItem.Text = "Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.NewFolderToolStripMenuItem_Click);
            // 
            // newTextDocumentToolStripMenuItem
            // 
            this.newTextDocumentToolStripMenuItem.Name = "newTextDocumentToolStripMenuItem";
            this.newTextDocumentToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newTextDocumentToolStripMenuItem.Text = "Text Document";
            this.newTextDocumentToolStripMenuItem.Click += new System.EventHandler(this.NewTextDocumentToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 461);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.fileListView);
            this.Controls.Add(this.driveTreeView);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.topToolStrip);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "File Manager";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_Resize);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.topToolStrip.ResumeLayout(false);
            this.topToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView driveTreeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel selectedItemsToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel countToolStripStatusLabel;
        private ToolStrip topToolStrip;
        private ToolStripButton backButton;
        private ToolStripButton forwardButton;
        private ToolStripButton upButton;
        private System.Windows.Forms.ToolStripComboBox addressBar;
        private ToolStripButton refreshButton;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderType;
        private ColumnHeader columnHeaderSize;
        private ColumnHeader columnHeaderModified;
        private ListView fileListView;
        private ToolStripMenuItem homeToolStripMenuItem;
        private ToolStripMenuItem shareToolStripMenuItem;
        private ToolStripMenuItem batchRenameToolStripMenuItem;
        private Splitter splitter1;
        private ToolStripMenuItem viewToolStripMenuItem1;
        private ToolStripMenuItem extraLargeIconsToolStripMenuItem;
        private ToolStripMenuItem largeIconsToolStripMenuItem;
        private ToolStripMenuItem mediumIconsToolStripMenuItem;
        private ToolStripMenuItem smallIconsToolStripMenuItem;
        private ToolStripMenuItem toolbarToolStripMenuItem;
        private ToolStripMenuItem statusbarToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripComboBox searchToolStripComboBox;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem itemCheckBoxesToolStripMenuItem;
        private ToolStripMenuItem tileToolStripMenuItem;
        private ToolStripMenuItem listToolStripMenuItem;
        private ToolStripMenuItem detailedInfoToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem newFolderToolStripMenuItem;
        private ToolStripMenuItem newTextDocumentToolStripMenuItem;
    }
}
