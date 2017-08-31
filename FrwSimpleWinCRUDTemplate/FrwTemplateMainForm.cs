using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrwSoftware.Model.Example;

namespace FrwSoftware
{
    public partial class FrwTemplateMainForm :  BaseMainAppForm
    {
        public FrwTemplateMainForm()
        {
            InitializeComponent();
            /* 
            for (int i = 0; i < 20; i++)
            {
                JExampleDto1 dto1 = Dm.Instance.EmptyObject<JExampleDto1>(null);
                dto1.Name = "Dto_1_Record_" + i;
                JExampleDto2 dto2 = Dm.Instance.EmptyObject<JExampleDto2>(null);
                dto2.Name = "Dto_2_Record_" + i;
                JExampleDto3 dto3 = Dm.Instance.EmptyObject<JExampleDto3>(null);
                dto3.Name = "Dto_3_Record_" + i;

                dto1.Dto2 = dto2;
                dto1.Dto3s.Add(dto3);

                Dm.Instance.InsertOrUpdateObject(dto3);
                Dm.Instance.InsertOrUpdateObject(dto2);
                Dm.Instance.InsertOrUpdateObject(dto1);
            }
            */
        }

        private void FrwTemplateMainForm_Load(object sender, EventArgs e)
        {
            CreateMainMenuItems();

        }
        private void CreateMainMenuItems()
        {
            ToolStripMenuItem menuItem = null;
            ToolStripMenuItem groupItem = null;

            ////////////////
            CreateFileMenuItems(menuItemFile);

            groupItem = new ToolStripMenuItem("Example database");
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JExampleDto1));
            CreateMainMenuItemForEntityType(groupItem, typeof(JExampleDto2));
            CreateMainMenuItemForEntityType(groupItem, typeof(JExampleDto3));

            /////////////////////////////////////

            CreateViewMenuItems(menuItemView, toolBar, statusBar);

            ////////////////////////////////////////
            CreateMainMenuItemForWindowType(menuItemTools, "Application settings", typeof(AppSettingsWindow));


            //////////////////////////////////
            menuItem = new ToolStripMenuItem("About");
            menuItem.Click += (s, em) =>
            {
                try
                {
                    AboutDialog aboutDialog = new AboutDialog();
                    aboutDialog.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemHelp.DropDownItems.Add(menuItem);

        }
        //example of savig and restoring user settings for main window
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

    }
}
