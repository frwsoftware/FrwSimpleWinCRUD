using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FrwSoftware;
using Flobbster.Windows.Forms;

namespace FrwSoftware
{
    //for PropertyGrid
    public class CustomPropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            PropertyBag bag = (PropertyBag)context.Instance;
            string pName = context.PropertyDescriptor.Name;
            object rowObject = bag.SourceObject;
            string name = ModelHelper.GetPropertyNameForDescription(bag.SourceObjectType, pName);
            if (name == null) name = pName;
            bool cancelEdit = false;
            return AppManager.Instance.EditCustomPropertyValue(rowObject, name, out cancelEdit, null);
        }
    }
    
    //https://rsdn.org/article/dotnet/PropertyGridFAQ.xml
    class BooleanTypeConverter : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destType)
        {
            return (bool)value ?
              FrwCRUDRes.Present : FrwCRUDRes.Not_present;
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == FrwCRUDRes.Present;
        }
    }



}
