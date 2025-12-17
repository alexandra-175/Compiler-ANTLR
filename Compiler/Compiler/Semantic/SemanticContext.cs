using Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Semantic
{
    public class SemanticContext
    {
        public SymbolTable GlobalScope = new();
        public List<FunctionInfo> Functions = new();
        public List<SemanticError> Errors = new();
    }
}
