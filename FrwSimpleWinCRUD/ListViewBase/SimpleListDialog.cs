using System;
using System.Collections;
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
    public partial class SimpleListDialog : BaseDialogForm
    {

        private IListProcessor listWindow = null;
        public IListProcessor ListWindow { get { return listWindow; } set { listWindow = value; } }

        List<object> selectedObjects = new List<object>();
        private void fillSelectedObjects()
        {
            selectedObjects.Clear();
            if (listWindow.SelectedObjects != null)
            {
                foreach (var o in listWindow.SelectedObjects)
                {
                    selectedObjects.Add(o);
                }
            }
        }
        public IList SelectedObjects
        {
            get
            {
                return selectedObjects;
            }
        
        }
        public SimpleListDialog(IListProcessor listWindow)
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleListDialog_Title;

            this.SuspendLayout();
            this.listWindow = listWindow;
            if (listWindow is IParentView)
            {
                (listWindow as IParentView).OnObjectSelectEvent += ListWindow_OnObjectSelectEvent;
            }
            this.panel1.Controls.Add((Control)listWindow);
            ((Control)listWindow).Dock = DockStyle.Fill;
            this.ResumeLayout();
        }

        private void ListWindow_OnObjectSelectEvent(object sender, ObjectSelectEventArgs e)
        {
            fillSelectedObjects();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            fillSelectedObjects();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButtion_Click(object sender, EventArgs e)
        {

        }
    }
}
