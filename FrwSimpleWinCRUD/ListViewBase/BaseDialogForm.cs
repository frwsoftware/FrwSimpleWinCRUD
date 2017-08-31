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
    public partial class BaseDialogForm : Form
    {
        public BaseDialogForm()
        {
            InitializeComponent();
        }

        private void BaseDialogForm_Load(object sender, EventArgs e)
        {
            try
            {
                WinFormsUtils.SetNewControlFont(this, FrwSimpleWinCRUDConfig.GetApplicationFont());//if this.AutoScaleMode = Font - Setting the font will change the size of the window 
            }
            catch (Exception)
            {
                //to can show dialog designer
            }
        }
    }
}
