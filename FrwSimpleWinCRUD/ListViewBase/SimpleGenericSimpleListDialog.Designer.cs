using BrightIdeasSoftware;

namespace FrwSoftware
{
    partial class SimpleGenericSimpleListDialog
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
            this.cancelButtion = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView = new BrightIdeasSoftware.ObjectListView();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.setNullButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButtion
            // 
            this.cancelButtion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButtion.Location = new System.Drawing.Point(505, 374);
            this.cancelButtion.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButtion.Name = "cancelButtion";
            this.cancelButtion.Size = new System.Drawing.Size(119, 29);
            this.cancelButtion.TabIndex = 8;
            this.cancelButtion.Text = global::FrwSoftware.FrwCRUDRes.Cancel;
            this.cancelButtion.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(379, 374);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(119, 29);
            this.okButton.TabIndex = 7;
            this.okButton.Text = global::FrwSoftware.FrwCRUDRes.OK;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(635, 246);
            this.panel1.TabIndex = 6;
            // 
            // listView
            // 
            this.listView.CellEditUseWholeCell = false;
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Margin = new System.Windows.Forms.Padding(2);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(635, 246);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(0, 312);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(116, 29);
            this.deleteButton.TabIndex = 9;
            this.deleteButton.Text = global::FrwSoftware.FrwCRUDRes.Remove;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(134, 312);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(116, 29);
            this.addButton.TabIndex = 10;
            this.addButton.Text = global::FrwSoftware.FrwCRUDRes.Add;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // moveDownButton
            // 
            this.moveDownButton.Location = new System.Drawing.Point(508, 312);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(116, 29);
            this.moveDownButton.TabIndex = 14;
            this.moveDownButton.Text = global::FrwSoftware.FrwCRUDRes.MoveDown;
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Location = new System.Drawing.Point(379, 312);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(116, 29);
            this.moveUpButton.TabIndex = 13;
            this.moveUpButton.Text = global::FrwSoftware.FrwCRUDRes.MoveUp;
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // setNullButton
            // 
            this.setNullButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.setNullButton.Location = new System.Drawing.Point(134, 374);
            this.setNullButton.Margin = new System.Windows.Forms.Padding(2);
            this.setNullButton.Name = "setNullButton";
            this.setNullButton.Size = new System.Drawing.Size(119, 29);
            this.setNullButton.TabIndex = 15;
            this.setNullButton.Text = global::FrwSoftware.FrwCRUDRes.Set_NULL;
            this.setNullButton.UseVisualStyleBackColor = true;
            this.setNullButton.Visible = false;
            this.setNullButton.Click += new System.EventHandler(this.setNullButton_Click);
            // 
            // SimpleGenericSimpleListDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 418);
            this.Controls.Add(this.setNullButton);
            this.Controls.Add(this.moveDownButton);
            this.Controls.Add(this.moveUpButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.cancelButtion);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SimpleGenericSimpleListDialog";
            this.ShowIcon = false;
            this.Text = "##Selection list of dictionary";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButtion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button addButton;
        private ObjectListView listView;
        private System.Windows.Forms.Button moveDownButton;
        private System.Windows.Forms.Button moveUpButton;
        private System.Windows.Forms.Button setNullButton;
    }
}