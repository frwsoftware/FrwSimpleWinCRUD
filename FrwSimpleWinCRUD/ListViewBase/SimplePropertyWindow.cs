﻿/**********************************************************************************
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
using Flobbster.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace FrwSoftware
{
    public partial class SimplePropertyWindow : BasePropertyWindow
    {
        static protected string ITEM_ATTRIBUTE_PREFIX = FrwCRUDRes.SimplePropertyWindow_Attribute;
        private PropertyBag bag1 = null;

        public SimplePropertyWindow()
        {
            InitializeComponent();
            Text = FrwCRUDRes.SimplePropertyWindow_Title;
            propertyGrid1.PropertySort = PropertySort.NoSort;
        }
        override public ViewMode ViewMode
        {
            get
            {
                return viewMode;
            }
            set
            {
                viewMode = value;
                if (viewMode == ViewMode.View || viewMode == ViewMode.ViewContent)
                {
                    saveButton.Visible = false;
                    revertButton.Visible = false;

                }
                else
                {
                    saveButton.Visible = true;
                    revertButton.Visible = true;
                }
            }
        }

        override public void CreateView()
        {
        }
        override public void ProcessView()
        {
            if (SourceObjectType != null)
            {
                string descr = ModelHelper.GetEntityJDescriptionOrName(SourceObjectType);
                SetNewCaption(descr + " " + FrwCRUDRes.SimplePropertyWindow_Props);
            }
            if (tempSourceObject != null)
            {
                if (tempSourceObject.GetType().Equals(SourceObjectType) == false) throw new ArgumentException("SourceObject not of SourceObjectType");

                bag1 = new PropertyBag();
                bag1.GetValue += new PropertySpecEventHandler(this.bag1_GetValue);
                bag1.SetValue += new PropertySpecEventHandler(this.bag1_SetValue);
                bag1.ValueModified += Bag1_ValueModified;
                bag1.SourceObject = tempSourceObject;
                bag1.SourceObjectType = SourceObjectType;

                PropertyInfo[] propsList = AttrHelper.GetBasePropertiesFirst(tempSourceObject.GetType());
                PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(tempSourceObject.GetType());
                if (pName != null)
                {
                    propsList = propsList.OrderBy(x => x != pName).ToArray();
                }

                PropertySpec props = null;
                foreach (PropertyInfo p in propsList)
                {
                    if (AttrHelper.IsGenericListTypeOf(p.PropertyType, typeof(IField)))
                    {
                        IList itemDatas = (IList)p.GetValue(tempSourceObject);
                        if (itemDatas != null)
                        {
                            foreach (var it in itemDatas)
                            {
                                IField itemdata = (IField)it;
                                string group = null;

                                props = new PropertySpec(ITEM_ATTRIBUTE_PREFIX + itemdata.FieldSysname, typeof(string), group,
                                    itemdata.Name);
                                props.PropTag = itemdata;
                                if (viewMode == ViewMode.View || viewMode == ViewMode.ViewContent)
                                {
                                    props.Attributes = new Attribute[] { new ReadOnlyAttribute(true) };
                                }
                                bag1.Properties.Add(props);

                            }

                        }
                    }
                    else
                    {
                        JReadOnly readOnlyAttr = AttrHelper.GetAttribute<JReadOnly>(SourceObjectType, p.Name);
                        JIgnore ignoreAttr = AttrHelper.GetAttribute<JIgnore>(SourceObjectType, p.Name);

                        if (ignoreAttr != null)
                            continue;

                        string desc = ModelHelper.GetPropertyJDescriptionOrName(p);
                        bool isCustomEdit = false;
                        if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, p.Name))
                        {
                            isCustomEdit = true;
                        }
                        Type pType = null;
                        if (isCustomEdit) pType = typeof(string);//disabled comboboxes for list type fields 
                        else pType = p.PropertyType;
                        props = new PropertySpec(desc, pType, null, desc);
                        props.PropTag = p.Name;
                        if (readOnlyAttr != null || viewMode == ViewMode.View || viewMode == ViewMode.ViewContent)
                        {
                            props.Attributes = new Attribute[] { new ReadOnlyAttribute(true) };
                        }
                        if (isCustomEdit)
                        {
                            //not only for edit
                            props.EditorTypeName = typeof(CustomPropertyEditor).ToString();
                        }

                        bag1.Properties.Add(props);
                    }

                    this.propertyGrid1.SelectedObjects = new object[] { bag1 };
                }
            }
            else
            {
                this.propertyGrid1.SelectedObjects = null;
            }
        }

        private void Bag1_ValueModified(object sender, PropertySpecEventArgs e)
        {
            SetModified(true);
        }

        private void bag1_GetValue(object sender, PropertySpecEventArgs e)
        {
            try
            {
                if (tempSourceObject != null)
                {
                    if (e.Property.PropTag != null && e.Property.PropTag is IField)
                    {
                        e.Value = (e.Property.PropTag as IField).Value;
                    }
                    else
                    {
                        string aspectName = e.Property.PropTag as string;//
                        Type pType = AttrHelper.GetPropertyType(SourceObjectType, aspectName);
                        e.Value = Dm.Instance.GetCustomPropertyValue(tempSourceObject, aspectName, true, AppManager.Instance.PropertyWindowTruncatedMaxItemCount, AppManager.Instance.PropertyWindowTruncatedMaxLength);
                    }
                }
                else
                    e.Value = null;
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        override protected void SetModified(bool modified)
        {
            base.SetModified(modified);
            saveButton.Enabled = modified;
            revertButton.Enabled = modified;
        }

        private void bag1_SetValue(object sender, PropertySpecEventArgs e)
        {
            try
            {
                SetModified(true);
                if (tempSourceObject != null)
                {
                    if (e.Property.PropTag != null && e.Property.PropTag is IField)
                    {
                        (e.Property.PropTag as IField).Value = (string)e.Value;
                    }
                    else
                    {
                        string name = e.Property.PropTag as string;
                        if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, name))
                        {
                        }
                        else
                        {
                            object oldValue = null;
                            oldValue = AttrHelper.GetPropertyValue(tempSourceObject, name);
                            AttrHelper.SetPropertyValue(tempSourceObject, name, e.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        #region saveconfig
        public override IDictionary<string, object> GetKeyParams()
        {
            IDictionary<string, object> pars = base.GetKeyParams();
            if (SourceObjectType != null) pars.Add("SourceObjectType", SourceObjectType);
            if (RelPaneUID != null) pars.Add(FrwBaseViewControl.PersistStringRelPaneUIDParameter, RelPaneUID);
            return pars;
        }
        public override void SetKeyParams(IDictionary<string, object> pars)
        {
            base.SetKeyParams(pars);

            if (pars == null) return;

            object t = DictHelper.Get(pars, "SourceObjectType");
            if (t != null && t is Type) SourceObjectType = t as Type;
            else if (t != null && t is string)
            {
                string typeName = t as string;
                SourceObjectType = TypeHelper.FindType(typeName);

            }
            else if (t != null) throw new ArgumentException();
        }
        public override bool CompareKeyParams(IDictionary<string, object> pars)
        {
            if (!compareObjectKey(DictHelper.Get(pars, "SourceObjectType"), SourceObjectType)) return false;
            return true;
        }
        #endregion


        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            try
            {
                RevertChanges();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }
    }
}
