using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    //for using in combobox
    public class CustomItem
    {
        public string Text { get; set; }
        public string Key { get; set; }

        public CustomItem(string key, string text)
        {
            Key = key; Text = text;
        }
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Text;
        }

    }
}
