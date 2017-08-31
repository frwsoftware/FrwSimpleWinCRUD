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
    public partial class SimplePropertyDialog : BaseDialogForm
    {
        private IPropertyProcessor propertyWindow = null;
        public IPropertyProcessor PropertyWindow { get { return propertyWindow; } set { propertyWindow = value; } }

        public SimplePropertyDialog(IPropertyProcessor propertyWindow)
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimplePropertyDialog_Title;

            this.SuspendLayout();
            this.propertyWindow = propertyWindow;
            this.panel1.Controls.Add((Control)propertyWindow);
            ((Control)propertyWindow).Dock = DockStyle.Fill;
            this.ResumeLayout();
        }
    }
}
