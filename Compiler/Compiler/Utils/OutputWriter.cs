using Compiler.Models;
using Compiler.Semantic;
using System.Collections.Generic;
using System.IO;

namespace Compiler.Utils
{
    public static class OutputWriter
    {
        public static void WriteGlobalVariables(SymbolTable scope)
        {
            using var sw = new StreamWriter("variabile_globale.txt");
            foreach (var v in scope.GetAllSymbols())
                sw.WriteLine($"{v.Type} {v.Name} = {v.Value} (Linia {v.Line})");
        }

        public static void WriteFunctions(List<FunctionInfo> functions)
        {
            using var sw = new StreamWriter("functii.txt");
            foreach (var f in functions)
            {
                sw.WriteLine($"Funcție: {f.Name} [{f.ReturnType}] - Linia {f.Line}");
                sw.WriteLine($"Tip: {(f.IsMain ? "main" : "non-main")}, {(f.IsRecursive ? "recursivă" : "iterativă")}");

                sw.WriteLine("Parametri:");
                foreach (var p in f.Parameters)
                    sw.WriteLine($"  {p.Type} {p.Name}");

                sw.WriteLine("Variabile locale:");
                foreach (var v in f.LocalVariables)
                    sw.WriteLine($"  {v.Type} {v.Name} = {v.Value}");

                sw.WriteLine("Structuri de control:");
                foreach (var s in f.ControlStructures)
                    sw.WriteLine($"  {s.Type} (linia {s.Line})");

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
