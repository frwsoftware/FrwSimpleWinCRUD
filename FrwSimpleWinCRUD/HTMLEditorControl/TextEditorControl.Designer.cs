namespace FrwSoftware
{
    partial class TextEditorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorControl));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.htmlEditorControl = new MSDN.Html.Editor.HtmlEditorControl();
            this.simpleTextBox = new System.Windows.Forms.TextBox();
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.buttonHTML = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStripTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.htmlEditorControl);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.simpleTextBox);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripTop);
            // 
            // htmlEditorControl
            // 
            resources.ApplyResources(this.htmlEditorControl, "htmlEditorControl");
            this.htmlEditorControl.InnerText = null;
            this.htmlEditorControl.Name = "htmlEditorControl";
            // 
            // simpleTextBox
            // 
            resources.ApplyResources(this.simpleTextBox, "simpleTextBox");
            this.simpleTextBox.Name = "simpleTextBox";
            // 
            // toolStripTop
            // 
            resources.ApplyResources(this.toolStripTop, "toolStripTop");
            this.toolStripTop.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonHTML});
            this.toolStripTop.Name = "toolStripTop";
            // 
            // buttonHTML
            // 
            this.buttonHTML.CheckOnClick = true;
            this.buttonHTML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonHTML.Image = global::FrwSoftware.Properties.Resources.if_html_5415;
            resources.ApplyResources(this.buttonHTML, "buttonHTML");
            this.buttonHTML.Name = "buttonHTML";
            this.buttonHTML.Click += new System.EventHandler(this.buttonHTML_Click);
            // 
            // TextEditorControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "TextEditorControl";
            this.Load += new System.EventHandler(this.TextEditorControl_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MSDN.Html.Editor.HtmlEditorControl htmlEditorControl;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.TextBox simpleTextBox;
        private System.Windows.Forms.ToolStripButton buttonHTML;
    }
}
