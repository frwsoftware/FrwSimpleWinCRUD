using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{

    [JEntity]
    public class TestDto7
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JManyToOne]
        public TestDto1 TestDto1 { get; set; }

    }
}
