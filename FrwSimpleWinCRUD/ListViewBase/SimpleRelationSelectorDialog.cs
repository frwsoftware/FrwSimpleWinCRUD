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


namespace FrwSoftware
{
    // modes of operation
    // display the contents of the field of the "styling list" type SourceObjectType is JStringWrapperDto
    // show selection of Entity objects SourceObjectType
    // show selection of dictionary positions DictId! = Null AllDictItems = false
    // show the entire dictionary DictId! = Null AllDictItems = true
    public partial class SimpleRelationSelectorDialog : BaseDialogForm
    {
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

        public SimpleRelationSelectorDialog()
        {

            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleDictListDialog_Dict;

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
            //listView.MultiSelect = allowMultiSelect;

            OLVColumn column = null;
            column = new OLVColumn();
            column.AspectName = "Text";
            column.Text = FrwCRUDRes.SimpleDictListDialog_Name;
            column.Width = 350;
            column.FillsFreeSpace = true;
            AddColumnToList(column);

            //SourceObjects = Dm.Instance.GetDictionaryItems(DictId);

            ((System.ComponentModel.ISupportInitialize)(listView)).EndInit();

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
