using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    [JEntityPlugin(typeof(JRunningJob))]
    public class JRunningJobBasePlugin : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            menuItemList.Add(new ToolStripSeparator());
            JRunningJob job = (JRunningJob)selectedObject;
            if (job != null)
            {
                JobManager.MakeContextMenuForRunningJob(job, menuItemList, list.ContentContainer);
            }
        }
    }
}
