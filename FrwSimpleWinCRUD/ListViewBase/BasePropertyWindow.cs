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
        protected object sourceObject = null;

        public object SourceObject {
            get
            {
                return sourceObject;
            }
            set
            {
                if (modified)
                {
                    DialogResult res = MessageBox.Show("Save changes?", FrwConstants.WARNING, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes)
                    {
                        SaveChanges();
                    }
                    else if (res == DialogResult.No)
                    {
                        //do nothings
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                
                sourceObject = value;
                tempSourceObject = Dm.Instance.CloneObject(sourceObject, CloneObjectType.ForTemp);
                SetModified(false);
            }
        }
        public bool SaveChanges()
        {
            JValidationResult result = Dm.Instance.ValidateObject(tempSourceObject);
            if (!result.isError)
            {
                Dm.Instance.CopyObjectProperties(tempSourceObject, sourceObject, CopyRestrictLevel.AllPropertiesNewLists);
                if (AttrHelper.IsAttributeDefinedForType<JEntity>(SourceObjectType, true)) { 
                    Dm.Instance.SaveObject(sourceObject);
                }
                SetModified(false);
                ChildObjectUpdateEventArgs ev = new ChildObjectUpdateEventArgs();
                ev.UpdatedObject = sourceObject;
                OnPropertyObjectUpdate(ev);
                return true;
            }
            else {
                AppManager.ShowValidationErrorMessage(result);
                return false;
            }

        }
        public bool CancelChanges()
        {
            if (modified)
            {
                DialogResult res = MessageBox.Show("Cancel without save changes?", FrwConstants.WARNING, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    SetModified(false);
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }

        protected void RevertChanges()
        {
            tempSourceObject = Dm.Instance.CloneObject(sourceObject, CloneObjectType.ForTemp);
            SetModified(false);
            ProcessView();
        }
        protected object tempSourceObject = null;
    
        public Type SourceObjectType { get; set; }

        protected ViewMode viewMode = ViewMode.View;
        virtual public ViewMode ViewMode {
            get
            {
                return viewMode;
            }
            set
            {
                viewMode = value;
            }
        }

        public BasePropertyWindow()
        {
            InitializeComponent();
           
        }
        protected bool modified = false;
        virtual protected void SetModified(bool modified)
        {
            this.modified = modified;
        }
        public string RelPaneUID { get; set; }
        public event ChildObjectUpdateEventHandler ChildObjectUpdateEvent;
        protected virtual void OnPropertyObjectUpdate(ChildObjectUpdateEventArgs e)
        {
            e.ViewMode = ViewMode;
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
