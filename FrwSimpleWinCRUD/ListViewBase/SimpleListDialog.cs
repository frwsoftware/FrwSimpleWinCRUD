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

        public bool EnableSetNull
        {
            set
            {
                setNullButton.Visible = value;
            }
        }

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
        public SimpleListDialog(Type type)
        {
            InitializeComponent();
            this.SuspendLayout();

            listWindow = (IListProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IListProcessor), type,
                new Dictionary<string, object>() { { FrwBaseViewControl.PersistStringDlgModeParameter, true} });

            this.Text = FrwCRUDRes.SimpleListDialog_Title;

            if (listWindow is IParentView)
            {
                (listWindow as IParentView).OnObjectSelectEvent += ListWindow_OnObjectSelectEvent;
            }
            this.panel1.Controls.Add((Control)listWindow);
            ((Control)listWindow).Dock = DockStyle.Fill;
            listWindow.ProcessView();
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

        private void SimpleListDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            ListWindow.ClosingContent();
        }

        private void SimpleListDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            ListWindow.SaveConfig();
        }

        private void setNullButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
