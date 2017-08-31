namespace FrwSoftware
{
    partial class AppSettingsWindow
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
            Flobbster.Windows.Forms.PropertyBag propertyBag1 = new Flobbster.Windows.Forms.PropertyBag();
            this.appSettingsPropertyGrid1 = new AppSettingsPropertyGrid();
            this.SuspendLayout();
            // 
            // appSettingsPropertyGrid1
            // 
            this.appSettingsPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appSettingsPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.appSettingsPropertyGrid1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.appSettingsPropertyGrid1.Name = "appSettingsPropertyGrid1";
            this.appSettingsPropertyGrid1.SelectedObject = propertyBag1;
            this.appSettingsPropertyGrid1.Size = new System.Drawing.Size(449, 289);
            this.appSettingsPropertyGrid1.TabIndex = 0;
            // 
            // AppSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 289);
            this.Controls.Add(this.appSettingsPropertyGrid1);
            //this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "AppSettingsWindow";
            this.Text = "AppSettingsWindow";
            this.Load += new System.EventHandler(this.AppSettingsWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private AppSettingsPropertyGrid appSettingsPropertyGrid1;
    }
}