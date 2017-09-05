using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrwSoftware.Model.Chinook;
using FrwSoftware.Model.Example;
using WeifenLuo.WinFormsUI.Docking;

namespace FrwSoftware
{
    public partial class FrwMainAppForm :  BaseMainAppForm
    {

      
        public FrwMainAppForm()
        {
            InitializeComponent();
        }

        private void FrwMainAppForm_Load(object sender, EventArgs e)
        {
            CreateMainMenuItems();
        }
     
        private void CreateMainMenuItems()
        {
            ToolStripMenuItem menuItem = null;
            ToolStripMenuItem groupItem = null;

            ////////////////
            CreateFileMenuItems(menuItemFile);

            menuItem = new ToolStripMenuItem("Run performance test 100 records");
            menuItem.Click += (s, em) =>
            {
                Cursor cursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    PerformanceTest.Test(100, false);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
                finally
                {
                    Cursor.Current = cursor;
                }
            };
            menuItemFile.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem("Run performance test 1000 records");
            menuItem.Click += (s, em) =>
            {
                Cursor cursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    PerformanceTest.Test(1000, false);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
                finally
                {
                    Cursor.Current = cursor;
                }
            };
            menuItemFile.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem("Run performance test 10000 records");
            menuItem.Click += (s, em) =>
            {
                Cursor cursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    PerformanceTest.Test(10000, false);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
                finally
                {
                    Cursor.Current = cursor;
                }
            };
            menuItemFile.DropDownItems.Add(menuItem);

            menuItem = new ToolStripMenuItem("Run performance test 100 records. Compare saving mode and SQLite");
            menuItem.Click += (s, em) =>
            {
                Cursor cursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    PerformanceTest.Test(100, true);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
                finally
                {
                    Cursor.Current = cursor;
                }
            };
            menuItemFile.DropDownItems.Add(menuItem);
            ///////////////////////////////////



            groupItem = new ToolStripMenuItem("Chinook database");
            menuItemDict.DropDownItems.Add(groupItem);

            CreateMainMenuItemForEntityType(groupItem, typeof(Album));
            CreateMainMenuItemForEntityType(groupItem, typeof(Artist));
            CreateMainMenuItemForEntityType(groupItem, typeof(Customer));
            CreateMainMenuItemForEntityType(groupItem, typeof(Employee));
            CreateMainMenuItemForEntityType(groupItem, typeof(Genre));
            CreateMainMenuItemForEntityType(groupItem, typeof(Invoice));
            CreateMainMenuItemForEntityType(groupItem, typeof(InvoiceLine));
            CreateMainMenuItemForEntityType(groupItem, typeof(MediaType));
            CreateMainMenuItemForEntityType(groupItem, typeof(Playlist));
            CreateMainMenuItemForEntityType(groupItem, typeof(Track));

            groupItem = new ToolStripMenuItem("Blank clone of Chinook database");
            menuItemDict.DropDownItems.Add(groupItem);

            CreateMainMenuItemForEntityType(groupItem, typeof(FrwSoftware.Model.TestChinook.Album));
            CreateMainMenuItemForEntityType(groupItem, typeof(FrwSoftware.Model.TestChinook.Artist));
        
            groupItem = new ToolStripMenuItem("Example database");
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(TeacherDto));
            CreateMainMenuItemForEntityType(groupItem, typeof(StudentDto));

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
