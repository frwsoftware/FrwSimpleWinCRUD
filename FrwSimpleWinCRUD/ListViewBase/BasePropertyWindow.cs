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
