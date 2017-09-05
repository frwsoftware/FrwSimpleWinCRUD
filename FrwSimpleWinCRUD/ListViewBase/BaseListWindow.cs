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
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using FrwSoftware;


namespace FrwSoftware
{
    //abstract
    //The class is not labeled abstract to ensure the work of the visual designer 
    public partial class BaseListWindow : FrwBaseViewControl, IListProcessor, IParentView
    {
        public Type SourceObjectType { get; set; }
        public object FilteredObject { get; set; }
        public bool SelectionListMode { get; set; }//Control is used in the dialog of the selection list
        public bool DialogView { get; set; }//Display the properties window as a dialog
        public event ObjectSelectEventHandler OnObjectSelectEvent;
        public bool IsBufferToCut { get; set; }//for copy/past
        protected SimplePropertyDialog propertyDialog = null;
        #region paren-child property view
        public string PaneUID { get; set; }
        protected List<IChildView> childViews = new List<IChildView>();
        public IEnumerable<IChildView> ChildViews { get { return childViews; } }
        public void AddChildView(IChildView view)
        {
            if (childViews.Contains(view) == false)
            {
                childViews.Add(view);
                view.ChildObjectUpdateEvent += View_OnObjectUpdateEvent;
                view.RegisterAsChildView(this);
            }
        }

        public void RemoveChildView(IChildView view)
        {
            view.ChildObjectUpdateEvent -= View_OnObjectUpdateEvent;
            view.UnRegisterAsChildView(this);
            childViews.Remove(view);
        }
        public bool ContainsChildView(IChildView view)
        {
            return childViews.Contains(view);
        }
        //todo
        virtual public IList SelectedObjects
        {
            get
            {
                return null;
            }
        }
        #endregion
        public BaseListWindow()
        {
            InitializeComponent();
        }
        // the main external method for creating a list
        // called only once
        virtual public void CreateView()
        {
            if (SourceObjectType != null)
            {
                string descr = ModelHelper.GetEntityJDescriptionOrName(SourceObjectType);
                SetNewCaption(descr + " - " + FrwCRUDRes.List);
            }

        }

        // it is called to fill and draw the list
        // can be called many times (for example, after changing parameters)
        virtual public void ProcessView()
        {
        }
        // updates the view of the object in the list (it is used from the edit form)
        virtual public void RefreshObject(object o)
        {
        }

        // updates the view of all objects in the list
        // unlike ReloadList does not access the repository
        virtual public void RefreshList()
        {

        }

        virtual protected void ReloadList()
        {
            // here the database is accessed
        }

        virtual protected bool DeleteObjectInStorage(object[] oarray)
        {
            bool deleted = false;
            foreach (var ob in oarray)
            {
                Dm.Instance.DeleteObject(ob);
                deleted = true;
            }
            return deleted;
        }
        virtual protected bool DeleteAllObjectsInStorage()
        {
            bool deleted = false;
            Dm.Instance.DeleteAllObjects(SourceObjectType);
            deleted = true;
            return deleted;
        }
        virtual protected object EmptyObjectInStorage(Type sourceObjectType, IDictionary<string, object> pars = null)
        {
            //create new object bat not added it to storage
            return Dm.Instance.EmptyObject(sourceObjectType, pars);

        }
        virtual protected void InsertOrUpdateObjectInStorage(object o, string updatedPropertyName)
        {
            // since we are editing the storage object directly
            // this call can be purely nominal (if not overridden in the vault)
            // and used to set the modification flag in the repository
            Dm.Instance.SaveObject(o, updatedPropertyName);
            //Dm.Instance.UpdateRelations(o);//todo move to InsertOrUpdateObjectOrProperty ? 
        }
        virtual protected void AddObjectInStorage(object o)
        {
            //add it to storage
            Dm.Instance.SaveObject(o);
        }
        virtual protected bool RemoveObjectFromInStorage(object[] o)
        {
            return false;
        }


        private void View_OnObjectUpdateEvent(object sender, ChildObjectUpdateEventArgs e)
        {
            InsertOrUpdateObjectInStorage(e.UpdatedObject, e.UpdatedPropertyName);
            this.RefreshObject(e.UpdatedObject);
        }


        #region saverestorestate
        protected string GetStateConfigFileName()
        {
            string filename = Path.Combine(FrwConfig.Instance.ProfileConfigDir, GetType().FullName + "_" + this.SourceObjectType.FullName + "_state.config");
            return filename;
        }

        virtual protected void LoadUserSettings(IDictionary<string, object> userSettings)
        {

        }
        virtual protected void SaveUserSettings(IDictionary<string, object> userSettings)
        {

        }
    

        #endregion

        virtual protected JRights GetRights(object model)
        {
            JRights rights = null;
            if (model == null)
            {
                rights = new JRights(false);
                rights.CanView = false;
            }
            else
                rights = new JRights(true);
            return rights;
        }

        virtual protected void OnSelectObject(object selectedObject, dynamic extraParams = null)
        {
            if (OnObjectSelectEvent != null)
                OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObject = selectedObject, EditMode = ViewMode.View, ExtraParams = extraParams });
        }


        protected void ViewObjectLocal(object selectedObject, dynamic extraParams = null)
        {
            if (selectedObject == null) return;
            Cursor cursor = Cursor.Current;
            try
            {
                JRights rights = GetRights(selectedObject);
                if (!rights.CanView) throw new InvalidOperationException(FrwConstants.NO_RIGHTS);
                Cursor.Current = Cursors.WaitCursor;
                if (selectedObject != null)
                {
                    if (DialogView)
                    {
                        IPropertyProcessor propertyControl = null;
                        if (propertyDialog == null || propertyDialog.PropertyWindow.SourceObjectType.Equals(selectedObject.GetType()) == false)
                        {
                            propertyControl = (IPropertyProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IPropertyProcessor), selectedObject.GetType(), null);
                            propertyDialog = new SimplePropertyDialog(propertyControl);
                        }
                        else propertyControl = propertyDialog.PropertyWindow;
                        propertyControl.ViewMode = ViewMode.View;
                        propertyControl.SourceObject = selectedObject;
                        propertyControl.ProcessView();
                        DialogResult res = propertyDialog.ShowDialog();
                    }
                    else
                    {
                        IPropertyProcessor propertyControl = AppManager.Instance.CreatePropertyContentForModelType(this.ContentContainer, this, selectedObject.GetType(), null);
                        propertyControl.ViewMode = ViewMode.View;
                        propertyControl.SourceObject = selectedObject;
                        propertyControl.ProcessView();
                    }

                    if (OnObjectSelectEvent != null)
                        OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObject = selectedObject, EditMode = ViewMode.View, ExtraParams = extraParams });
                }
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
        protected void ViewObjectContentLocal(object selectedObject, dynamic extraParams = null)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (OnObjectSelectEvent != null)
                    OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObject = selectedObject, EditMode = ViewMode.ViewContent, ExtraParams = extraParams, DbClick = true });
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
        protected void UpdateObject(object selectedObject, dynamic extraParams = null)
        {
            if (UpdateObjectLocal(selectedObject))
            {
                //refresh
                RefreshObject(selectedObject);
            }
        }

        protected bool UpdateObjectLocal(object selectedObject, dynamic extraParams = null)
        {
            if (selectedObject == null) return false;
            Cursor cursor = Cursor.Current;
            try
            {
                JRights rights = GetRights(selectedObject);
                if (!rights.CanUpdate) throw new InvalidOperationException(FrwConstants.NO_RIGHTS);

                Cursor.Current = Cursors.WaitCursor;
                if (selectedObject != null)
                {
                    if (DialogView)
                    {
                        IPropertyProcessor propertyControl = null;
                        if (propertyDialog == null || propertyDialog.PropertyWindow.SourceObjectType.Equals(selectedObject.GetType()) == false)
                        {
                            propertyControl = (IPropertyProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IPropertyProcessor), selectedObject.GetType(), null);
                            propertyDialog = new SimplePropertyDialog(propertyControl);
                        }
                        else propertyControl = propertyDialog.PropertyWindow;
                        propertyControl.ViewMode = ViewMode.Edit;
                        propertyControl.SourceObject = selectedObject;
                        propertyControl.ProcessView();
                        DialogResult res = propertyDialog.ShowDialog();
                        if (res == DialogResult.OK)
                        {
                            InsertOrUpdateObjectInStorage(propertyControl.SourceObject, null);
                        }
                    }
                    else
                    {
                        IPropertyProcessor propertyControl = AppManager.Instance.CreatePropertyContentForModelType(this.ContentContainer, this, selectedObject.GetType(), null);
                        propertyControl.ViewMode = ViewMode.Edit;
                        propertyControl.SourceObject = selectedObject;
                        propertyControl.ProcessView();
                        // in this case the update in the repository asynchronously initiates the propertyControl
                    }
                    if (OnObjectSelectEvent != null)
                        OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObject = selectedObject, EditMode = ViewMode.Edit, ExtraParams = extraParams });
                }
                return true;
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
        virtual protected bool RemoveObjectFromLocal(object[] selectedObjects, dynamic extraParams = null)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (var selectedObject in selectedObjects)
                {
                    JRights rights = GetRights(selectedObject);
                    if (!rights.CanDelete) throw new InvalidOperationException(FrwConstants.NO_RIGHTS);
                }
                //no window 
                bool res = RemoveObjectFromInStorage(selectedObjects);
                if (OnObjectSelectEvent != null)
                    OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObjects = selectedObjects, EditMode = ViewMode.RemoveFrom, ExtraParams = extraParams });
                return res;
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
        virtual protected void AddObject(object selectedListItem, object selectedObject, Type sourceObjectType, IDictionary<string, object> extraParams = null)
        {

        }
        virtual protected object AddObjectLocal(object selectedObject, Type sourceObjectType, IDictionary<string, object> extraParams = null)
        {
            object o = null;
            try
            {
                JRights rights = GetRights(selectedObject);
                if (selectedObject != null)
                {
                    if (!rights.CanAddChild) throw new InvalidOperationException(FrwConstants.NO_RIGHTS);
                }
                else
                {
                    if (!rights.CanAdd) throw new InvalidOperationException(FrwConstants.NO_RIGHTS);
                }


                if (DialogView)
                {
                    IPropertyProcessor propertyControl = null;
                    if (propertyDialog == null || propertyDialog.PropertyWindow.SourceObjectType.Equals(sourceObjectType) == false)
                    {
                        propertyControl = (IPropertyProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IPropertyProcessor), sourceObjectType, null);
                        propertyDialog = new SimplePropertyDialog(propertyControl);
                    }
                    else propertyControl = propertyDialog.PropertyWindow;
                    propertyControl.ViewMode = ViewMode.New;
                    o = EmptyObjectInStorage(sourceObjectType, extraParams);
                    propertyControl.SourceObject = o;
                    propertyControl.ProcessView();
                    DialogResult res = propertyDialog.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        InsertOrUpdateObjectInStorage(propertyControl.SourceObject, null);
                    }
                }
                else
                {
                    IPropertyProcessor propertyControl = AppManager.Instance.CreatePropertyContentForModelType(this.ContentContainer, this, sourceObjectType, null);
                    propertyControl.ViewMode = ViewMode.New;
                    o = EmptyObjectInStorage(sourceObjectType, extraParams);
                    InsertOrUpdateObjectInStorage(o, null);
                    propertyControl.SourceObject = o;
                    propertyControl.ProcessView();
                    selectedObject = o;
                    // in this case the update in the repository asynchronously initiates the propertyControl
                }
                /*
                ObjectSelectEventArgs ev = new ObjectSelectEventArgs() { SelectedObject = selectedObject, EditMode = ViewMode.New, ExtraParams = extraParams };
                if (OnObjectSelectEvent != null)
                {
                    OnObjectSelectEvent(this, ev);
                    object o1 = ev.SelectedObject;//todo
                }
                */
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            return o;
        }

        virtual protected void  DeleteObject(object selectedListItem, object[] selectedObjects)
        {
        }

        protected bool DeleteObjectLocal(object[] selectedObjects)
        {
            // delete occurs without form and dialog
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                bool deleted = DeleteObjectInStorage(selectedObjects);
                if (OnObjectSelectEvent != null)
                    OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObjects = selectedObjects, EditMode = ViewMode.Delete });
                return deleted;
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
                return false;
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
        virtual protected void DeleteAllObjects()
        {
        }
        protected bool DeleteAllObjectsLocal()
        {
            // delete occurs without form and dialog
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                bool deleted = DeleteAllObjectsInStorage();

                if (OnObjectSelectEvent != null)
                    OnObjectSelectEvent(this, new ObjectSelectEventArgs() { SelectedObjects = null, EditMode = ViewMode.Delete });
                return deleted;
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
                return false;
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
    
        virtual protected void MakeContextMenuAddBlock(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, JRights rights)
        {
            ToolStripMenuItem menuItem = null;
            menuItem = new ToolStripMenuItem();
            menuItem.Text = FrwCRUDRes.List_Create_New_Record;
            menuItem.Image = Properties.Resources.AllPics_05;
            menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            menuItem.ImageScaling = ToolStripItemImageScaling.None;
            menuItem.Enabled = rights.CanAdd;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    AddObject(selectedListItem, selectedObject, SourceObjectType);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemList.Add(menuItem);
        }

 
        virtual protected bool CanCopyOrCutSelectedObjects(object sourceObjects, bool cut)
        {
            if (cut) return false;// can not be cut in regular lists 
            else return true;
        }
        virtual protected void CopySelectedObjectToClipBoard(IList sourceObjects, bool cut)
        {
            throw new InvalidOperationException();
        }
        virtual protected void CopyAdditionalObjectsToAppClipboard(object sourceObjects, bool cut, IDataObject dataObject)
        {

        }

        private bool CanPasteObjectFromWindowsClipboard(object destObject)
        {
            if (Clipboard.GetDataObject() == null) return false;
            else return CanPasteObjectsFromIDataObject(destObject, Clipboard.GetDataObject());
        }
        private bool CanPasteObjectFromAppClipboard(object destListItem)
        {
            if (AppManager.Instance.Clipboard.DataObject != null)
                return CanPasteObjectsFromIDataObject(destListItem, AppManager.Instance.Clipboard.DataObject);
            else return false;
        }

        // in the case of copy-paste it works with our buffer
        // in the case of a drop, it works with BrightIdeasSoftware.OLVDataObject or TreeNode
        // in the case of drops out of the box, it works with either a string or text
        protected bool CanPasteObjectsFromIDataObject(object destListItem, IDataObject sourceDataObject)
        {
            /*
            foreach (var f in sourceDataObject.GetFormats())
            {
                Console.WriteLine("Data format in clipboard: " + f);
            }
            */
            if (sourceDataObject is BrightIdeasSoftware.OLVDataObject)
            {
                BrightIdeasSoftware.OLVDataObject olvData = sourceDataObject as BrightIdeasSoftware.OLVDataObject;
                IList models = olvData.ModelObjects;
                return CanPasteSelectedObjects(destListItem, models, sourceDataObject);
            }
            else if (sourceDataObject is FrwOlvDataObject)
            {
                FrwOlvDataObject olvData = sourceDataObject as FrwOlvDataObject;
                IList models = olvData.SelectedObjects;
                return CanPasteSelectedObjects(destListItem, models, sourceDataObject);
            }
            else if (sourceDataObject is FrwTreeDataObject)
            {
                FrwTreeDataObject olvData = sourceDataObject as FrwTreeDataObject;
                return CanPasteSelectedObjects(destListItem, new List<TreeNode>() { olvData.SelectedTreeNode }, sourceDataObject);
            }
            else if (sourceDataObject != null && sourceDataObject.GetDataPresent(typeof(TreeNode)))
            {
                return CanPasteSelectedObjects(destListItem, new List<object>() { sourceDataObject.GetData(typeof(TreeNode)) }, sourceDataObject);
            }
            else if (sourceDataObject != null && (sourceDataObject.GetDataPresent(DataFormats.FileDrop)
                || sourceDataObject.GetDataPresent(DataFormats.Html) || sourceDataObject.GetDataPresent(DataFormats.Text)))
            {
                return CanPasteSelectedObjects(destListItem, null, sourceDataObject);
            }
            else
            {
                return false;
            }
        }

        virtual protected bool CanPasteSelectedObjects( object destListItem, object sourceObjects, IDataObject sourceDataObject)
        {
            if (destListItem == null)
            {
                return false; 
            }
            if (sourceObjects == null) return false;
            Type pasteType = null;
            if (sourceObjects is IList)
            {
                if ((sourceObjects as IList).Count == 0) return false;
                else pasteType = (sourceObjects as IList)[0].GetType();
            }
            else pasteType = sourceObjects.GetType();

            Type t = destListItem.GetType();
            foreach (var p in t.GetProperties())
            {
                Type pt = p.PropertyType;
              
                if (pt.Equals(pasteType))
                {
                    return true;
                }
                else if (AttrHelper.IsGenericListTypeOf(pt, pasteType))
                {
                    return true;
                }
                
            }
            return false;
        }
        private bool PasteSelectedObjectsFromWindowsClipBoard(object destListItem)
        {
            if (Clipboard.GetDataObject() == null) return false;
            else
            {
                bool res = PasteSelectedObjectsFromIDataObject(destListItem, Clipboard.GetDataObject(), false);
                Clipboard.Clear();
                return res;
            }

        } 
        private bool PasteSelectedObjectsFromAppClipBoard(object destListItem)
        {
            if (AppManager.Instance.Clipboard.DataObject != null)
            {
                bool res = PasteSelectedObjectsFromIDataObject(destListItem, AppManager.Instance.Clipboard.DataObject, AppManager.Instance.Clipboard.IsCutOperation);
                AppManager.Instance.Clipboard.Clear();
                return res;
            }
            else return false;
        }
        protected bool PasteSelectedObjectsFromIDataObject(object destListItem, IDataObject sourceDataObject, bool cut)
        {

            if (sourceDataObject is BrightIdeasSoftware.OLVDataObject)
            {
                BrightIdeasSoftware.OLVDataObject olvData = sourceDataObject as BrightIdeasSoftware.OLVDataObject;
                IList models = olvData.ModelObjects;
                return PasteSelectedObjects(destListItem, models, sourceDataObject, cut);
            }
            else if (sourceDataObject is FrwOlvDataObject)
            {
                FrwOlvDataObject olvData = sourceDataObject as FrwOlvDataObject;
                IList models = olvData.SelectedObjects;
                return PasteSelectedObjects(destListItem, models, sourceDataObject, cut);
            }
            else if (sourceDataObject is FrwTreeDataObject)
            {
                FrwTreeDataObject olvData = sourceDataObject as FrwTreeDataObject;
                return PasteSelectedObjects(destListItem, new List<TreeNode>() { olvData.SelectedTreeNode }, sourceDataObject, cut);
            }
            else if (sourceDataObject != null && sourceDataObject.GetDataPresent(typeof(TreeNode)))
            {
                return PasteSelectedObjects(destListItem, new List<object>() { sourceDataObject.GetData(typeof(TreeNode)) }, sourceDataObject, cut);
            }
            else if (sourceDataObject != null && (sourceDataObject.GetDataPresent(DataFormats.FileDrop)
                || sourceDataObject.GetDataPresent(DataFormats.Html) || sourceDataObject.GetDataPresent(DataFormats.Text)))
            {
                return PasteSelectedObjects(destListItem, null, sourceDataObject, cut);
            }
            else return false;
        }
        virtual protected bool PasteSelectedObjects( object destListItem,  object sourceObjects, IDataObject sourceDataObject, bool cut)
        {
            if (destListItem == null)
            {
                MessageBox.Show(FrwConstants.CAN_NOT_DROP_NO_DESTINATION_ITEM, FrwConstants.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false; 
            }
            Type pasteType = null;
            if (sourceObjects is IList)
            {
                if ((sourceObjects as IList).Count == 0) return false;
                else pasteType = (sourceObjects as IList)[0].GetType();
            }
            else pasteType = sourceObjects.GetType();
            Type t = destListItem.GetType();
            SimpleChooseDialog dlg = new SimpleChooseDialog();
            dlg.Text = FrwCRUDRes.SelectFieldToPaste;
            foreach (var p in t.GetProperties())
            {
                Type pt = p.PropertyType;
                if (pt.Equals(pasteType))
                {
                    dlg.AddChoose(ModelHelper.GetPropertyJDescriptionOrName(p), p);
                }
                else if (AttrHelper.IsGenericListTypeOf(pt, pasteType))
                {
                    dlg.AddChoose(ModelHelper.GetPropertyJDescriptionOrName(p) + " (Список)", p);
                }
            }
            if (dlg.ChooseCount > 0)
            {
                DialogResult res = dlg.ShowDialog();
                if (res == DialogResult.OK && dlg.SelectedTag != null)
                {
                    foreach (var p in t.GetProperties())
                    {
                        if (p.Equals(dlg.SelectedTag))
                        {
                            Type pt = p.PropertyType;
                            if (pt.Equals(pasteType))
                            {
                                //todo test for single
                                p.SetValue(destListItem, (sourceObjects as IList)[0]);
                                Dm.Instance.SaveObject(destListItem);
                            }
                            else if (AttrHelper.IsGenericListTypeOf(pt, pasteType))
                            {
                                IList l = (IList)p.GetValue(destListItem);
                                foreach (var ll in (sourceObjects as IList)) { 
                                    l.Add(ll);//todo exists
                                }
                                Dm.Instance.SaveObject(destListItem);
                            }
                            RefreshObject(destListItem);
                        }
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show(FrwConstants.CAN_NOT_DROP, FrwConstants.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

   


        virtual protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            IList selectedForCopyPaste = GetSelectedForCopyPaste();
            JRights rights = GetRights(selectedObject);
            ToolStripMenuItem menuItem = null;
            if (aspectName != null)
            {
                if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, aspectName))
                {
                    object rowObject = selectedObject;

                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.List_Edit_Field;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            //todo button xak ?
                            bool cancel = false;
                            object newValue = AppManager.Instance.EditCustomPropertyValue(rowObject, aspectName, out cancel, this);
                            Dm.Instance.SaveObject(rowObject);
                            RefreshObject(rowObject);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);
                    menuItemList.Add(new ToolStripSeparator());

                }
            }

            menuItem = new ToolStripMenuItem();
            menuItem.Text = FrwCRUDRes.Lisе_Refresh_List;
            menuItem.Image = Properties.Resources.AllPics_01;
            menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            menuItem.ImageScaling = ToolStripItemImageScaling.None;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    Cursor cursor = Cursor.Current;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        ReloadList();
                    }
                    finally
                    {
                        Cursor.Current = cursor;
                    }
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemList.Add(menuItem);

            MakeContextMenuAddBlock(menuItemList, selectedListItem, selectedObject, rights);

            if (selectedObject != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_View_Of_Record;
                menuItem.Image = Properties.Resources.AllPics_08;
                menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Enabled = rights.CanView;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        ViewObjectLocal(selectedObject);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Edit_Record;
                menuItem.Image = Properties.Resources.AllPics_09;
                menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Enabled = rights.CanUpdate;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        UpdateObject(selectedObject);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Remove_Record;
                menuItem.Image = Properties.Resources.AllPics_06;
                menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Enabled = rights.CanDelete;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        DeleteObject(selectedListItem, new object[] { selectedObject });
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Remove_All_Records;
                menuItem.Image = Properties.Resources.AllPics_07;
                menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Enabled = rights.CanDeleteAll;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        DeleteAllObjects();
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItemList.Add(new ToolStripSeparator());

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Copy;// Record (s) to the clipboard 
                menuItem.Image = Properties.Resources.AllPics_11;
                menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Enabled = CanCopyOrCutSelectedObjects(selectedForCopyPaste, false);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        CopySelectedObjectToClipBoard( selectedForCopyPaste, false);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Cut;// запись(и) в буфер обмена";
                menuItem.Enabled = CanCopyOrCutSelectedObjects( selectedForCopyPaste, true);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        
                        CopySelectedObjectToClipBoard( selectedForCopyPaste, true);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);
                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Paste_From_Buffer;// Windows";
                menuItem.Enabled = CanPasteObjectFromAppClipboard(GetSelectedListItem());
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        PasteSelectedObjectsFromAppClipBoard(GetSelectedListItem());
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Paste_From_Windows_Buffer;
                menuItem.Enabled = CanPasteObjectFromWindowsClipboard(GetSelectedListItem());
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        PasteSelectedObjectsFromWindowsClipBoard(GetSelectedListItem());
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);


            }
        }




        private void refreshButton_Click(object sender, EventArgs e)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ReloadList();
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }



        // returns the first (or only) selected object
        // used in toolbar buttons for editing, browsing, new
        protected object GetSelectedObject()
        {
            IList o = GetSelectedObjects();
            return (o != null && o.Count > 0) ? o[0] : null;
        }
        // returns the first (or only) allocated list node
        virtual protected object GetSelectedListItem()
        {
            return null;
        }

        // returns all selected objects
        // used for the delete button
        virtual protected IList GetSelectedObjects()
        {
            return null;
        }
        // returns all the selected entities for copy operations (this can be either objects or list nodes)
        virtual protected IList GetSelectedForCopyPaste()
        {
            return null;
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetSelectedObjects() != null && GetSelectedObjects().Count > 1) MessageBox.Show(FrwCRUDRes.List_Selected_More_Than_On_record);
                else if (GetSelectedObject() == null) MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                else
                {
                     ViewObjectLocal(GetSelectedObject());
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private object[] ToArray(IList list)
        {
            object[] array = new object[list.Count];
            list.CopyTo(array, 0);
            return array;

        }

        private void newButton_Click(object sender, EventArgs e)
        {
            try
            {
                 object selectedObject = GetSelectedObject();
                 AddObject(GetSelectedListItem(), selectedObject, SourceObjectType);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetSelectedObjects() != null && GetSelectedObjects().Count > 1) MessageBox.Show(FrwCRUDRes.List_Selected_More_Than_On_record);
                else if (GetSelectedObject() == null) MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                else { 
                    UpdateObject(GetSelectedObject());
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetSelectedObjects() == null || GetSelectedObjects().Count == 0) MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                else
                {
                    IList selectedObjects = GetSelectedObjects();
                    object[] d = selectedObjects.Cast<object>().ToArray();
                    DeleteObject(GetSelectedListItem(), d);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private void copyToClipboardButton_Click(object sender, EventArgs e)
        {
            try
            {
                IList selectedObjects = GetSelectedForCopyPaste();
                if (selectedObjects == null || selectedObjects.Count == 0) MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                else
                {
                    CopySelectedObjectToClipBoard(selectedObjects, false);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        virtual protected void ShowSettingsDialog()
        {

        }

        private void listSettingsButton_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (SourceObjectType != null) Dm.Instance.SaveEntityData(SourceObjectType);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }

        protected void AddToolStripItem(ToolStripItem item)
        {
            this.toolStrip.Items.Add(item);
        }


        private void dialogViewButton_Click(object sender, EventArgs e)
        {
            this.DialogView = dialogViewButton.Checked;
        }
        #region saveconfig

        public override IDictionary<string, object> GetKeyParams()
        {
            IDictionary<string, object> pars = base.GetKeyParams();
            if (SourceObjectType != null) pars.Add("SourceObjectType", SourceObjectType);
            if (PaneUID != null) pars.Add(FrwBaseViewControl.PersistStringPaneUIDParameter, PaneUID);
            return pars;
        }
        public override void SetKeyParams(IDictionary<string, object> pars)
        {
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
            //Keys in this method are objects only, but not with strings 
            if (!compareObjectKey(DictHelper.Get(pars, "SourceObjectType"), SourceObjectType)) return false;
            return true;
        }
        #endregion
    }

    public class DragDropEffectsWrapper
    {
        public DragDropEffectsWrapper(DragDropEffects dragDropEffects)
        {
            this.dragDropEffects = dragDropEffects;
        }
        private DragDropEffects dragDropEffects = DragDropEffects.None;
        public DragDropEffects DragDropEffects { get { return dragDropEffects; } }
    }
 
}
