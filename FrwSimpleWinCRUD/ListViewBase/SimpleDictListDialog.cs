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
    // modes of operation
    // display the contents of the field of the "styling list" type SourceObjectType is JStringWrapperDto
    // show selection of Entity objects SourceObjectType
    // show selection of dictionary positions DictId! = Null AllDictItems = false
    // show the entire dictionary DictId! = Null AllDictItems = true
    public partial class SimpleDictListDialog : BaseDialogForm
    {
        public string DictId { get; set; }
        protected SimplePropertyDialog propertyDialog = null;

        List<object> selectedObjects = new List<object>();
        private IList objectList = null;

        public IList SourceObjects
        {
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

        public SimpleDictListDialog(string dictId, bool allowMultiSelect)
        {

            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleDictListDialog_Dict;

            this.DictId = dictId;
            ((System.ComponentModel.ISupportInitialize)(listView)).BeginInit();
            //ovl settings 
            listView.EmptyListMsg = FrwCRUDRes.List_No_Records;
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
            listView.MultiSelect = allowMultiSelect;

            OLVColumn column = null;
            column = new OLVColumn();
            column.AspectName = "Text";
            column.Text = FrwCRUDRes.SimpleDictListDialog_Name;
            column.Width = 350;
            column.FillsFreeSpace = true;
            AddColumnToList(column);

            column = new OLVColumn();
            column.AspectName = "Image";
            column.Text = FrwCRUDRes.SimpleDictListDialog_Image;
            column.Width = 50;
            column.ImageGetter = delegate (object x)
            {
                JDictItem item = (JDictItem)x;

                Image smallImage = BaseOLVListWindow.AddImageToImageList(this.listView, item.Image, item.ImageName);
                if (smallImage != null) item.Image = smallImage;
                return smallImage;
            };
            AddColumnToList(column);

            SourceObjects = Dm.Instance.GetDictionaryItems(DictId);
            this.listView.ItemsAdding += ListView_ItemsAdding;
            this.listView.ItemsRemoving += ListView_ItemsRemoving;

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
        public IList SelectedObjects
        {
            get
            {
                return selectedObjects;
            }

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
                //dict edit 
                SimpleJDictItemDialog dialog = new SimpleJDictItemDialog(new JDictItem());
                DialogResult res = dialog.ShowDialog();
                if (res == DialogResult.OK && dialog.DictItem.Key != null)
                {
                    JDictItem newObject = dialog.DictItem;
                    bool oPresent = false;
                    foreach (var o in listView.Objects)
                    {
                        if (((JDictItem)o).Key.Equals(newObject.Key))
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
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedObjects.Clear();
            foreach (var o in listView.SelectedObjects)
            {
                selectedObjects.Add(o);
            }
        }

    }


}
