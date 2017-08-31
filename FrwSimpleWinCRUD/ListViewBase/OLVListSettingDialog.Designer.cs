using FrwSoftware.Properties;

namespace FrwSoftware
{
    partial class OLVListSettingDialog
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
            this.labelEditable = new System.Windows.Forms.Label();
            this.labelViewType = new System.Windows.Forms.Label();
            this.comboBoxHotItemStyle = new System.Windows.Forms.ComboBox();
            this.comboBoxEditable = new System.Windows.Forms.ComboBox();
            this.labelHotItemStyle = new System.Windows.Forms.Label();
            this.comboBoxView = new System.Windows.Forms.ComboBox();
            this.checkBoxGroups = new System.Windows.Forms.CheckBox();
            this.rowHeightUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelRowHeight = new System.Windows.Forms.Label();
            this.autoCalcHeightCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.rowHeightUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label20
            // 
            this.labelEditable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelEditable.AutoSize = true;
            this.labelEditable.Location = new System.Drawing.Point(22, 127);
            this.labelEditable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelEditable.Name = "label20";
            this.labelEditable.Size = new System.Drawing.Size(94, 17);
            this.labelEditable.TabIndex = 42;
            this.labelEditable.Text = "##Editable type:";
            // 
            // label38
            // 
            this.labelViewType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelViewType.AutoSize = true;
            this.labelViewType.Location = new System.Drawing.Point(24, 54);
            this.labelViewType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelViewType.Name = "label38";
            this.labelViewType.Size = new System.Drawing.Size(85, 17);
            this.labelViewType.TabIndex = 40;
            this.labelViewType.Text = "##List View Type";
            // 
            // comboBoxHotItemStyle
            // 
            this.comboBoxHotItemStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxHotItemStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHotItemStyle.FormattingEnabled = true;
            this.comboBoxHotItemStyle.Items.AddRange(new object[] {
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_None,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_TextColor,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_Border,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_Translucent,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_Lightbox});
            this.comboBoxHotItemStyle.Location = new System.Drawing.Point(344, 90);
            this.comboBoxHotItemStyle.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxHotItemStyle.Name = "comboBoxHotItemStyle";
            this.comboBoxHotItemStyle.Size = new System.Drawing.Size(275, 24);
            this.comboBoxHotItemStyle.TabIndex = 41;
            this.comboBoxHotItemStyle.SelectedIndexChanged += new System.EventHandler(this.comboBoxHotItemStyle_SelectedIndexChanged);
            // 
            // comboBoxEditable
            // 
            this.comboBoxEditable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxEditable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEditable.FormattingEnabled = true;
            this.comboBoxEditable.Items.AddRange(new object[] {
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_No,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_SingleClick,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_DoubleClick,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_F2Only});
            this.comboBoxEditable.Location = new System.Drawing.Point(344, 127);
            this.comboBoxEditable.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxEditable.Name = "comboBoxEditable";
            this.comboBoxEditable.Size = new System.Drawing.Size(275, 24);
            this.comboBoxEditable.TabIndex = 37;
            this.comboBoxEditable.SelectedIndexChanged += new System.EventHandler(this.comboBoxEditable_SelectedIndexChanged);
            // 
            // label6
            // 
            this.labelHotItemStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelHotItemStyle.AutoSize = true;
            this.labelHotItemStyle.Location = new System.Drawing.Point(22, 93);
            this.labelHotItemStyle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHotItemStyle.Name = "label6";
            this.labelHotItemStyle.Size = new System.Drawing.Size(92, 17);
            this.labelHotItemStyle.TabIndex = 38;
            this.labelHotItemStyle.Text = "##Hot item type";
            // 
            // comboBoxView
            // 
            this.comboBoxView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxView.FormattingEnabled = true;
            this.comboBoxView.Items.AddRange(new object[] {
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_SmallIcon,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_LargeIcon,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_List,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_Tile,
            global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_Details});
            this.comboBoxView.Location = new System.Drawing.Point(344, 54);
            this.comboBoxView.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxView.Name = "comboBoxView";
            this.comboBoxView.Size = new System.Drawing.Size(275, 24);
            this.comboBoxView.TabIndex = 39;
            this.comboBoxView.SelectedIndexChanged += new System.EventHandler(this.comboBoxView_SelectedIndexChanged);
            // 
            // checkBoxGroups
            // 
            this.checkBoxGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxGroups.Location = new System.Drawing.Point(25, 175);
            this.checkBoxGroups.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxGroups.Name = "checkBoxGroups";
            this.checkBoxGroups.Size = new System.Drawing.Size(144, 26);
            this.checkBoxGroups.TabIndex = 36;
            this.checkBoxGroups.Text = global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_Groups;
            this.checkBoxGroups.UseVisualStyleBackColor = true;
            this.checkBoxGroups.CheckedChanged += new System.EventHandler(this.checkBoxGroups_CheckedChanged);
            // 
            // rowHeightUpDown
            // 
            this.rowHeightUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rowHeightUpDown.Location = new System.Drawing.Point(344, 24);
            this.rowHeightUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.rowHeightUpDown.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.rowHeightUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.rowHeightUpDown.Name = "rowHeightUpDown";
            this.rowHeightUpDown.Size = new System.Drawing.Size(85, 22);
            this.rowHeightUpDown.TabIndex = 44;
            this.rowHeightUpDown.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.labelRowHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRowHeight.AutoSize = true;
            this.labelRowHeight.Location = new System.Drawing.Point(24, 20);
            this.labelRowHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelRowHeight.Name = "label11";
            this.labelRowHeight.Size = new System.Drawing.Size(103, 17);
            this.labelRowHeight.TabIndex = 43;
            this.labelRowHeight.Text = "##List row height:";
            // 
            // autoCalcHeightCheckBox
            // 
            this.autoCalcHeightCheckBox.AutoSize = true;
            this.autoCalcHeightCheckBox.Location = new System.Drawing.Point(456, 26);
            this.autoCalcHeightCheckBox.Name = "autoCalcHeightCheckBox";
            this.autoCalcHeightCheckBox.Size = new System.Drawing.Size(135, 21);
            this.autoCalcHeightCheckBox.TabIndex = 45;
            this.autoCalcHeightCheckBox.Text = global::FrwSoftware.FrwCRUDRes.ListSettingDialogcs_AutoCalcHeight;
            this.autoCalcHeightCheckBox.UseVisualStyleBackColor = true;
            this.autoCalcHeightCheckBox.CheckedChanged += new System.EventHandler(this.autoCalcHeightCheckBox_CheckedChanged);
            // 
            // OLVListSettingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 237);
            this.Controls.Add(this.autoCalcHeightCheckBox);
            this.Controls.Add(this.rowHeightUpDown);
            this.Controls.Add(this.labelRowHeight);
            this.Controls.Add(this.labelEditable);
            this.Controls.Add(this.labelViewType);
            this.Controls.Add(this.comboBoxHotItemStyle);
            this.Controls.Add(this.comboBoxEditable);
            this.Controls.Add(this.labelHotItemStyle);
            this.Controls.Add(this.comboBoxView);
            this.Controls.Add(this.checkBoxGroups);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OLVListSettingDialog";
            this.ShowIcon = false;
            this.Text = "##List Settings";
            ((System.ComponentModel.ISupportInitialize)(this.rowHeightUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelEditable;
        private System.Windows.Forms.Label labelViewType;
        private System.Windows.Forms.ComboBox comboBoxHotItemStyle;
        private System.Windows.Forms.ComboBox comboBoxEditable;
        private System.Windows.Forms.Label labelHotItemStyle;
        private System.Windows.Forms.ComboBox comboBoxView;
        private System.Windows.Forms.CheckBox checkBoxGroups;
        private System.Windows.Forms.NumericUpDown rowHeightUpDown;
        private System.Windows.Forms.Label labelRowHeight;
        private System.Windows.Forms.CheckBox autoCalcHeightCheckBox;
    }
}