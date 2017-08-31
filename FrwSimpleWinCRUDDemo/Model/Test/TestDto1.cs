using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{

    [JEntity]
    public class TestDto1
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JManyToOne("TestDto1s")]
        public TestDto2 TestDto2 { get; set; }
        [JManyToOne("TestDto1_1s")]
        public TestDto2 TestDto2_1 { get; set; }

        [JManyToOne]
        public TestDto6 TestDto6 { get; set; }


        [JManyToOne]
        public TestDto1 ParentTestDto1 { get; set; }
        [JOneToMany]
        public IList<TestDto1> ChildTestDto1s { get; set; }

        [JManyToMany]
        public IList<TestDto3> TestDto3s { get; set; }

        [JManyToMany]
        public IList<TestDto4> TestDto4s { get; set; }


        [JOneToMany]
        public IList<TestDto8> TestDto8s { get; set; }
    }
}
