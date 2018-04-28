using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrwSoftware
{
    [JDisplayName("Базовая категория")]
    //[JEntity]
    public class JBaseCategory
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JBaseCategoryId { get; set; }

        [JDisplayName("Наименование")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }
        
        /*
        [JDisplayName("Родительская категория")]
        [JManyToOne]
        public JBaseCategory Parent { get; set; }
        */
    }
}
