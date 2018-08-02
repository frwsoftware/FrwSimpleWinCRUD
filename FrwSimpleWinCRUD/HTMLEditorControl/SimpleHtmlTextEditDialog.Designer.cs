

namespace FrwSoftware
{
    partial class SimpleHtmlTextEditDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleHtmlTextEditDialog));
            this.cancelButtion = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.textEditorControl = new FrwSoftware.TextEditorControl();
            this.SuspendLayout();
            // 
            // cancelButtion
            // 
            resources.ApplyResources(this.cancelButtion, "cancelButtion");
            this.cancelButtion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButtion.Name = "cancelButtion";
            this.cancelButtion.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // textEditorControl
            // 
            resources.ApplyResources(this.textEditorControl, "textEditorControl");
            this.textEditorControl.EditedText = "";
            this.textEditorControl.Name = "textEditorControl";
            // 
            // SimpleHtmlTextEditDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textEditorControl);
            this.Controls.Add(this.cancelButtion);
            this.Controls.Add(this.okButton);
            this.MinimizeBox = false;
            this.Name = "SimpleHtmlTextEditDialog";
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button cancelButtion;
        private System.Windows.Forms.Button okButton;
        private TextEditorControl textEditorControl;
    }
}