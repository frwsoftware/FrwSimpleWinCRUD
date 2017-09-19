using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FrwSoftware
{
    public class WebEntryInfo
    {
        public string Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }


        static public WebEntryInfo GetWebEntryInfoFromObject(object o)
        {
            if (o == null) return null;
            PropertyInfo p = o.GetType().GetProperties().Where(
                prop => prop.PropertyType == typeof(WebEntryInfo)).FirstOrDefault();
            if (p == null) return null;
            return p.GetValue(o) as WebEntryInfo;
        }
    }
}
