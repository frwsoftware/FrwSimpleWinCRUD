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
using BrightIdeasSoftware;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace FrwSoftware
{
    public partial class BaseOLVListWindow : BaseListWindow
    {
        public int Start { get { return start; } set { start = value; } }
        public int Limit { get { return limit; } set { limit = value; } }
        protected ObjectListView listView = null;
        private bool isTreeList = false;
        public OLVHotItemStyle HotItemStyle { get; set; }

        private ToolStripButton columnsButton;
        private ToolStripTextBox filterTextBox;

        //todo for pagination
        protected int start = 0;
        protected int limit = 20;

        public bool IsTreeList
        {
            get
            {
                return isTreeList;
            }
            set
            {
                if (this.listView != null) throw new InvalidOperationException();
                isTreeList = value;
            }
        }

        public BaseOLVListWindow()
        {
            InitializeComponent();

            HotItemStyle = OLVHotItemStyle.Translucent;
            // 
            // columnsButton
            // 
            this.columnsButton = new System.Windows.Forms.ToolStripButton();
            this.columnsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.columnsButton.Image = global::FrwSoftware.Properties.Resources.AllPics_03;
            this.columnsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            //this.columnsButton.ImageScaling = ToolStripItemImageScaling.None;
            this.columnsButton.Name = "columnsButton";
            this.columnsButton.Size = new System.Drawing.Size(24, 32);
            this.columnsButton.Text = FrwCRUDRes.List_Columns;
            this.columnsButton.Click += new System.EventHandler(this.columnsButton_Click);

            // 
            // filterTextBox
            // 
            this.filterTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.filterTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(100, 35);
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);

            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnsButton,
            this.filterTextBox});
        }

        // the main external method for creating a list
        // called only once
        override public void CreateView()
        {
            base.CreateView();

            if (listView != null) throw new InvalidOperationException();
            if (SourceObjectType == null) throw new InvalidOperationException();

            this.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(listView)).BeginInit();
            CreateObjectListView();
            MakeListColumns();
            LoadConfig();//Here columns can change the order, sorting, etc.
            //((System.ComponentModel.ISupportInitialize)(listView)).EndInit();
            this.ResumeLayout();
        }
        // create columns and set additional list properties
        virtual protected void MakeListColumns()
        {
        }
        // it is called to fill and draw the list
        // can be called many times (for example, after changing parameters)
        override public void ProcessView()
        {
            this.SuspendLayout();
            ReloadList();
            this.ResumeLayout();
        }
        override public void RefreshObject(object o)
        {
            listView.RefreshObject(o);
        }
        // updates the view of all objects in the list
        // unlike ReloadList does not access the repository
        override public void RefreshList()
        {
            listView.BuildList(true);
        }
        override protected void EnsureAddedObjectVisible(object newObject)
        {
            // the standard listView method, when the filter is enabled, adds a record to the sources and rebuilds the list
            // and at us record in the source is already added, therefore the duplication
            // If we are filtering the list, there is no way to efficiently
            // insert the objects, so just put them into our collection and rebuild.
            if (listView.IsFiltering)
            {
                //index = Math.Max(0, Math.Min(index, ourObjects.Count));
                //ourObjects.InsertRange(index, modelObjects);
                listView.BuildList(true);
            }
            else
            {
                this.listView.AddObject(newObject);
            }
            this.listView.EnsureModelVisible(newObject); //? 
        }
        override public IList SelectedObjects
        {
            get
            {
                return listView.SelectedObjects;
            }
        }
        // helper method creating a list and setting typical list parameters
        virtual protected void CreateObjectListView()
        {
            // FastDataListView FastListView - inherited from VirtualListView and intended to display lists with a large number of entries
            // their feature is that objects are drawn only for those lines that are in the zone of visibility
            // however, this also involves a drawback - the list constantly redraws its visible area
            // DataListView - is designed to work with datasors and it must be set
            // FastDataListView is the same, but behaves differently, because it is inherited from VirtualListView
            // just like FastListView, so it already has its own internal virtual datasource
            //this.listView = new FastDataListView ();
            //this.listView = new FastObjectListView ();
            //listView.VirtualMode = true; Should not be set for normal lists (this field is only for the Fast list and it is set internally.) When using the wizard, it is necessary to monitor whether it was installed.)
            if (isTreeList) listView = new TreeListView();
            else listView = new ObjectListView();
           
            listView.Cursor = Cursors.Default;
            listView.Dock = DockStyle.Fill;
            listView.Name = "listView";
            listView.TabIndex = 0;
            //ovl settings 
            listView.ShowGroups = false;
            listView.EmptyListMsg = FrwCRUDRes.List_No_Records;
            listView.EmptyListMsgFont = new Font("Tahoma", 9);//todo;
            listView.FullRowSelect = true;
            listView.IsSimpleDragSource = true;// drag drop
            listView.IsSimpleDropSink = true;//drag drop
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details; 

            listView.UseFiltering = true;//can filter 
            listView.UseFilterIndicator = true;
            listView.AllowColumnReorder = true; 
            listView.TriStateCheckBoxes = false;//todo If you want the user to be able to give check boxes the Indeterminate value, you should set the ObjectListView.TriStateCheckBoxes property to true.
            listView.TintSortColumn = true;//If you set TintSortColumn property to true, the sort column will be automatically tinted. The color of the tinting is controlled by the SelectedColumnTint property.
            listView.ShowItemToolTips = true;

            //listView.UseHotItem = true;

            listView.UseHyperlinks = true;
            listView.HyperlinkClicked += ListView_HyperlinkClicked;
            listView.SelectColumnsOnRightClickBehaviour = ObjectListView.ColumnSelectBehaviour.Submenu;
            listView.ShowCommandMenuOnRightClick = true;
            listView.TintSortColumn = true;
            listView.GridLines = true;
            listView.UseAlternatingBackColors = true;
            listView.AlternateRowBackColor = Color.Azure;
            //Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(220))))); //Color.LightGreen;// Color.FromArgb(35, 35, 42) - not working
            //Make sure HeaderUsesThemes is false. If this is true, ObjectListView will use the OS’s theme to draw the header, ignoring the HeaderFormatStyle completely.
            listView.HeaderUsesThemes = false;
            var headerstyle = new HeaderFormatStyle();
            headerstyle.Normal.BackColor = Color.LightBlue;// .SetBackColor(Color.LightBlue);
            listView.HeaderFormatStyle = headerstyle;
            // support for editing
            // activates by double click
            // it is possible to activate editing by F2 if double-click still has to be occupied by another
            listView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            listView.CellEditUseWholeCell = false;
            // double click is no longer used for anything
            //listView.ItemActivate += new EventHandler(listView_ItemActivate);
            listView.CellRightClick += new EventHandler<CellRightClickEventArgs>(this.listView_CellRightClick);
            listView.SelectionChanged += new EventHandler(listView_SelectionChanged);
            listView.SubItemChecking += ListView_SubItemChecking;// A special handler is needed because CellEditFinished does not cover CheckboxColumn
            listView.HeaderCheckBoxChanging += ListView_HeaderCheckBoxChanging;// A special handler is needed because CellEditFinished does not cover CheckboxColumn
            listView.CellEditStarting += ListView_CellEditStarting;
            listView.CellEditFinished += ListView_CellEditFinished;
            listView.CanDrop += ListView_CanDrop;
            listView.Dropped += ListView_Dropped;
            listView.ModelCanDrop += ListView_ModelCanDrop;
            listView.ModelDropped += ListView_ModelDropped;
            listView.CellToolTipShowing += ListView_CellToolTipShowing;
            //add to container
            toolStripContainer1.ContentPanel.Controls.Add(listView);
        }

        private void ListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            string stringValue = null;
            try
            {
                OLVColumn column = e.Column ?? e.ListView.GetColumn(0);

                if (column.AspectName != null && AppManager.Instance.IsCustomEditProperty(SourceObjectType, column.AspectName))
                {
                    object objectValue = Dm.Instance.GetCustomPropertyValue(e.Model, column.AspectName, true, AppManager.Instance.TooltipTruncatedMaxItemCount, AppManager.Instance.TooltipTruncatedMaxLength);
                    stringValue = objectValue != null ? objectValue.ToString() : null;
                    if (string.IsNullOrEmpty(stringValue)) return;
                    else
                    {
                        e.ToolTipControl.SetMaxWidth(400);
                        // Changing colour doesn't work in systems other than XP
                        e.BackColor = Color.AliceBlue;
                        e.ForeColor = Color.IndianRed;
                        e.AutoPopDelay = 15000;
                        e.Text = stringValue;
                    }
                }
                else
                {
                    return;//default tooltip 
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                stringValue = ex.ToString();
            }
        }

        override protected void CopySelectedObjectToClipBoard(IList sourceObject, bool cut)
        {
            //windows
            if (sourceObject.Count > 1)
                listView.CopyObjectsToClipboard(sourceObject);
            else
                Clipboard.SetText(ModelHelper.ModelPropertyList(sourceObject[0], "\n", null, null));
            //app
            FrwOlvDataObject dataObject = new FrwOlvDataObject();
            dataObject.SelectedObjects = sourceObject;
            CopyAdditionalObjectsToAppClipboard(sourceObject, cut, dataObject);
            AppManager.Instance.Clipboard.DataObject = dataObject;  // use our buffer, because SourceDataObject.GetData (typeof (BrightIdeasSoftware.OLVDataObject) returns null (apparently due to serialization)
            AppManager.Instance.Clipboard.IsCutOperation = cut;
        }
        private void ListView_ModelDropped(object sender, ModelDropEventArgs e)
        {
            try
            {
                Console.WriteLine(SourceObjectType.Name + " ModelDropped " + e.TargetModel);
            }
            catch(Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private bool dropLocker = false;
        private void ListView_ModelCanDrop(object sender, ModelDropEventArgs e)
        {
            try
            {

                if (CanCopyOrCutSelectedObjects(e.SourceModels, false) == true)
                {
                    // transfer style / copy standard, note that it's the opposite style in zotero
                    bool cut = false;
                    if (((e.DragEventArgs.KeyState & 4) == 4 || (e.DragEventArgs.KeyState & 8) == 8) &&
                        (e.DragEventArgs.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        //4 - SHIFT KeyState.
                        //8 - CTRL KeyState.
                        e.Effect = DragDropEffects.Copy;
                        e.DragEventArgs.Effect = DragDropEffects.Copy;
                        e.InfoMessage = FrwCRUDRes.Drag_Copy;
                        e.DragEventArgs.Data.SetData(new DragDropEffectsWrapper(DragDropEffects.Copy));
                        cut = false;


                    }
                    else if ((e.DragEventArgs.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {
                        //no key
                        e.Effect = DragDropEffects.Move;
                        e.DragEventArgs.Effect = DragDropEffects.Move;
                        e.InfoMessage = FrwCRUDRes.Drag_Move;
                        e.DragEventArgs.Data.SetData(new DragDropEffectsWrapper(DragDropEffects.Move));
                        cut = true;
                    }
                    CopyAdditionalObjectsToAppClipboard(e.SourceModels, cut, e.DragEventArgs.Data);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void ListView_Dropped(object sender, OlvDropEventArgs e)
        {
            try
            {
                Console.WriteLine(SourceObjectType.Name + " Dropped " + e.DataObject);
                bool cut = (e.Effect == DragDropEffects.Move);
                bool res = PasteSelectedObjectsFromIDataObject(e.DropTargetItem != null ? e.DropTargetItem.RowObject: null, e.DataObject as IDataObject, cut);
                if (res) MessageBox.Show(FrwCRUDRes.Drag_Record_Successfully + (cut ? FrwCRUDRes.Drage_Moved : FrwCRUDRes.Drag_Copied));
             }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private void ListView_CanDrop(object sender, OlvDropEventArgs e)
        {
            try
            {
                if (dropLocker)
                {
                    e.Effect = DragDropEffects.None;
                    dropLocker = false;
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        protected void CreateColumHeaderImage(OLVColumn column, string imageName)
        {
            if (imageName != null)
            {
                if (this.listView.SmallImageList == null) this.listView.SmallImageList = new ImageList();
                if (!this.listView.SmallImageList.Images.ContainsKey(imageName))
                {
                    Image smallImage = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    if (smallImage != null)
                    {
                        this.listView.SmallImageList.Images.Add(imageName, smallImage);
                    }
                }
                column.HeaderImageKey = imageName;
            }
        }
        // create a typical handler for the image        
        protected bool CreateImageGetterDelegate(OLVColumn column, Type sourceObjectType)
        {
            Type pType = AttrHelper.GetPropertyType(sourceObjectType, column.AspectName);
            // the code for the case of the dictionary is implemented
            JImageName imageNameAttr = AttrHelper.GetAttribute<JImageName>(SourceObjectType, column.AspectName);
            JImageRef imageRefAttr = AttrHelper.GetAttribute<JImageRef>(SourceObjectType, column.AspectName);
            JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(SourceObjectType, column.AspectName);
            if (dictAttr != null && dictAttr.DictPropertyStyle != DisplyPropertyStyle.TextOnly &&
                dictAttr.AllowMultiValues == false) //todo
            {
                column.ImageGetter = delegate (object x)
                {
                    if (dictAttr.AllowMultiValues) return null;
                    object value = AttrHelper.GetPropertyValue(x, column.AspectName);
                    if (value == null) return null;
                    JDictItem item = Dm.Instance.GetDictText(dictAttr.Id, value.ToString());
                    if (item == null) return null;
                    Image smallImage = AddImageToImageList(this.listView, item.Image, item.ImageName);
                    if (smallImage != null && item.Image == null) item.Image = smallImage;
                    return smallImage;
                };
                return true;
            }
            else if (imageNameAttr != null && imageNameAttr.DictPropertyStyle != DisplyPropertyStyle.TextOnly)
            {
                column.ImageGetter = delegate (object x)
                {
                    object value = AttrHelper.GetPropertyValue(x, column.AspectName);
                    if (value == null) return null;
                    Image smallImage = AddImageToImageList(this.listView, null, value.ToString());
                    //if (smallImage != null && item.Image == null) item.Image = smallImage;
                    return smallImage;
                };
                return true;
            }
            else if (imageRefAttr != null && imageRefAttr.DictPropertyStyle != DisplyPropertyStyle.TextOnly)
            {
                column.ImageGetter = delegate (object x)
                {
                    object value = AttrHelper.GetPropertyValue(x, column.AspectName);
                    if (value == null) return null;
                    PropertyInfo propInfo = x.GetType().GetProperty(column.AspectName);
                    PropertyInfo imagePropInfo = AttrHelper.GetPropertiesWithAttribute<JImageName>(propInfo.PropertyType).FirstOrDefault<PropertyInfo>();
                    if (imagePropInfo == null) return null;
                    value = imagePropInfo.GetValue(value);
                    if (value == null) return null;

                    AttrHelper.GetProperty<JImageName>(propInfo.PropertyType);
                    Image smallImage = AddImageToImageList(this.listView, null, value.ToString());
                    //if (smallImage != null && item.Image == null) item.Image = smallImage;
                    return smallImage;
                };
                return true;
            }
            else if (pType == typeof(JAttachment) || AttrHelper.IsGenericListTypeOf(pType, typeof(JAttachment)) ||
                AttrHelper.GetAttribute<JText>(sourceObjectType, column.AspectName) != null)
            {
                column.ImageGetter = delegate (object x)
                {
                    object v = AttrHelper.GetPropertyValue(x, column.AspectName);
                    if (v != null)
                    {
                        Image smallImage = null;
                        if (AttrHelper.GetAttribute<JText>(sourceObjectType, column.AspectName) != null)
                            smallImage = (Image)Properties.Resources.book_open;
                        else if (pType == typeof(JAttachment) || AttrHelper.IsGenericListTypeOf(pType, typeof(JAttachment)))
                            smallImage = (Image)Properties.Resources.attachment;
                        return smallImage;
                    }
                    else return null;
                };
                return true;
            }
            return false;
        }
        //decoration example 
        protected void AddColumnToList(OLVColumn column)
        {
            listView.Columns.AddRange(new ColumnHeader[] { column });//may be only TreeList ? 
            listView.AllColumns.Add(column);
        }
        #region saverestorestate

        private string configFileStr = null;
        private void LoadConfig()
        {
            try
            {
                string filename = null;
                if (DlgMode)
                {
                    filename = GetStateConfigFileName(true);
                    if (!File.Exists(filename)) filename = GetStateConfigFileName(false);
                }
                else
                {
                    filename = GetStateConfigFileName(false);
                }
                if (File.Exists(filename))
                {
                    configFileStr = File.ReadAllText(filename, Encoding.UTF8);
                    OLVStateAdv state = JsonSerializeHelper.LoadFromString<OLVStateAdv>(configFileStr);
                    if (listView != null)
                    {
                        OLVHelper.RestoreStateAdvanced(listView, state);
                    }
                    HotItemStyle = state.HotItemStyle;
                    OLVHelper.SetHotItemStyle(listView, HotItemStyle);
                    LoadUserSettings(state.UserSettings);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Can not read config for " + GetType().ToString(), ex);
            }

        }
        override public void SaveConfig()
        {
            try
            {
                string filename = GetStateConfigFileName(DlgMode);
                OLVStateAdv state = new OLVStateAdv();
                SaveUserSettings(state.UserSettings);
                if (listView != null)
                {
                    OLVHelper.SaveStateAdvanced(listView, state);
                }
                state.HotItemStyle = HotItemStyle;
                string newConfigStr = JsonSerializeHelper.SaveToString(state);
                if (newConfigStr.Equals(configFileStr) == false)
                {
                    File.WriteAllText(filename, newConfigStr, Encoding.UTF8);
                    configFileStr = newConfigStr;
                    //Log.ProcessDebug("@@@@@ Saved config for OLV list  " + SourceObjectType);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Can not write config for " + GetType().ToString(), ex);
            }

        }
        #endregion

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            TimedFilter(listView, ((ToolStripTextBox)sender).Text, 0, (ToolStripTextBox)sender);
        }

        public OLVColumn GetColumnByName(string name)
        {
            foreach (OLVColumn column in listView.AllColumns)
            {
                if (column.Name == name)
                    return (OLVColumn)column;
            }
            return null;
        }
        public OLVColumn GetColumnByAspectName(string name)
        {
            foreach (OLVColumn column in listView.AllColumns)
            {
                if (column.AspectName == name)
                    return (OLVColumn)column;
            }
            return null;
        }
        private void ListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            try
            {
                OLVColumn column = e.Column;
                object rowObject = e.RowObject;
                //object value = e.Value;
                if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, column.AspectName))
                {
                    if (AttrHelper.GetAttribute<JText>(SourceObjectType, column.AspectName) != null)
                    {
                        // it is necessary to use a complex scheme, otherwise it cycles through the MouseUp event
                        Button b = new Button();
                        b.Image = Properties.Resources.book_open;
                        b.Bounds = e.CellBounds;
                        b.Font = ((ObjectListView)sender).Font;
                        b.Click += (s1, e1) =>
                        {
                            bool complated = false;
                            object newValue = AppManager.Instance.ProcessEditCustomPropertyValueAndSave(rowObject, column.AspectName, out complated, this);
                            if (complated)
                            {
                                RefreshObject(rowObject);
                                b.Text = newValue as string;
                            }
                            b.Dispose();
                        };
                        e.Control = b;
                        e.Cancel = false;
                    }
                    else
                    {
                        bool complated = false;
                        e.NewValue = AppManager.Instance.ProcessEditCustomPropertyValueAndSave(rowObject, column.AspectName, out complated, this);
                        if (complated)
                        {
                            RefreshObject(rowObject);
                        }
                        e.Cancel = true;
                    }
                }
                else e.Cancel = false;

            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
                e.Cancel = true;
            }
        }
        private void ListView_CellEditFinished(object sender, CellEditEventArgs e)
        {
            OLVColumn column = e.Column;
            object rowObject = e.RowObject;
            if (AttrHelper.GetAttribute<JText>(SourceObjectType, column.AspectName) != null)
            {
                e.Cancel = true;
                RefreshObject(rowObject);
                //((ObjectListView)sender).RefreshItem(e.ListViewItem);
            }
            try
            {
                Dm.Instance.SaveObject(e.RowObject);
            }
            catch (JValidationException ex)
            {
                dropLocker = true;//xak
                AppManager.ShowValidationErrorMessage(ex.ValidationResult, this);
                AttrHelper.SetPropertyValue(rowObject, column.AspectName, e.Value);
                RefreshObject(rowObject);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }



        private void listView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                object selectedObject = null;
                if (listView.SelectedIndex > -1) selectedObject = listView.GetItem(listView.SelectedIndex).RowObject;
                if (selectedObject != null)
                    OnSelectObject(selectedObject);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        // the handler can be disabled so that it does not conflict with the activation of the editing mode
        private void listView_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                // double click on the line
                object selectedObject = null;
                if (listView.SelectedIndex > -1) selectedObject = listView.GetItem(listView.SelectedIndex).RowObject;
                if (selectedObject != null)
                    ViewObjectContentLocal(selectedObject);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private void ListView_SubItemChecking(object sender, SubItemCheckingEventArgs e)
        {
            try
            {
                try
                {
                    Dm.Instance.SaveObject(e.RowObject);
                }
                catch (JValidationException ex)
                {
                    AppManager.ShowValidationErrorMessage(ex.ValidationResult);
                    //todo
                    //AttrHelper.SetPropertyValue(rowObject, column.AspectName, e.Value);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void ListView_HyperlinkClicked(object sender, HyperlinkClickedEventArgs e)
        {
            //Just to be complete, when a hyperlink is clicked, ObjectListView triggers a HyperlinkClickd event
            //(no prizes for guessing that). If you listen for and handle this event, set Handled to true so that 
            //the default processing is not done. By default, ObjectListView will try to open the URL,
            //using System.Diagnostics.Process.Start()
        }
        private void ListView_HeaderCheckBoxChanging(object sender, HeaderCheckBoxChangingEventArgs e)
        {
            try
            {
                Dm.Instance.SetEntityModified(SourceObjectType);
                // todo it will be correct for all visible objects to call UpdateObjectInStorage (e.RowObject, null);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        override protected object GetSelectedListItem()
        {
            if (listView.SelectedIndex > -1) return listView.GetItem(listView.SelectedIndex);
            else return null;
        }
        override protected IList GetSelectedObjects()
        {
            return listView.SelectedObjects;
        }
        override protected IList GetSelectedForCopyPaste()
        {
            if (listView.SelectedObjects == null || listView.SelectedObjects.Count == 0)
            {
                return null;
            }
            else return listView.SelectedObjects;

        }
        private void columnsButton_Click(object sender, EventArgs e)
        {
            ColumnSelectionForm form = new ColumnSelectionForm();
            form.OpenOn(listView);
        }

        protected OLVColumn MakeButtonColumn(string name, string text)
        {
            //this.listView.GetColumn()
            OLVColumn column = null;
            column = new OLVColumn();
            column.Name = name;
            column.Text = text;
            if (text != null)
            {
                column.AspectGetter = delegate (Object rowObject)
                {
                    return text;
                };
            }

            // Renderer must be installed earlier IsButton = true
            //column.Renderer = new StaticTextColumnButtonRenderer () {StaticText = text};
            // for some strange reason, there is a background error 'System.IO.FileNotFoundException' in System.Drawing.dll

            // Tell the columns that it is going to show buttons.
            // The label that goes into the button is the Aspect that would have been
            // displayed in the cell.
            column.IsButton = true;

            // How will the button be sized? That can either be:
            //   - FixedBounds. Each button is ButtonSize in size
            //   - CellBounds. Each button is as wide as the cell, inset by CellPadding
            //   - TextBounds. Each button resizes to match the width of the text plus ButtonPadding
            column.ButtonSizing = OLVColumn.ButtonSizingMode.CellBounds;
            //column.ButtonSize = new Size(80, 26);

            // Make the buttons clickable even if the row itself is disabled
            column.EnableButtonWhenItemIsDisabled = true;
            column.TextAlign = HorizontalAlignment.Center;
            return column;
        }
        private IModelFilter previousFilter = null;
        public void TimedFilter(ObjectListView olv, string txt, int matchKind, ToolStripTextBox textBox)
        {
            TextMatchFilter filter = null;
            if (!String.IsNullOrEmpty(txt))
            {
                switch (matchKind)
                {
                    case 0:
                    default:
                        filter = TextMatchFilter.Contains(olv, txt);
                        break;
                    case 1:
                        filter = TextMatchFilter.Prefix(olv, txt);
                        break;
                    case 2:
                        filter = TextMatchFilter.Regex(olv, txt);
                        break;
                }
            }

            // Text highlighting requires at least a default renderer
            if (olv.DefaultRenderer == null)
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //When the user chooses an Excel filter, any previously installed ModelFilter will be replaced. 
            //If the programmer wants their filter to be combined with the user chosen Excel filter,
            //the programmer should set the AdditionalFilter property instead of the ModelFilter.
            if (filter != null)
            {
                if (!(olv.AdditionalFilter is TextMatchFilter)) previousFilter = olv.AdditionalFilter;
                olv.AdditionalFilter = filter;
            }
            else
            {
                olv.AdditionalFilter = previousFilter;
            }
            //olv.Invalidate();
            stopWatch.Stop();

            IList objects = olv.Objects as IList;
            string message = null;


            if (objects == null)
                message =
                    String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            else
                message =
                    String.Format("Filtered {0} items down to {1} items in {2}ms",
                                  objects.Count,
                                  olv.Items.Count,
                                  stopWatch.ElapsedMilliseconds);
            //todo
            if (objects != null && objects.Count < olv.Items.Count)
            {
                textBox.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }
            else
            {
                textBox.BackColor = System.Drawing.SystemColors.Window;
            }

        }
        override protected void ShowSettingsDialog()
        {
            try
            {
                OLVListSettingDialog sett = new OLVListSettingDialog(listView, this);
                sett.Show();
            }
            catch(Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        static public Image AddImageToImageList(ObjectListView listView, Image image, string imageName)
        {
            if (image != null) return image; //we do not need use ImageList
            if (imageName != null)
            {
                if (listView.SmallImageList == null) listView.SmallImageList = new ImageList();
                if (!listView.SmallImageList.Images.ContainsKey(imageName))
                {
                    Image smallImage = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    //if not found in current assembly do advanced search 
                    if (smallImage == null) smallImage = TypeHelper.FindImageInAllDiskStorages(imageName);
                    if (smallImage == null) smallImage = TypeHelper.FindImageInAllAssemblyResources(imageName);
                    if (smallImage != null)
                    {
                        listView.SmallImageList.Images.Add(imageName, smallImage);
                        return smallImage;
                    }
                    else return null;
                    //todo for big icon mode listView.LargeImageList
                }
                else return listView.SmallImageList.Images[imageName];
            }
            else return null;
        }
        override protected void AddObject(object selectedListItem, object selectedObject, Type sourceObjectType, IDictionary<string, object> extraParams = null)
        {
            object newObject = AddObjectLocal(selectedObject, SourceObjectType);
            /*
            if (newObject != null)
            {
                // the standard listView method, when the filter is enabled, adds a record to the sources and rebuilds the list
                // and at us record in the source is already added, therefore the duplication
                // If we are filtering the list, there is no way to efficiently
                // insert the objects, so just put them into our collection and rebuild.
                if (listView.IsFiltering)
                {
                    //index = Math.Max(0, Math.Min(index, ourObjects.Count));
                    //ourObjects.InsertRange(index, modelObjects);
                    listView.BuildList(true);
                }
                else
                {
                    this.listView.AddObject(newObject);
                }
                this.listView.EnsureModelVisible(newObject); //? 
            }
            */
        }
        override protected void DeleteObject(object selectedListItem, object[] selectedObjects)
        {

            DialogResult res = MessageBox.Show(null, FrwCRUDRes.List_Delete_Record_Confirmation,
                             FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                if (DeleteObjectLocal(selectedObjects))
                {
                    listView.RemoveObjects(selectedObjects);
                }
            }
        }
        override protected void DeleteAllObjects()
        {
            DialogResult res = MessageBox.Show(null, FrwCRUDRes.List_Delete_All_records_Confirmation,
                               FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {

                if (DeleteAllObjectsLocal())
                    this.listView.ClearObjects();
            }
        }

        private void listView_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
        {

            try
            {
                object selectedObject = e.Model;

                List<ToolStripItem> menuItemList = new List<ToolStripItem>();
                // add this item here to take into account e.Column
                // add other menu items in the override MakeContextMenu
                string aspectName = null;
                if (e.Column != null)
                {
                    aspectName = e.Column.AspectName;
                    /*
                    if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, aspectName))
                    {

                        ToolStripMenuItem menuItem = new ToolStripMenuItem();
                        menuItem.Text = FrwCRUDRes.List_Edit_Field;
                        menuItem.Click += (s, em) =>
                        {
                            try
                            {
                                //todo button xak ?
                                object rowObject = e.Model;
                                bool cancel = false;
                                object newValue = AppManager.Instance.EditCustomPropertyValue(rowObject, aspectName, out cancel, this);
                                Dm.Instance.InsertOrUpdateObject(rowObject);
                                RefreshObject(rowObject);
                            }
                            catch (Exception ex)
                            {
                                Log.showError(ex);
                            }
                        };
                        menuItemList.Add(menuItem);
                        menuItemList.Add(new ToolStripSeparator());
                        */
                        /* todo
                        if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, aspectName))
                        {
                            object rowObject = e.Model;
                            object v = AttrHelper.GetPropertyValue(rowObject, aspectName);
                            if (v != null) {
                                menuItem = new ToolStripMenuItem();
                                menuItem.Text = FrwCRUDRes.List_View_Field;
                                menuItem.Click += (s, em) =>
                                {
                                    try
                                    {
                                        Dm.Instance.ResolveRelation(rowObject, aspectName);
                                        //IList rvs = (IList)AttrHelper.GetPropertyValue(rowObject, aspectName);
                                        if (rvs != null && rvs.Count > 0)
                                        {
                                            IPropertyProcessor propertyControl = null;
                                            if (propertyDialog == null)
                                            {
                                                propertyControl = (IPropertyProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IPropertyProcessor), rvs[0].GetType(), null);
                                                propertyDialog = new SimplePropertyDialog(propertyControl);
                                            }
                                            else propertyControl = propertyDialog.PropertyWindow;
                                            propertyControl.ViewMode = ViewMode.View;
                                            propertyControl.SourceObject = rvs[0];
                                            propertyControl.ProcessView();
                                            DialogResult res = propertyDialog.ShowDialog();
                                        }
                                        else
                                        {
                                            MessageBox.Show(FrwCRUDRes.Object_For_View_Not_Found);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.showError(ex);
                                    }
                                };
                                menuItemList.Add(menuItem);
                            }
                        }
                        */
                    //}
                }
                MakeContextMenu(menuItemList, e.Item, selectedObject, aspectName);


                if (menuItemList != null)
                {
                    this.contextMenu.Items.Clear();
                    this.contextMenu.Items.AddRange(menuItemList.ToArray<ToolStripItem>());
                    e.MenuStrip = this.contextMenu;
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
            menuItemList.Add(new ToolStripSeparator());

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = FrwCRUDRes.List_Copy_To_Clipboard_OLV;
            menuItem.Image = Properties.Resources.AllPics_15;
            menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            menuItem.ImageScaling = ToolStripItemImageScaling.None;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    GetSelectedForCopyPaste(); 
                    listView.CopySelectionToClipboard();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemList.Add(menuItem);
        }


    }
}
