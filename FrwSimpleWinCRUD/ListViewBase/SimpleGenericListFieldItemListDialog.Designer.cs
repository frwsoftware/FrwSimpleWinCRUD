namespace FrwSoftware
{
    partial class SimpleGenericListFieldItemListDialog
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
            this.setNullButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cancelButtion
            // 
            this.cancelButtion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButtion.Location = new System.Drawing.Point(974, 614);
            this.cancelButtion.Name = "cancelButtion";
            this.cancelButtion.Size = new System.Drawing.Size(164, 44);
            this.cancelButtion.TabIndex = 5;
            this.cancelButtion.Text = global::FrwSoftware.FrwCRUDRes.Cancel;
            this.cancelButtion.UseVisualStyleBackColor = true;
            this.cancelButtion.Click += new System.EventHandler(this.cancelButtion_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(791, 614);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(164, 44);
            this.okButton.TabIndex = 4;
            this.okButton.Text = global::FrwSoftware.FrwCRUDRes.OK;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1154, 574);
            this.panel1.TabIndex = 3;
            // 
            // setNullButton
            // 
            this.setNullButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.setNullButton.Location = new System.Drawing.Point(601, 614);
            this.setNullButton.Name = "setNullButton";
            this.setNullButton.Size = new System.Drawing.Size(164, 44);
            this.setNullButton.TabIndex = 6;
            this.setNullButton.Text = global::FrwSoftware.FrwCRUDRes.Set_NULL;
            this.setNullButton.UseVisualStyleBackColor = true;
            this.setNullButton.Visible = false;
            this.setNullButton.Click += new System.EventHandler(this.setNullButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(13, 614);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(160, 44);
            this.deleteButton.TabIndex = 10;
            this.deleteButton.Text = global::FrwSoftware.FrwCRUDRes.Remove;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // SimpleGenericListFieldItemListDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 666);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.setNullButton);
            this.Controls.Add(this.cancelButtion);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.Name = "SimpleGenericListFieldItemListDialog";
            this.ShowIcon = false;
            this.Text = "##List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleListDialog_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SimpleListDialog_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButtion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button setNullButton;
        private System.Windows.Forms.Button deleteButton;
    }
}