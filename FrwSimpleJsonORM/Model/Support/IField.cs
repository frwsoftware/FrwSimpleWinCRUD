using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public interface IField
    {
        string Name { get; }
        string Value { get; set; }
        string FieldSysname { get; }
    }
}
