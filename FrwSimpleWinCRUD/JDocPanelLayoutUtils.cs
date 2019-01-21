using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    public class JDocPanelLayoutUtils
    {
        /////////////////
        static public void MakeContextMenuBlock(List<ToolStripItem> menuItemList, object selectedObject, IContentContainer container)
        {
            menuItemList.Add(new ToolStripSeparator());
            //обработчик вызова контекстного меню 
            ToolStripMenuItem menuItem = null;
            JDocPanelLayout item = (JDocPanelLayout)selectedObject;
            if (item != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = "Сохранить текущее размещение виджетов в эту запись";
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
                menuItem.Text = "Загрузить  размещение виджетов из этой записи";
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
