using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    /// <summary>
    /// http://ua.theafh.net/
    /// </summary>
    [JDisplayName(typeof(FrwUtilsRes), "JUserAgent")]
    [JEntity]
    public class JUserAgent
    {
        [JDisplayName(typeof(FrwUtilsRes), "JUserAgent_JUserAgentId")]
        [JPrimaryKey, JAutoIncrement]
        public string JUserAgentId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JUserAgent_Name")]
        [JNameProperty, JReadOnly]
        public string Name {
            get
            {
                if (Alias != null) return Alias;
                else return Data;
            }
        }

        [JDisplayName(typeof(FrwUtilsRes), "JUserAgent_Alias")]
        [JNameProperty, JUnique]
        public string Alias { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JUserAgent_Data")]
        [JRequired]
        public string Data { get; set; }
    }
}
