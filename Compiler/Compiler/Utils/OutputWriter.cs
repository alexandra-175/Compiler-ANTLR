using Compiler.Models;
using Compiler.Semantic;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Utils
{
    public static class OutputWriter
    {
        public static void WriteGlobalVariables(SymbolTable globalScope)
        {
            using var sw = new StreamWriter("variabile_globale.txt");
            foreach (var v in globalScope.GetAllSymbols())
                sw.WriteLine($"{v.Type} {v.Name} = {v.Value} (Linia {v.Line})");
        }

        public static void WriteFunctions(List<FunctionInfo> functions)
        {
            using var sw = new StreamWriter("functii.txt");
            foreach (var f in functions)
            {
                sw.WriteLine($"Functie: {f.Name}");
                sw.WriteLine($"Return: {f.ReturnType}");
                sw.WriteLine($"Tip: {(f.IsMain ? "main" : "non-main")}, {(f.IsRecursive ? "recursiva" : "iterativa")}");
                sw.WriteLine($"Parametri: {string.Join(", ", f.Parameters.Select(p => p.Type + " " + p.Name))}");
                sw.WriteLine($"Locale: {string.Join(", ", f.LocalVariables.Select(v => v.Name))}");
                sw.WriteLine($"Control: {string.Join(", ", f.ControlStructures.Select(c => c.Type))}");
                sw.WriteLine();
            }
        }

        public static void WriteErrors(List<SemanticError> errors)
        {
            using var sw = new StreamWriter("erori.txt");
            foreach (var e in errors)
                sw.WriteLine(e.ToString());
        }
    }
}
