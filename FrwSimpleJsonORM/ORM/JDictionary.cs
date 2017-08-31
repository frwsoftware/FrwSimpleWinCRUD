using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class JDictionary
    {
        private List<JDictItem> items = new List<JDictItem>();

        public string Id { get; set; }

        public List<JDictItem> Items
        {
            get
            {
                return items;
            }

        }

        public string Name { get; set; }
    }
}
