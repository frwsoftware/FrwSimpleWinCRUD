using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrwSoftware;

namespace FrwSoftware
{
    public partial class AppSettingsWindow : FrwBaseViewControl
    {
        public AppSettingsWindow()
        {
            InitializeComponent();
           
            Text = FrwCRUDRes.AppSettingsWindow_Title;
        }
        override public void SaveConfig()
        {
            Log.ProcessDebug("@@@@@ Saved config for AppSettingsWindow  ");

        }

        public void setObjects(object o)
        {
            this.appSettingsPropertyGrid1.SelectedObjects = new object[] { o};
        }

        private void AppSettingsWindow_Load(object sender, EventArgs e)
        {
            this.appSettingsPropertyGrid1.SetAppSettings();
        }

    }
}
