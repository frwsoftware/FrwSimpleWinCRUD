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
    public partial class SimpleTextDialog : BaseDialogForm
    {
        public string TextToEdit
        {
            get
            {
                return this.textBox1.Text;
            }
            set
            {
                this.textBox1.Text = value;
            }
        }

        public SimpleTextDialog()
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleTextDialog_EnterText;

        }
    }
}
