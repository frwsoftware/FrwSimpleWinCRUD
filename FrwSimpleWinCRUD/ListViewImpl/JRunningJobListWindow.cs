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
using System.IO;

namespace FrwSoftware
{
    public partial class JRunningJobListWindow : SimpleListWindow
    {
        public JRunningJobListWindow()
        {
            InitializeComponent();
        }
        
        override protected void MakeListColumns()
        {
            base.MakeListColumns();
            StartRefreshing();//todo
        }
  
        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
            menuItemList.Add(new ToolStripSeparator());

            JRunningJob job = (JRunningJob)selectedObject;
            if (job != null)
            {
                JobManager.MakeContextMenuForRunningJob(job, menuItemList, this.ContentContainer);
            }

        }
    }
}
