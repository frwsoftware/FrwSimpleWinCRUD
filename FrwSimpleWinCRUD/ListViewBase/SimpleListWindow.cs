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
using FrwSoftware;
using System.Reflection;

namespace FrwSoftware
{
    // is intended for objects that are fully declared and build the list automatically
    // if you use objects that can not be declared (for example, from third-party libraries) and there is no worm
    // then use BaseListWindow
    public partial class SimpleListWindow : BaseOLVListWindow
    {
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
                if (pType == typeof(bool))
                {
                    column.CheckBoxes = true;
                    column.HeaderCheckBox = true;// -not SubItemChecking, but only HeaderCheckBoxChanging 
                }
                if (AttrHelper.GetAttribute<JUrl>(propInfo) != null) column.Hyperlink = true;
                column.Text = ModelHelper.GetPropertyJDescriptionOrName(propInfo);
                if (headerImageAttr != null&& headerImageAttr.HeaderImageName != null)
                {
                    this.CreateColumHeaderImage(column, headerImageAttr.HeaderImageName);
                    column.ShowTextInHeader = false;
                }
                if ( AttrHelper.GetAttribute<JReadOnly>(SourceObjectType, column.AspectName) != null) column.IsEditable = false;
                if (AppManager.Instance.IsCustomEditProperty(SourceObjectType, column.AspectName)){
                    column.AspectGetter = delegate (Object rowObject)
                    {
                        try
                        {
                            if (AppManager.Instance.IsOnlyImageColumnProperty(SourceObjectType, column.AspectName)) return null;
                            else return Dm.Instance.GetCustomPropertyValue(rowObject, column.AspectName, true,
                                AppManager.Instance.ListCellTruncatedMaxItemCount, AppManager.Instance.ListCellTruncatedMaxLength);
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
        }

        override protected void ReloadList()
        {
            this.listView.SetObjects(Dm.Instance.FindAll(SourceObjectType));
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
                //if (Attribute.GetCustomAttribute(pinfo, typeof(JManyToOne)) != null)
                  //  continue; //обычно дублирована с JForeinKey поэтому оставляем одну 

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
                if (column != null) columns.Add(column);
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
