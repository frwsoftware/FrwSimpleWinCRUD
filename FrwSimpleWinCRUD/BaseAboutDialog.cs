using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;


namespace FrwSoftware
{
    public partial class BaseAboutDialog : Form
    {
        private Form mainForm = null;

        public BaseAboutDialog(Form mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void AboutDialog_Load(object sender, EventArgs e)
        {
            labelAppVersion.Text = mainForm.GetType().Assembly.GetName().Version.ToString();
            labelLibVersion.Text = typeof(DockPanel).Assembly.GetName().Version.ToString();
        }

     
    }
}