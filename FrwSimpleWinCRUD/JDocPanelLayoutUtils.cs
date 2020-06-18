using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{

    [JEntityPlugin(typeof(JDocPanelLayout))]
    public class JVPNServerBasePlugin : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            JDocPanelLayout item = selectedObject as JDocPanelLayout;
            JDocPanelLayoutUtils.MakeContextMenuBlock(menuItemList, item, list.ContentContainer);
        }
    }

    public class JDocPanelLayoutUtils
    {
        /////////////////
        static public void MakeContextMenuBlock(List<ToolStripItem> menuItemList, object selectedObject, IContentContainer container)
        {
            menuItemList.Add(new ToolStripSeparator());
            ToolStripMenuItem menuItem = null;
            JDocPanelLayout item = (JDocPanelLayout)selectedObject;
            if (item != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.Save_current_widget_placement_to_this_entry;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        AppManager.Instance.SaveLayout(item);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.Download_widget_placement_from_this_entry;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        AppManager.Instance.LoadLayout(item);
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
