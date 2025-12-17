using Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Models
{
    public class FunctionInfo
    {
        public string Name;
        public string ReturnType;
        public bool IsMain;
        public bool IsRecursive;

        public List<VariableInfo> Parameters = new();
        public List<VariableInfo> LocalVariables = new();
        public List<ControlStructureInfo> ControlStructures = new();
    }
}

