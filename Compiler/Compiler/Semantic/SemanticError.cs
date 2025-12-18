namespace Compiler.Semantic
{
    public class SemanticError
    {
        public int Line { get; set; }
        public string Message { get; set; } = "";

        public override string ToString()
        {
            return $"Eroare semantică la linia {Line}: {Message}";
        }
    }
}