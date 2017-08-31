using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    [JDisplayName(typeof(FrwUtilsRes), "Attribute_Layout_Of_Screen_Widgets")]
    [JEntity]
    public class JDocPanelLayout
    {
        [JDisplayName(typeof(FrwUtilsRes), "Attribute_Identifier")]
        [JPrimaryKey, JAutoIncrement]
        public string JDocPanelLayoutId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "Attribute_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "Attribute_Xml")]
        [JIgnore]
        public string[] Layout { get; set; }

        [JIgnore]
        public string Containers { get; set; }
    }
}
