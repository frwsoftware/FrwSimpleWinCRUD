using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FrwSoftware
{
    public enum InfoHeaderEnum
    {
        C, D, F, H
    }


    [JDisplayName("Справочная запись")]
    [JEntity(ImageName = "fr_project", Resource = typeof(Properties.Resources))]//todo
    public class JInfoHeader
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JInfoHeaderId { get; set; }

        [JDisplayName("Наименование")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName("Родительская запись")]
        [JManyToOne]
        public JInfoHeader Parent { get; set; }

        [JDisplayName("Url")]
        public string Url { get; set; }
        [JDisplayName("Url")]
        public string UrlExt { get; set; }

        [JDisplayName("Тип")]
        [JDictProp(DictNames.InfoHeaderType, false, DisplyPropertyStyle.TextAndImage)]
        public string Type { get; set; }

        [JDisplayName("Дата модификации")]
        [JReadOnly]
        public DateTime DateModified { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo() { Url = Url };
                return w;
            }
        }

        [JIgnore, JsonIgnore]
        public int LevelInFile { get; set; }

    }
}
