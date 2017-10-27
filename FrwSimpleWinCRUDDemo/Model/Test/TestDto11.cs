using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware.Model.Test
{
    [JEntity]
    public class TestDto11
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JOneToOne]
        public TestDto9 TestDto9 { get; set; }

    }
}
