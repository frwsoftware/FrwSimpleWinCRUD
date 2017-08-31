namespace FrwSoftware
{ 

    partial class SimpleAttachmentsDialog
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
            this.listView = new BrightIdeasSoftware.ObjectListView();
            this.deleteButton = new System.Windows.Forms.Button();
            this.cancelButtion = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.addFilePathButton = new System.Windows.Forms.Button();
            this.addFileButton = new System.Windows.Forms.Button();
            this.addURLButton = new System.Windows.Forms.Button();
            this.addDirPathButton = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.openDirButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.CellEditUseWholeCell = false;
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Location = new System.Drawing.Point(3, 1);
            this.listView.Margin = new System.Windows.Forms.Padding(2);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(772, 364);
            this.listView.TabIndex = 11;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Location = new System.Drawing.Point(648, 395);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(116, 29);
            this.deleteButton.TabIndex = 14;
            this.deleteButton.Text = global::FrwSoftware.FrwCRUDRes.Delete;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // cancelButtion
            // 
            this.cancelButtion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButtion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButtion.Location = new System.Drawing.Point(648, 511);
            this.cancelButtion.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButtion.Name = "cancelButtion";
            this.cancelButtion.Size = new System.Drawing.Size(119, 29);
            this.cancelButtion.TabIndex = 13;
            this.cancelButtion.Text = global::FrwSoftware.FrwCRUDRes.Cancel;
            this.cancelButtion.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(522, 511);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(119, 29);
            this.okButton.TabIndex = 12;
            this.okButton.Text = global::FrwSoftware.FrwCRUDRes.OK;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // addFilePathButton
            // 
            this.addFilePathButton.Location = new System.Drawing.Point(12, 430);
            this.addFilePathButton.Name = "addFilePathButton";
            this.addFilePathButton.Size = new System.Drawing.Size(207, 33);
            this.addFilePathButton.TabIndex = 15;
            this.addFilePathButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleAttachmentsDialog_AddRef;
            this.addFilePathButton.UseVisualStyleBackColor = true;
            this.addFilePathButton.Click += new System.EventHandler(this.addFilePathButton_Click);
            // 
            // addFileButton
            // 
            this.addFileButton.Location = new System.Drawing.Point(12, 391);
            this.addFileButton.Name = "addFileButton";
            this.addFileButton.Size = new System.Drawing.Size(207, 33);
            this.addFileButton.TabIndex = 16;
            this.addFileButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleAttachmentsDialog_AddFileCopy;
            this.addFileButton.UseVisualStyleBackColor = true;
            this.addFileButton.Click += new System.EventHandler(this.addFileButton_Click);
            // 
            // addURLButton
            // 
            this.addURLButton.Location = new System.Drawing.Point(225, 391);
            this.addURLButton.Name = "addURLButton";
            this.addURLButton.Size = new System.Drawing.Size(207, 33);
            this.addURLButton.TabIndex = 17;
            this.addURLButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleAttachmentsDialog_AddUrl;
            this.addURLButton.UseVisualStyleBackColor = true;
            this.addURLButton.Click += new System.EventHandler(this.addURLButton_Click);
            // 
            // addDirPathButton
            // 
            this.addDirPathButton.Location = new System.Drawing.Point(225, 430);
            this.addDirPathButton.Name = "addDirPathButton";
            this.addDirPathButton.Size = new System.Drawing.Size(207, 33);
            this.addDirPathButton.TabIndex = 18;
            this.addDirPathButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleAttachmentsDialog_AddFolderRef;
            this.addDirPathButton.UseVisualStyleBackColor = true;
            this.addDirPathButton.Click += new System.EventHandler(this.addDirPathButton_Click);
            // 
            // openFileButton
            // 
            this.openFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openFileButton.Location = new System.Drawing.Point(19, 511);
            this.openFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(119, 29);
            this.openFileButton.TabIndex = 19;
            this.openFileButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleAttachmentsDialog_OpenFile;
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // openDirButton
            // 
            this.openDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openDirButton.Location = new System.Drawing.Point(320, 511);
            this.openDirButton.Margin = new System.Windows.Forms.Padding(2);
            this.openDirButton.Name = "openDirButton";
            this.openDirButton.Size = new System.Drawing.Size(119, 29);
            this.openDirButton.TabIndex = 20;
            this.openDirButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleAttachmentsDialog_OpenFolder;
            this.openDirButton.UseVisualStyleBackColor = true;
            this.openDirButton.Click += new System.EventHandler(this.openDirButton_Click);
            // 
            // SimpleAttachmentsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 551);
            this.Controls.Add(this.openDirButton);
            this.Controls.Add(this.openFileButton);
            this.Controls.Add(this.addDirPathButton);
            this.Controls.Add(this.addURLButton);
            this.Controls.Add(this.addFileButton);
            this.Controls.Add(this.addFilePathButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cancelButtion);
            this.Controls.Add(this.okButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SimpleAttachmentsDialog";
            this.ShowIcon = false;
            this.Text = "##Attacments";
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView listView;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button cancelButtion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button addFilePathButton;
        private System.Windows.Forms.Button addFileButton;
        private System.Windows.Forms.Button addURLButton;
        private System.Windows.Forms.Button addDirPathButton;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.Button openDirButton;
    }
}