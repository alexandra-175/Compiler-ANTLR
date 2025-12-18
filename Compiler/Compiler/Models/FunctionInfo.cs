using System.Collections.Generic;

namespace Compiler.Models
{
    public class FunctionInfo
    {
        public string Name { get; set; } = "";
        public string ReturnType { get; set; } = "";
        public int Line { get; set; }
        public bool IsMain { get; set; }
        public bool IsRecursive { get; set; }

        public List<VariableInfo> Parameters { get; set; } = new();
        public List<VariableInfo> LocalVariables { get; set; } = new();
        public List<ControlStructureInfo> ControlStructures { get; set; } = new();
    }
}