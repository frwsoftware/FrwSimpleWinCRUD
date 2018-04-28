using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    public partial class JJobTypeListWindow : SimpleListWindow
    {
        public JJobTypeListWindow()
        {
            InitializeComponent();
        }
        
        override protected void MakeListColumns()
        {
            base.MakeListColumns();
        }
  
        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
            menuItemList.Add(new ToolStripSeparator());
            JJobType job = (JJobType)selectedObject;
            if (job != null)
            {
                JobManager.MakeContextMenuForRunningJobBatch(job, menuItemList, this.ContentContainer);
            }

        }
    }
}
