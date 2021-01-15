using FrwSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSimpleDemoPluginLib
{
    [JEntityPlugin(typeof(JCountry))]
    public class DemoEntityPluginForJCountry : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            JCountry item = selectedObject as JCountry;
            if (item != null)
            {

                ToolStripMenuItem menuItem = null;

                menuItem = new ToolStripMenuItem();
                menuItem.Text = "Demo menu item from DemoPluginForJCountry";
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        MessageBox.Show("Country: " + item.Name);
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
}
