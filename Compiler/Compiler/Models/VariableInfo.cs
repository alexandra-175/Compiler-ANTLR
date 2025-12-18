using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Models
{
    public class VariableInfo
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";
        public int Line { get; set; }
        public bool IsConst { get; set; }
    }
}

