using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware.Model.Example
{
    [JDisplayName("dto 2")]
    [JEntity]
    public class JExampleDto2
    {
        //primary key with autoincrement 
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string JExampleDto2Id { get; set; }

        //simple field
        [JDisplayName("Field 1")]
        public string Field1 { get; set; }

        //many to one
        [JDisplayName("Example")]
        [JManyToOne]
        public JExampleDto JExampleDto { get; set; }

        //name field 
        [JDisplayName("Field 3")]
        [JNameProperty, JRequired, JUnique]
        public string Field3 { get; set; }
    }
}
