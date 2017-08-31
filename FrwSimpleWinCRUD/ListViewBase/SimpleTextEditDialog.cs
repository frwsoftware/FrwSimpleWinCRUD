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
    public partial class SimpleTextEditDialog : BaseDialogForm
    {
        public string EditedText
        {
            get {
                return this.textEditorControl.Text;
            }
            set
            {
                this.textEditorControl.Text = value;
            }
        }

        public SimpleTextEditDialog()
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleTextEditDialog_Title;

        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }
    }
}
