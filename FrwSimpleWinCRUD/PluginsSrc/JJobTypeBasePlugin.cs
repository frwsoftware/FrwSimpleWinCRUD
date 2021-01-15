using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    [JEntityPlugin(typeof(JJobType))]
    public class JJobTypeBasePlugin : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            menuItemList.Add(new ToolStripSeparator());
            JJobType job = (JJobType)selectedObject;
            if (job != null)
            {
                JobManager.MakeContextMenuForRunningJobBatch(job, menuItemList, list.ContentContainer);
            }
        }
    }
}
