using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware;

namespace FrwSoftware
{
    // event when selecting a list row, also for editing, deleting, or adding
    public delegate void ObjectSelectEventHandler(object sender, ObjectSelectEventArgs e);
    public class ObjectSelectEventArgs : EventArgs
    {
        private object[] selectedObjects = null;
        public object SelectedObject {
            get
            {
                return selectedObjects != null && selectedObjects.Length > 0 ? selectedObjects[0] : null;
            }
            set
            {
                selectedObjects = new object[]{value};
            }
        }
        public object[] SelectedObjects
        {
            get
            {
                return selectedObjects;
            }
            set
            {
                selectedObjects = value ;
            }
        }
        public ViewMode EditMode { get; set; }
        public dynamic ExtraParams { get; set; }
        public bool DbClick { get; set; }

    }
    public delegate void IViewProcessorCreateEventHandler(object sender, IViewProcessorCreateEventArgs e);
    public class IViewProcessorCreateEventArgs : EventArgs
    {
        public IObjectViewProcessor IViewProcessorControl { get; set; }
    }

    public class ChildObjectUpdateEventArgs : EventArgs
    {
        public object UpdatedObject { get; set; }
        public string UpdatedPropertyName { get; set; }//заполняется тогда, когда нужно обновить только конкретный атрибут
    }
    public delegate void ChildObjectUpdateEventHandler(object sender, ChildObjectUpdateEventArgs e);

    public class SelectionListSelectedEventArgs : EventArgs
    {
        private object[] selectedObjects = null;
        public object SelectedObject
        {
            get
            {
                return selectedObjects != null && selectedObjects.Length > 0 ? selectedObjects[0] : null;
            }
            set
            {
                selectedObjects = new object[] { value };
            }
        }
        public object[] SelectedObjects
        {
            get
            {
                return selectedObjects;
            }
            set
            {
                selectedObjects = value;
            }
        }
    }
    public delegate void SelectionListSelectedEventHandler(object sender, SelectionListSelectedEventArgs e);



}
