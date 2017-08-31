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
    public partial class SimpleChooseDialog : BaseDialogForm
    {
        public SimpleChooseDialog()
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.Choise;
        }

        public void AddChoose(string text, object tag)
        {
            ListViewItem item = new AdvListViewItem();
            item.Text = text;
            item.Tag = tag;
            this.listBox.Items.Add(item);
        }
        public int ChooseCount
        {
            get
            {
                return listBox.Items.Count;
            }
        }
        public object SelectedTag
        {

            get
            {
                if (this.listBox.SelectedItem != null)
                    return ((ListViewItem)this.listBox.SelectedItem).Tag;
                else return null;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SimpleChooseDialog_Load(object sender, EventArgs e)
        {
            if (this.listBox.Items.Count > 0)
                this.listBox.SelectedIndex = 0;
        }
    }
    public class AdvListViewItem : ListViewItem
    {
        override public string ToString()
        {
            return this.Text;
        }
    }
}
