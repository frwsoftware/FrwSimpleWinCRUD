using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{

    [JEntity]
    public class TestDto2
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JOneToMany("TestDto2")]
        public IList<TestDto1> TestDto1s { get; set; }
        [JOneToMany("TestDto2_1")]
        public IList<TestDto1> TestDto1_1s { get; set; }

    }
}
