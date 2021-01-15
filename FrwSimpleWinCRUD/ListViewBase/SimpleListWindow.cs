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
using System.Collections;

using System.Reflection;

namespace FrwSoftware
{
    // is intended for objects that are fully declared and build the list automatically
    // if you use objects that can not be declared (for example, from third-party libraries) and there is no worm
    // then use BaseListWindow
    public partial class SimpleListWindow : BaseOLVListWindow
    {
        protected CheckBox isActiveCheckBox = new CheckBox();

        public SimpleListWindow()
        {
            InitializeComponent();



        }

        override protected void MakeListColumns()
        {
            JModelOVLGenerator.Instance = new JModelOVLGenerator();
            JModelOVLGenerator.GenerateColumns(this.listView, SourceObjectType, true);
            foreach (var column in this.listView.AllColumns)
            {
                JHeaderImage headerImageAttr = AttrHelper.GetAttribute<JHeaderImage>(SourceObjectType, column.AspectName);
                PropertyInfo propInfo = AttrHelper.GetProperty(SourceObjectType, column.AspectName);
                Type pType = propInfo.PropertyType;
                if (isVirtualList)
                {
                    column.Sortable = false;
                }
                //else { 
                    if (pType == typeof(bool))
                    {
                        column.CheckBoxes = true;
                        column.HeaderCheckBox = true;// -not SubItemChecking, but only HeaderCheckBoxChanging 
                    }
                //}
                if (AttrHelper.GetAttribute<JUrl>(propInfo) != null) column.Hyperlink = true;
                column.Text = ModelHelper.GetPropertyJDescriptionOrName(propInfo);
                if (headerImageAttr != null && headerImageAttr.HeaderImageName != null)
                {
                    this.CreateColumHeaderImage(column, headerImageAttr.HeaderImageName);
                    column.ShowTextInHeader = false;
                }
                if (AttrHelper.GetAttribute<JReadOnly>(SourceObjectType, column.AspectName) != null) column.IsEditable = false;
                if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, column.AspectName))
                {
                    column.AspectGetter = delegate (Object rowObject)
                    {
                        try
                        {
                            if (AppManager.Instance.IsOnlyImageColumnProperty(SourceObjectType, column.AspectName)) return null;
                            else
                            {
                                return Dm.Instance.GetCustomPropertyValue(rowObject, column.AspectName, true,
                                AppManager.Instance.ListCellTruncatedMaxItemCount, AppManager.Instance.ListCellTruncatedMaxLength);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogError(ex);
                            return ex.ToString();
                        }
                    };
                    CreateImageGetterDelegate(column, SourceObjectType);
                }
            }

            bool isIsArchivedFieldPresent = ModelHelper.IsIsArchivedFieldPresent(SourceObjectType);
            if (isIsArchivedFieldPresent)
            {
                isActiveCheckBox.Text = FrwCRUDRes.Active;
                isActiveCheckBox.Checked = true;
                isActiveCheckBox.CheckStateChanged += (s, ex) => { listView.UpdateColumnFiltering(); };
                AddToolStripItem(new ToolStripControlHost(isActiveCheckBox));

                listView.AdditionalFilter = new ModelFilter(delegate (object x)
                {
                    return (isActiveCheckBox.Checked ? !ModelHelper.GetIsArchivedValue(x) : true);
                });
            }
            if (isIsArchivedFieldPresent || ModelHelper.IsTextColoredFieldPresent(SourceObjectType) || ModelHelper.IsExpiredFieldPresent(SourceObjectType))
            {
                listView.UseCellFormatEvents = true;
                listView.FormatCell += ListView_FormatCell;
            }
        }
        private void ListView_FormatCell(object sender, FormatCellEventArgs e)
        {
            object item = e.Model;
            if (item != null)
            {
                PropertyInfo p = item.GetType().GetProperty("IsArchived");
                if (p != null && p.PropertyType == typeof(bool) && (bool)(p.GetValue(item)) == true)
                {
                    e.SubItem.ForeColor = Color.Gray;
                }
                else
                {

                    if (e.Column.AspectName == null) return;
                    Type pType = AttrHelper.GetPropertyType(SourceObjectType, e.Column.AspectName);
                    PropertyInfo propInfo = SourceObjectType.GetProperty(e.Column.AspectName);
                    JExpired expiredProp = AttrHelper.GetAttribute<JExpired>(propInfo);
                    if (expiredProp != null)
                    {
                        object propValue = null;
                        if (expiredProp.ExpiredProperty != null)
                        {
                            PropertyInfo propInfo1 = SourceObjectType.GetProperty(expiredProp.ExpiredProperty);
                            if (propInfo1 != null) propValue = AttrHelper.GetPropertyValue(e.Model, propInfo1);
                        }
                        else
                            propValue = AttrHelper.GetPropertyValue(e.Model, propInfo);
                        if (propValue != null)
                        {
                            ModelHelper.ExpiredToColor(propValue.ToString(), e.SubItem.ForeColor);
                        }
                    }
                    else
                    {

                        JDictProp dictProp = AttrHelper.GetAttribute<JDictProp>(propInfo);
                        if (dictProp != null &&
                            (dictProp.DictPropertyStyle == DisplyPropertyStyle.ColoredTextOnly || dictProp.DictPropertyStyle == DisplyPropertyStyle.ColoredTextAndImage))
                        {
                            object propValue = AttrHelper.GetPropertyValue(e.Model, propInfo);
                            if (propValue != null)
                            {
                                JDictItem ditem = Dm.Instance.GetDictText(dictProp.Id, propValue.ToString());
                                if (ditem != null && ditem.TextColor != null && ditem.TextColor != Color.Black)
                                {
                                    e.SubItem.ForeColor = ditem.TextColor;
                                }
                            }
                        }
                    }
                }
            }
        }

        override protected void LoadUserSettings(IDictionary<string, object> userSettings)
        {
            base.LoadUserSettings(userSettings);
            isActiveCheckBox.Checked = DictHelper.GetValueAsBool(userSettings, "isActiveCheckBox");

        }
        override protected void SaveUserSettings(IDictionary<string, object> userSettings)
        {
            base.SaveUserSettings(userSettings);
            userSettings.Add("isActiveCheckBox", isActiveCheckBox.Checked);
        }

        override protected void ReloadList()
        {
            if (!(isVirtualList || NoDmMode || AttrHelper.IsAttributeDefinedForType<JEntity>(SourceObjectType, true) == false))
            {
                this.listView.SetObjects(Dm.Instance.FindAll(SourceObjectType));
            }
        }
    }

    public class JModelOVLGenerator : Generator
    {

        /// <summary>
        /// Generate a list of OLVColumns based on the attributes of the given type
        /// If allProperties to true, all public properties will have a matching column generated.
        /// If allProperties is false, only properties that have a OLVColumn attribute will have a column generated.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn].</param>
        /// <returns>A collection of OLVColumns matching the attributes of Type that have OLVColumnAttributes.</returns>
        public override IList<OLVColumn> GenerateColumns(Type type, bool allProperties)
        {
            List<OLVColumn> columns = new List<OLVColumn>();

            // Sanity
            if (type == null)
                return columns;

            PropertyInfo[] pList = AttrHelper.GetBasePropertiesFirst(type);
            PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(type);
            if (pName != null)
            {
                pList = pList.OrderBy(x => x != pName).ToArray();
            }

            // Iterate all public properties in the class and build columns from those that have
            // an OLVColumn attribute and that are not ignored.
            //foreach (PropertyInfo pinfo in type.GetProperties())
            foreach (PropertyInfo pinfo in pList)
            {
                if (Attribute.GetCustomAttribute(pinfo, typeof(OLVIgnoreAttribute)) != null)
                    continue;
                //added j 
                if (Attribute.GetCustomAttribute(pinfo, typeof(JIgnore)) != null)
                    continue;

                OLVColumnAttribute attr = Attribute.GetCustomAttribute(pinfo, typeof(OLVColumnAttribute)) as OLVColumnAttribute;
                OLVColumn column = null;
                if (attr == null)
                {
                    if (allProperties)
                        column = this.MakeColumnFromPropertyInfo(pinfo);
                }
                else
                {
                    column = this.MakeColumnFromAttribute(pinfo, attr);
                }
                if (column != null)
                {
                    //reset checkboxes (setted by ConfigurePossibleBooleanColumn()) for correct working of virtual lists 
                    column.CheckBoxes = false;
                    columns.Add(column);
                }
            }

            // How many columns have DisplayIndex specifically set?
            int countPositiveDisplayIndex = 0;
            foreach (OLVColumn col in columns)
            {
                if (col.DisplayIndex >= 0)
                    countPositiveDisplayIndex += 1;
            }

            // Give columns that don't have a DisplayIndex an incremental index
            int columnIndex = countPositiveDisplayIndex;
            foreach (OLVColumn col in columns)
                if (col.DisplayIndex < 0)
                    col.DisplayIndex = (columnIndex++);

            columns.Sort(delegate (OLVColumn x, OLVColumn y) {
                return x.DisplayIndex.CompareTo(y.DisplayIndex);
            });

            return columns;
        }

    }

}
