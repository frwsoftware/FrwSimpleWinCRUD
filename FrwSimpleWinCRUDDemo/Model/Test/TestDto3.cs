﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{

    [JEntity]
    public class TestDto3
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JManyToMany]
        public IList<TestDto1> TestDto1s { get; set; }

    }
}

