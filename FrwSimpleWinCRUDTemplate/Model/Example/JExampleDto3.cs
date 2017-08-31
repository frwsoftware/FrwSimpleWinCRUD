using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware;

namespace FrwSoftware.Model.Example
{
    [JDisplayName("Dto 3")]
    [JEntity]
    public class JExampleDto3
    {
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string JExampleDto3Id { get; set; }

        [JDisplayName("Name")]
        [JNameProperty]
        public string Name { get; set; }

        //many to many 
        [JDisplayName("Dto1 list")]
        [JManyToMany]
        public IList<JExampleDto1> Dto1s { get; set; }

    }
}