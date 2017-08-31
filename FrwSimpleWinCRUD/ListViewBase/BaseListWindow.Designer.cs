namespace FrwSoftware
{
    partial class BaseListWindow
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
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.viewButton = new System.Windows.Forms.ToolStripButton();
            this.newButton = new System.Windows.Forms.ToolStripButton();
            this.editButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.copyToClipboardButton = new System.Windows.Forms.ToolStripButton();
            this.listSettingsButton = new System.Windows.Forms.ToolStripButton();
            this.dialogViewButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenu.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenu.Name = "contextMenuStrip1";
            this.contextMenu.Size = new System.Drawing.Size(212, 28);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(211, 24);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1093, 406);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1093, 433);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshButton,
            this.viewButton,
            this.newButton,
            this.editButton,
            this.deleteButton,
            this.copyToClipboardButton,
            this.listSettingsButton,
            this.dialogViewButton,
            this.saveButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(228, 27);
            this.toolStrip.TabIndex = 0;
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::FrwSoftware.Properties.Resources.AllPics_01;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(24, 24);
            this.refreshButton.Text = global::FrwSoftware.FrwCRUDRes.List_Refresh_List;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // viewButton
            // 
            this.viewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewButton.Image = global::FrwSoftware.Properties.Resources.AllPics_08;
            this.viewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(24, 24);
            this.viewButton.Text = global::FrwSoftware.FrwCRUDRes.List_Details_Info;
            this.viewButton.Click += new System.EventHandler(this.viewButton_Click);
            // 
            // newButton
            // 
            this.newButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newButton.Image = global::FrwSoftware.Properties.Resources.AllPics_05;
            this.newButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(24, 24);
            this.newButton.Text = global::FrwSoftware.FrwCRUDRes.List_Add_Record;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // editButton
            // 
            this.editButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editButton.Image = global::FrwSoftware.Properties.Resources.AllPics_09;
            this.editButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(24, 24);
            this.editButton.Text = global::FrwSoftware.FrwCRUDRes.List_Edit_Record;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::FrwSoftware.Properties.Resources.AllPics_06;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(24, 24);
            this.deleteButton.Text = global::FrwSoftware.FrwCRUDRes.List_Remove_Record;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // copyToClipboardButton
            // 
            this.copyToClipboardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToClipboardButton.Image = global::FrwSoftware.Properties.Resources.AllPics_11;
            this.copyToClipboardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToClipboardButton.Name = "copyToClipboardButton";
            this.copyToClipboardButton.Size = new System.Drawing.Size(24, 24);
            this.copyToClipboardButton.Text = global::FrwSoftware.FrwCRUDRes.List_Copy_To_Clipboard;
            this.copyToClipboardButton.Click += new System.EventHandler(this.copyToClipboardButton_Click);
            // 
            // listSettingsButton
            // 
            this.listSettingsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.listSettingsButton.Image = global::FrwSoftware.Properties.Resources.AllPics_02;
            this.listSettingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.listSettingsButton.Name = "listSettingsButton";
            this.listSettingsButton.Size = new System.Drawing.Size(24, 24);
            this.listSettingsButton.Text = global::FrwSoftware.FrwCRUDRes.List_Settings;
            this.listSettingsButton.Click += new System.EventHandler(this.listSettingsButton_Click);
            // 
            // dialogViewButton
            // 
            this.dialogViewButton.CheckOnClick = true;
            this.dialogViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dialogViewButton.Image = global::FrwSoftware.Properties.Resources.AllPics_04;
            this.dialogViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dialogViewButton.Name = "dialogViewButton";
            this.dialogViewButton.Size = new System.Drawing.Size(24, 24);
            this.dialogViewButton.Text = global::FrwSoftware.FrwCRUDRes.List_Open_In_Dialog;
            this.dialogViewButton.Click += new System.EventHandler(this.dialogViewButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Image = global::FrwSoftware.Properties.Resources.AllPics_12;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(24, 24);
            this.saveButton.Text = global::FrwSoftware.FrwCRUDRes.List_Save_List;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // BaseListWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "BaseListWindow";
            this.Size = new System.Drawing.Size(1093, 433);
            this.contextMenu.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        protected System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;

        #endregion


        protected System.Windows.Forms.ToolStripContainer toolStripContainer1;
        protected System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton viewButton;
        private System.Windows.Forms.ToolStripButton newButton;
        private System.Windows.Forms.ToolStripButton editButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton copyToClipboardButton;
        private System.Windows.Forms.ToolStripButton listSettingsButton;
        private System.Windows.Forms.ToolStripButton dialogViewButton;
        private System.Windows.Forms.ToolStripButton saveButton;
    }
}