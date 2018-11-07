using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    public partial class AdvancedMainAppForm : BaseMainAppForm
    {
        protected ToolStrip toolBar;
        protected MenuStrip mainMenu;
        protected ToolStripMenuItem menuItemFile;
        protected ToolStripMenuItem menuItemDict;
        protected ToolStripMenuItem menuItemView;
        protected ToolStripMenuItem menuItemTools;
        protected ToolStripMenuItem menuItemWindow;
        protected ToolStripMenuItem menuItemHelp;
        protected ToolStripMenuItem menuItemLayouts;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton saveDatabaseButton;


        protected StatusStrip statusBar;


        public AdvancedMainAppForm()
        {
            InitializeComponent();
            CreateSimpleMainMenuAndToolAndStatusBar();
        }



        protected void CreateSimpleMainMenuAndToolAndStatusBar()
        {
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDict = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLayouts = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveDatabaseButton = new System.Windows.Forms.ToolStripButton();


            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notificationTimer
            // 
            this.notificationTimer.Enabled = true;
            // 
            // dockPanel
            // 
            this.dockPanel.Size = new System.Drawing.Size(771, 398);
            // 
            // toolBar
            // 
            this.toolBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolBar.Location = new System.Drawing.Point(0, 28);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(771, 25);
            this.toolBar.TabIndex = 21;
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveDatabaseButton});
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.menuItemLayouts,
            this.menuItemDict,
            this.menuItemView,
            this.menuItemTools,
            this.menuItemWindow,
            this.menuItemHelp});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.MdiWindowListItem = this.menuItemWindow;
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(771, 28);
            this.mainMenu.TabIndex = 20;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(63, 6);
            // 
            // menuItemFile
            // 
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(44, 24);
            this.menuItemFile.Text = FrwCRUDRes.File;
            // 
            // menuItemDict
            // 
            this.menuItemDict.Name = "menuItemDict";
            this.menuItemDict.Size = new System.Drawing.Size(100, 24);
            this.menuItemDict.Text = FrwCRUDRes.Dictionaries;
            // 
            // menuItemView
            // 
            this.menuItemView.MergeIndex = 1;
            this.menuItemView.Name = "menuItemView";
            this.menuItemView.Size = new System.Drawing.Size(53, 24);
            this.menuItemView.Text = FrwCRUDRes.Menu_View;
            // 
            // menuItemTools
            // 
            this.menuItemTools.MergeIndex = 2;
            this.menuItemTools.Name = "menuItemTools";
            this.menuItemTools.Size = new System.Drawing.Size(56, 24);
            this.menuItemTools.Text = FrwCRUDRes.Tools;
            // 
            // menuItemWindow
            // 
            this.menuItemWindow.MergeIndex = 2;
            this.menuItemWindow.Name = "menuItemWindow";
            this.menuItemWindow.Size = new System.Drawing.Size(76, 24);
            this.menuItemWindow.Text = FrwCRUDRes.Window;
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.MergeIndex = 3;
            this.menuItemHelp.Name = "menuItemHelp";
            this.menuItemHelp.Size = new System.Drawing.Size(53, 24);
            this.menuItemHelp.Text = FrwCRUDRes.Help;
            // 
            // menuItemLayouts
            // 
            this.menuItemLayouts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1});
            this.menuItemLayouts.Name = "menuItemLayouts";
            this.menuItemLayouts.Size = new System.Drawing.Size(124, 24);
            this.menuItemLayouts.Text = FrwCRUDRes.Layout;
            this.menuItemLayouts.Click += new System.EventHandler(this.menuItemLayouts_Click);
            // 
            // saveDatabaseButton
            // 
            this.saveDatabaseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveDatabaseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveDatabaseButton.Name = "saveDatabaseButton";
            this.saveDatabaseButton.Image = Properties.Resources.save;
            this.saveDatabaseButton.Size = new System.Drawing.Size(23, 24);
            this.saveDatabaseButton.Text = FrwCRUDRes.Save_all_entities_data;
            this.saveDatabaseButton.Click += new System.EventHandler(this.saveDatabaseButton_Click);
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Location = new System.Drawing.Point(0, 376);
            this.statusBar.Name = "statusBar";
            this.statusBar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusBar.ShowItemToolTips = true;
            this.statusBar.Size = new System.Drawing.Size(771, 22);
            this.statusBar.TabIndex = 22;
            // 
            // WebAccountMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 398);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.statusBar);
            this.DocPanelBounds = new System.Drawing.Rectangle(19, 19, 789, 445);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Controls.SetChildIndex(this.dockPanel, 0);
            this.Controls.SetChildIndex(this.statusBar, 0);
            this.Controls.SetChildIndex(this.mainMenu, 0);
            this.Controls.SetChildIndex(this.toolBar, 0);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        override protected void LoadUserSettings(IDictionary<string, object> userSettings)
        {
            base.LoadUserSettings(userSettings);
            toolBar.Visible = DictHelper.GetValueAsBool(userSettings, "toolBarVisible");
            statusBar.Visible = DictHelper.GetValueAsBool(userSettings, "statusBarVisible");
        }
        override protected void SaveUserSettings(IDictionary<string, object> userSettings)
        {
            base.SaveUserSettings(userSettings);
            userSettings.Add("toolBarVisible", toolBar.Visible);
            userSettings.Add("statusBarVisible", statusBar.Visible);
        }
        private void menuItemLayouts_Click(object sender, EventArgs e)
        {
            try
            {
                //menuCustomTask.DropDownItems.Clear();- очистка приводит к тому, что меню появляется по 0.0 координатам 
                ToolStripItem sep = new ToolStripSeparator();
                menuItemLayouts.DropDownItems.Add(sep);
                List<ToolStripItem> tmp = new List<ToolStripItem>();
                foreach (ToolStripItem c in menuItemLayouts.DropDownItems)
                {
                    if (c != sep) tmp.Add(c);
                }
                foreach (var c in tmp)
                {
                    menuItemLayouts.DropDownItems.Remove(c);
                }
                /////
                ToolStripItem menuItem = null;
                IList<JDocPanelLayout> list = Dm.Instance.FindAll<JDocPanelLayout>();
                foreach (var l in list)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = l.Name;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            AppManager.Instance.LoadLayout((JDocPanelLayout)l);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemLayouts.DropDownItems.Add(menuItem);
                }

            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private void saveDatabaseButton_Click(object sender, EventArgs e)
        {
            try
            {
                Dm.Instance.SaveAllEntitiesData(false);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


    }
}
