using Antlr4.Runtime;
using Compiler.Semantic;
using System;
using System.IO;
using System.Linq;
using System.Text;

try
{
    string input = File.ReadAllText("input.txt");
    AntlrInputStream inputStream = new AntlrInputStream(input);
    MiniLanguageLexer lexer = new MiniLanguageLexer(inputStream);
    CommonTokenStream tokenStream = new CommonTokenStream(lexer);

    // 1. Unități Lexicale
    StringBuilder tokensLog = new StringBuilder();
    tokenStream.Fill();
    foreach (var t in tokenStream.GetTokens())
        if (t.Type != -1) tokensLog.AppendLine($"<{lexer.Vocabulary.GetSymbolicName(t.Type)}, {t.Text}, {t.Line}>");
    File.WriteAllText("tokens.txt", tokensLog.ToString());

    // 2. Parsare și Vizitare
    MiniLanguageParser parser = new MiniLanguageParser(tokenStream);
    SemanticVisitor visitor = new SemanticVisitor();
    visitor.Visit(parser.program());

    // 3. Export Date
    File.WriteAllLines("variabile_globale.txt", visitor.Context.GlobalScope.GetAllSymbols().Select(v => $"{v.Type} {v.Name} = {v.Value} (Linia {v.Line})"));

    using (StreamWriter sw = new StreamWriter("functii.txt"))
    {
        foreach (var f in visitor.Context.Functions)
        {
            sw.WriteLine($"Functie: {f.Name} [{f.ReturnType}] - Linia {f.Line}");
            sw.WriteLine($"  Locale: {string.Join(", ", f.LocalVariables.Select(v => v.Name))}");
            sw.WriteLine($"  Control: {string.Join(", ", f.ControlStructures.Select(s => s.Type))}");
        }
    }

    // 4. Erori
    File.WriteAllLines("erori.txt", visitor.Context.Errors.Select(e => e.ToString()));
    Console.WriteLine("Gata! Fișierele au fost generate.");
}
catch (Exception ex) { Console.WriteLine("Eroare: " + ex.Message); }