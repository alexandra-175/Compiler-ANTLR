using Compiler.Models;
using Compiler.Semantic;
using Compiler.Models;
using Compiler.Semantic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Utils
{
    public static class OutputWriter
    {
        public static void WriteGlobalVariables(SymbolTable globalScope)
        {
            using var sw = new StreamWriter("Output/global_vars.txt");
            // se va completa după integrarea cu visitor
        }

        public static void WriteFunctions(List<FunctionInfo> functions)
        {
            using var sw = new StreamWriter("Output/functions.txt");
            foreach (var f in functions)
            {
                sw.WriteLine($"Funcție: {f.ReturnType} {f.Name}");
                sw.WriteLine($"Tip: {(f.IsMain ? "main" : "non-main")}, {(f.IsRecursive ? "recursivă" : "iterativă")}");
                sw.WriteLine();
            }
        }

        public static void WriteErrors(List<SemanticError> errors)
        {
            using var sw = new StreamWriter("Output/errors.txt");
            foreach (var e in errors)
                sw.WriteLine(e.ToString());
        }
    }
}

