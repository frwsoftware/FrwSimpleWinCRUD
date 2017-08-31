using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace FrwSoftware
{
    public partial class FrwDocContent : DockContent
    {
        private IContent contentControl = null;
        public IContent ContentControl { get { return contentControl; } }
        protected override string GetPersistString()
        {
            IDictionary<string, object> pars = null;
            if (ContentControl != null)
            {
                pars = ContentControl.GetKeyParams();
            }
            else
            {
                pars = new Dictionary<string, object>();
                pars.Add(FrwBaseViewControl.PersistStringTypeParameter, GetType().ToString());
            }
            StringBuilder q = new StringBuilder();
            foreach (var k in pars.Keys)
            {
                if (q.Length > 0) q.Append(FrwBaseViewControl.PersistStringSeparator);
                var v = pars[k];
                q.Append(k);
                q.Append(FrwBaseViewControl.PersistStringSeparatorKeyValue);
                q.Append(v);
            }
            return q.ToString();
        }
        public FrwDocContent(IContent c)
        {
            InitializeComponent();
           
            this.contentControl = c;
            ((Control)c).Dock = DockStyle.Fill;
            this.HideOnClose = c.HideOnClose;
            this.Text = ((Control)c).Text;
            this.Controls.Add(((Control)c));
        }

        private void DocContent_FormClosed(object sender, FormClosedEventArgs e)
        {
            //!! this event do not occurs for HideOnClose windows
            Console.WriteLine("Savig config in FrwDocContent = " + contentControl);
            ContentControl.SaveConfig();
        }
    }
}
