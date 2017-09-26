/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
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

        public ViewMode ViewMode {
            get
            {
                return propertyWindow.ViewMode;
            }
            set
            {
                propertyWindow.ViewMode = value;
                if (propertyWindow.ViewMode == ViewMode.View || propertyWindow.ViewMode == ViewMode.ViewContent)
                {
                    okButton.Enabled = false;
                    
                }
                else
                {
                    okButton.Enabled = true;
                }
            }
        }


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

        private void okButton_Click(object sender, EventArgs e)
        {
            if (propertyWindow.SaveChanges())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButtion_Click(object sender, EventArgs e)
        {
            if (propertyWindow.CancelChanges())
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
