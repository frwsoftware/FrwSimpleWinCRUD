using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    [JDisplayName(typeof(FrwUtilsRes), "JPreferences")]
    abstract public class JPreferences
    {
        [JDisplayName(typeof(FrwUtilsRes), "JPreferences_Identifier")]
        [JPrimaryKey, JAutoIncrement]
        public string JPreferencesId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JPreferences_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }
    }
}
