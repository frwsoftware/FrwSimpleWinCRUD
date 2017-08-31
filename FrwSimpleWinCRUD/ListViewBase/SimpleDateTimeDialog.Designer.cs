namespace FrwSoftware
{
    partial class SimpleDateTimeDialog
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.cancelButtion = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.clearTimeButton = new System.Windows.Forms.Button();
            this.nullButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker1.Location = new System.Drawing.Point(45, 11);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Size = new System.Drawing.Size(181, 22);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // cancelButtion
            // 
            this.cancelButtion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButtion.Location = new System.Drawing.Point(191, 319);
            this.cancelButtion.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButtion.Name = "cancelButtion";
            this.cancelButtion.Size = new System.Drawing.Size(88, 29);
            this.cancelButtion.TabIndex = 7;
            this.cancelButtion.Text = global::FrwSoftware.FrwCRUDRes.Cancel;
            this.cancelButtion.UseVisualStyleBackColor = true;
            this.cancelButtion.Click += new System.EventHandler(this.cancelButtion_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(-1, 319);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(90, 29);
            this.okButton.TabIndex = 6;
            this.okButton.Text = global::FrwSoftware.FrwCRUDRes.OK;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(45, 98);
            this.monthCalendar1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 8;
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // clearTimeButton
            // 
            this.clearTimeButton.Location = new System.Drawing.Point(45, 45);
            this.clearTimeButton.Margin = new System.Windows.Forms.Padding(2);
            this.clearTimeButton.Name = "clearTimeButton";
            this.clearTimeButton.Size = new System.Drawing.Size(180, 29);
            this.clearTimeButton.TabIndex = 9;
            this.clearTimeButton.Text = global::FrwSoftware.FrwCRUDRes.SimpleDateTimeDialog_ClearTime;
            this.clearTimeButton.UseVisualStyleBackColor = true;
            this.clearTimeButton.Click += new System.EventHandler(this.clearTimeButton_Click);
            // 
            // nullButton
            // 
            this.nullButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.nullButton.Location = new System.Drawing.Point(97, 319);
            this.nullButton.Margin = new System.Windows.Forms.Padding(2);
            this.nullButton.Name = "nullButton";
            this.nullButton.Size = new System.Drawing.Size(90, 29);
            this.nullButton.TabIndex = 10;
            this.nullButton.Text = global::FrwSoftware.FrwCRUDRes.Clear;
            this.nullButton.UseVisualStyleBackColor = true;
            this.nullButton.Click += new System.EventHandler(this.nullButton_Click);
            // 
            // SimpleDateTimeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 350);
            this.Controls.Add(this.nullButton);
            this.Controls.Add(this.clearTimeButton);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.cancelButtion);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.dateTimePicker1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SimpleDateTimeDialog";
            this.ShowIcon = false;
            this.Text = "##Date and time";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button cancelButtion;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Button clearTimeButton;
        private System.Windows.Forms.Button nullButton;
    }
}