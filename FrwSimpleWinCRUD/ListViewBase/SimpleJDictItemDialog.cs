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
    public partial class SimpleJDictItemDialog : BaseDialogForm
    {
        public JDictItem DictItem { get;  }
        public SimpleJDictItemDialog(JDictItem dictItem)
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleJDictItemDialog_Title;
            this.label3.Text = FrwCRUDRes.SimpleJDictItemDialog_Image;
            this.label2.Text = FrwCRUDRes.SimpleJDictItemDialog_Text;
            this.label1.Text = FrwCRUDRes.SimpleJDictItemDialog_Key;

            DictItem = DictItem;
            textTextBox.Text = DictItem.Text;
            keyTextBox.Text = DictItem.Key;
            //imageTextBox.Text = DictItem.Image;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DictItem.Text = textTextBox.Text;
            DictItem.Key = keyTextBox.Text;
            //DictItem.Image = imageTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
