using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware.Model.Test
{
    [JEntity]
    public class TestDto9
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JOneToOne]
        public TestDto10 TestDto10 { get; set; }
        [JOneToOne]
        public TestDto11 TestDto11 { get; set; }

    }
}
