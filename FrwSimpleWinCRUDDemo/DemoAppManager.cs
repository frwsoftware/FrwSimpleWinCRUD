using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware.Model.Chinook;

namespace FrwSoftware
{
    public class DemoAppManager : AppManager
    {
        override protected BaseMainAppForm GetMainForm()
        {
            return new FrwMainAppForm();
        }
        override protected IListProcessor GetListWindowForType(Type type)
        {
            IListProcessor w = null;
            if (type == typeof(Invoice)) w = new InvoiceListWindow();
            //else if (type == typeof(JDocPanelLayout)) w = new JDocPanelLayoutListWindow();
            else w = base.GetListWindowForType(type);
            return w;
        }
        override protected IPropertyProcessor GetPropertyWindowForType(Type type)
        {
            return base.GetPropertyWindowForType(type);
        }

    }
}
