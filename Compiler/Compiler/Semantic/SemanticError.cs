using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Semantic
{
    public class SemanticError
    {
        public int Line;
        public string Message;

        public override string ToString()
        {
            return $"Eroare semantică la linia {Line}: {Message}";
        }
    }
}
