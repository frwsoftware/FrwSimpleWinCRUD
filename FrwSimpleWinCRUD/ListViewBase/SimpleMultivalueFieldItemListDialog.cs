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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FrwSoftware;

namespace FrwSoftware
{
    public partial class SimpleMultivalueFieldItemListDialog : BaseDialogForm
    {
        public Type SourceObjectType { get; set; }
        protected SimplePropertyDialog propertyDialog = null;
        private IList objectList = null;

        public IList SourceObjects {
            get
            {
                return objectList;
            }
            set
            {
                objectList = value;
                listView.SetObjects(objectList);
            }
        }

        public SimpleMultivalueFieldItemListDialog(Type SourceObjectType)
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleMultivalueFieldItemListDialog_Title;

            this.SourceObjectType = SourceObjectType;
            ((System.ComponentModel.ISupportInitialize)(listView)).BeginInit();
            //ovl settings 
            listView.EmptyListMsg =FrwCRUDRes.List_No_Records;
            listView.EmptyListMsgFont = new Font("Tahoma", 9);//todo;
            listView.FullRowSelect = true;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            listView.UseFiltering = true;
            listView.UseFilterIndicator = true; 
            listView.AllowColumnReorder = true; 
            listView.TriStateCheckBoxes = false;
            listView.CellEditUseWholeCell = false;
            listView.TintSortColumn = true;
            listView.ShowItemToolTips = true;
            listView.UseHotItem = true; 
            listView.UseHyperlinks = true;
            listView.ShowCommandMenuOnRightClick = true;
            listView.TintSortColumn = true;

            OLVColumn column = null;

            PropertyInfo pPK = AttrHelper.GetProperty<JPrimaryKey>(SourceObjectType);

            column = new OLVColumn();
            if (pPK != null)
            {

                column.AspectName = pPK.Name;
                column.Text = ModelHelper.GetPropertyJDescriptionOrName(pPK);
            }
            else
            {
                column.AspectName = "Id";
                column.Text = FrwCRUDRes.SimpleMultivalueFieldItemListDialog_Id;
            }
            column.Width = 50;
            //column.HeaderCheckBox = true;//for checkbox selection 
            AddColumnToList(column);

            PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(SourceObjectType);

            column = new OLVColumn();
            if (pName != null)
            {
                column.Text = ModelHelper.GetPropertyJDescriptionOrName(pName);
                column.AspectName = pName.Name;
            }
            else
            {
                column.AspectName = "Name";
                column.Text = FrwCRUDRes.SimpleMultivalueFieldItemListDialog_Name;
            }
            column.Width = 350;
            column.FillsFreeSpace = true;
            AddColumnToList(column);


            ((System.ComponentModel.ISupportInitialize)(listView)).EndInit();

        }
        private void ListView_ItemsRemoving(object sender, ItemsRemovingEventArgs e)
        {
            foreach (var o in e.ObjectsToRemove)
            {
                SourceObjects.Remove(o);
            }
        }

        private void ListView_ItemsAdding(object sender, ItemsAddingEventArgs e)
        {
            foreach (var o in e.ObjectsToAdd)
            {
                SourceObjects.Add(o);
            }
        }

        protected void AddColumnToList(OLVColumn column)
        {
            listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { column });//may be only TreeList ? 
            listView.AllColumns.Add(column);
        }
        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                //cheched
                bool checkedPresent = false;
                List<object> oToRemove = new List<object>();
                foreach (var o in listView.Objects)
                {
                    if (listView.IsChecked(o))
                    {
                        checkedPresent = true;
                        oToRemove.Add(o);
                        
                    }
                }
                if (oToRemove.Count > 0)
                    this.listView.RemoveObjects(oToRemove);
                //selected
                if (!(listView.SelectedObjects != null && listView.SelectedObjects.Count > 0))
                { 
                    if (!checkedPresent)
                    {
                        MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                    }
                    return;
                }
                listView.RemoveObjects(listView.SelectedObjects);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
          
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                IListProcessor fkList = (IListProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IListProcessor), SourceObjectType, null);
                SimpleListDialog listDialog = new SimpleListDialog(fkList);
                fkList.ProcessView();
                DialogResult res = listDialog.ShowDialog();
                if (res == DialogResult.OK && listDialog.SelectedObjects != null && listDialog.SelectedObjects.Count > 0)
                {
                    IList newObjects = listDialog.SelectedObjects;

                    foreach (var newObject in newObjects)
                    {
                        bool oPresent = false;
                        foreach (var o in listView.Objects)
                        {
                            if (o.Equals(newObject))
                            {
                                oPresent = true;
                                break;
                            }
                        }
                        if (oPresent == false)
                        {
                            this.listView.AddObject(newObject);
                            this.listView.EnsureModelVisible(newObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            objectList.Clear();
            foreach (var o in listView.Objects)
            {
                objectList.Add(o);
            }
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView.SelectedObjects != null && listView.SelectedObjects.Count > 1) MessageBox.Show("Выделено более одной записи");
                else if (listView.SelectedObjects == null || listView.SelectedObjects.Count == 0) MessageBox.Show("Нет выделенных записей");
                else
                {
                    object selectedObject = null;
                    if (listView.SelectedIndex > -1) selectedObject = listView.GetItem(listView.SelectedIndex).RowObject;
                    if (selectedObject != null)
                        ViewObjectLocal(selectedObject);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }
        protected void ViewObjectLocal(object selectedObject, dynamic extraParams = null)
        {
            Cursor cursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (selectedObject != null)
                {
                   
                        IPropertyProcessor propertyControl = null;
                        if (propertyDialog == null)
                        {
                            propertyControl = (IPropertyProcessor)AppManager.Instance.CreateNewContentInstance(typeof(IPropertyProcessor), SourceObjectType, null);
                            propertyDialog = new SimplePropertyDialog(propertyControl);
                        }
                        else propertyControl = propertyDialog.PropertyWindow;
                        propertyControl.ViewMode = ViewMode.View;
                        propertyControl.SourceObject = selectedObject;
                        propertyControl.ProcessView();
                        DialogResult res = propertyDialog.ShowDialog();
                
                }
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }
    }
}
