namespace FrwSoftware
{
    partial class ConsoleAdvancedWindow
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
            this.consoleControlAdvanced1 = new ConsoleControlSample.ConsoleControlAdvanced();
            this.SuspendLayout();
            // 
            // consoleControlAdvanced1
            // 
            this.consoleControlAdvanced1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleControlAdvanced1.Location = new System.Drawing.Point(0, 0);
            this.consoleControlAdvanced1.Margin = new System.Windows.Forms.Padding(6);
            this.consoleControlAdvanced1.Name = "consoleControlAdvanced1";
            this.consoleControlAdvanced1.Size = new System.Drawing.Size(803, 461);
            this.consoleControlAdvanced1.TabIndex = 0;
            // 
            // ConsoleAdvancedWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.consoleControlAdvanced1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "ConsoleAdvancedWindow";
            this.Size = new System.Drawing.Size(803, 461);
            this.ResumeLayout(false);

        }

        #endregion

        private ConsoleControlSample.ConsoleControlAdvanced consoleControlAdvanced1;
    }
}