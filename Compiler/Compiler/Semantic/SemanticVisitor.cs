using Compiler.Models;
using Antlr4.Runtime.Misc;
using System.Linq;

namespace Compiler.Semantic
{
    public class SemanticVisitor : MiniLanguageBaseVisitor<object?>
    {
        public SemanticContext Context { get; } = new SemanticContext();
        private FunctionInfo? _currentFunction = null;

        public override object? VisitVarDeclaration([NotNull] MiniLanguageParser.VarDeclarationContext context)
        {
            var varDecl = context.varDecl();
            var varInfo = new VariableInfo
            {
                Name = varDecl.ID().GetText(),
                Type = varDecl.type().GetText(),
                Line = varDecl.Start.Line,
                Value = varDecl.expression()?.GetText() ?? "null"
            };

            if (_currentFunction == null)
            {
                if (!Context.GlobalScope.Declare(varInfo))
                    AddError(varInfo.Line, $"Variabila globală '{varInfo.Name}' duplicată.");
            }
            else
            {
                if (_currentFunction.LocalVariables.Any(v => v.Name == varInfo.Name))
                    AddError(varInfo.Line, $"Variabila locală '{varInfo.Name}' duplicată în funcție.");
                _currentFunction.LocalVariables.Add(varInfo);
            }
            return base.VisitVarDeclaration(context);
        }

        public override object? VisitFuncDecl([NotNull] MiniLanguageParser.FuncDeclContext context)
        {
            string name = context.ID().GetText();
            _currentFunction = new FunctionInfo
            {
                Name = name,
                ReturnType = context.type()?.GetText() ?? context.VOID()?.GetText() ?? "void",
                IsMain = name == "main",
                Line = context.Start.Line
            };

            if (_currentFunction.IsMain && Context.Functions.Any(f => f.IsMain))
                AddError(context.Start.Line, "Există deja o funcție main.");

            Context.Functions.Add(_currentFunction);
            var result = base.VisitFuncDecl(context);
            _currentFunction = null;
            return result;
        }

        public override object? VisitIfStat([NotNull] MiniLanguageParser.IfStatContext context)
        {
            _currentFunction?.ControlStructures.Add(new ControlStructureInfo { Type = "if", Line = context.Start.Line });
            return base.VisitIfStat(context);
        }

        public override object? VisitWhileStat([NotNull] MiniLanguageParser.WhileStatContext context)
        {
            _currentFunction?.ControlStructures.Add(new ControlStructureInfo { Type = "while", Line = context.Start.Line });
            return base.VisitWhileStat(context);
        }

        public override object? VisitForStat([NotNull] MiniLanguageParser.ForStatContext context)
        {
            _currentFunction?.ControlStructures.Add(new ControlStructureInfo { Type = "for", Line = context.Start.Line });
            return base.VisitForStat(context);
        }

        private void AddError(int line, string msg) => Context.Errors.Add(new SemanticError { Line = line, Message = msg });
    }
}