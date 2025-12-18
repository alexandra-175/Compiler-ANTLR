using System.Collections.Generic;
using Compiler.Models;

namespace Compiler.Semantic
{
    public class SymbolTable
    {
        private Dictionary<string, VariableInfo> symbols = new();
        public SymbolTable? Parent { get; set; }
        public SymbolTable(SymbolTable? parent = null) => Parent = parent;

        public bool Declare(VariableInfo variable)
        {
            if (symbols.ContainsKey(variable.Name)) return false;
            symbols[variable.Name] = variable;
            return true;
        }

        public VariableInfo? Lookup(string name)
        {
            if (symbols.ContainsKey(name)) return symbols[name];
            return Parent?.Lookup(name);
        }

        public IEnumerable<VariableInfo> GetAllSymbols() => symbols.Values;
    }
}