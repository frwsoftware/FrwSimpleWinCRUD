using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware.Model.Example
{
    [JDisplayName("Dto 2")]
    [JEntity]
    public class JExampleDto2
    {
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string JExampleDto2Id { get; set; }

        [JDisplayName("Name")]
        [JNameProperty]
        public string Name { get; set; }

        //one to many
        [JDisplayName("Dto1 list")]
        [JOneToMany]
        public IList<JExampleDto1> Dto1s { get; set; }

    }
}
