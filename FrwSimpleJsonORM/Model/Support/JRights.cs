using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class JRights
    {
        public JRights()
        {

        }
        public JRights(bool updatable)
        {
            CanView = true;
            CanUpdate = updatable;
            CanDelete = updatable;
            CanDeleteAll = updatable;
            CanAdd = true;
            CanAddChild = updatable;
        }

        public bool CanView { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDeleteAll { get; set; }
        public bool CanAdd { get; set; }
        public bool CanAddChild { get; set; } //child
    }
}
