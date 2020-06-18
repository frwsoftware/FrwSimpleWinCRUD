using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    public interface IFormsEntityPlugin : IEntityPlugin
    {
        void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName);
    }
}
