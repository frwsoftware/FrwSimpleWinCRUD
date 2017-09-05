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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    public partial class BasePropertyWindow : FrwBaseViewControl, IPropertyProcessor, IChildView
    {
        public object SourceObject { get; set; }
        public Type SourceObjectType { get; set; }
        public ViewMode ViewMode { get; set; }

        public BasePropertyWindow()
        {
            InitializeComponent();
        }

        public string RelPaneUID { get; set; }
        public event ChildObjectUpdateEventHandler ChildObjectUpdateEvent;
        protected virtual void OnPropertyObjectUpdate(ChildObjectUpdateEventArgs e)
        {
            if (ChildObjectUpdateEvent != null)
                ChildObjectUpdateEvent(this, e);
        }

        virtual public void RegisterAsChildView(IParentView parent)
        {
            parent.OnObjectSelectEvent += Parent_OnObjectSelectEvent;
        }


        virtual public void UnRegisterAsChildView(IParentView parent)
        {
            parent.OnObjectSelectEvent -= Parent_OnObjectSelectEvent;
        }

        private void Parent_OnObjectSelectEvent(object sender, ObjectSelectEventArgs e)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                object selectedObject = e.SelectedObject;
                if (selectedObject != null)
                {
                    if (Visible)
                    {
                        if (SourceObjectType.Equals(selectedObject.GetType()))
                        {
                            SourceObject = selectedObject;
                            ProcessView();
                        }
                    }
                }
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }


        virtual public void CreateView()
        {
        }
        virtual public void ProcessView()
        {
        }
    }
}
