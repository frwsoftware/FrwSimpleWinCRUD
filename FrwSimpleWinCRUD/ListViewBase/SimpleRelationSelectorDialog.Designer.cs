using BrightIdeasSoftware;

namespace FrwSoftware
{
    partial class SimpleRelationSelectorDialog
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
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButtion
            // 
            this.cancelButtion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButtion.Location = new System.Drawing.Point(520, 676);
            this.cancelButtion.Name = "cancelButtion";
            this.cancelButtion.Size = new System.Drawing.Size(164, 44);
            this.cancelButtion.TabIndex = 8;
            this.cancelButtion.Text = global::FrwSoftware.FrwCRUDRes.Cancel;
            this.cancelButtion.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(351, 676);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(164, 44);
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
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(707, 574);
            this.panel1.TabIndex = 6;
            // 
            // listView
            // 
            this.listView.CellEditUseWholeCell = false;
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(707, 574);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // SimpleRelationSelectorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 736);
            this.Controls.Add(this.cancelButtion);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.Name = "SimpleRelationSelectorDialog";
            this.ShowIcon = false;
            this.Text = "##Dictionary";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButtion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel1;
        private ObjectListView listView;
    }
}