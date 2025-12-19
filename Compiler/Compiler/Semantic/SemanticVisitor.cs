using Compiler.Models;
using Antlr4.Runtime.Misc;
using System.Linq;

namespace Compiler.Semantic
{
    public class SemanticVisitor : MiniLanguageBaseVisitor<object?>
    {
        public SemanticContext Context { get; } = new SemanticContext();
        private FunctionInfo? _currentFunction = null;

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

            if (context.paramList() != null)
            {
                foreach (var p in context.paramList().param())
                {
                    _currentFunction.Parameters.Add(new VariableInfo
                    {
                        Name = p.ID().GetText(),
                        Type = p.type().GetText(),
                        Line = p.Start.Line
                    });
                }
            }

            Context.Functions.Add(_currentFunction);
            base.VisitFuncDecl(context);
            _currentFunction = null;
            return null;
        }

        public override object? VisitVarDeclaration([NotNull] MiniLanguageParser.VarDeclarationContext context)
        {
            var decl = context.varDecl();

            var varInfo = new VariableInfo
            {
                Name = decl.ID().GetText(),
                Type = decl.type().GetText(),
                Value = decl.expression()?.GetText() ?? "",
                Line = decl.Start.Line,
                IsConst = decl.type().GetText().StartsWith("const")
            };

            if (_currentFunction == null)
            {
                if (!Context.GlobalScope.Declare(varInfo))
                    AddError(varInfo.Line, $"Variabilă globală duplicată: {varInfo.Name}");
            }
            else
            {
                if (_currentFunction.LocalVariables.Any(v => v.Name == varInfo.Name))
                    AddError(varInfo.Line, $"Variabilă locală duplicată: {varInfo.Name}");
                _currentFunction.LocalVariables.Add(varInfo);
            }

            return null;
        }


        public override object? VisitFuncCallExpr([NotNull] MiniLanguageParser.FuncCallExprContext context)
        {
            if (_currentFunction != null && context.ID().GetText() == _currentFunction.Name)
                _currentFunction.IsRecursive = true;
            return base.VisitFuncCallExpr(context);
        }

        public override object? VisitIfStat([NotNull] MiniLanguageParser.IfStatContext context)
        {
            _currentFunction?.ControlStructures.Add(
                new ControlStructureInfo { Type = "if", Line = context.Start.Line }
            );
            return base.VisitIfStat(context);
        }

        public override object? VisitWhileStat([NotNull] MiniLanguageParser.WhileStatContext context)
        {
            _currentFunction?.ControlStructures.Add(
                new ControlStructureInfo { Type = "while", Line = context.Start.Line }
            );
            return base.VisitWhileStat(context);
        }

        public override object? VisitForStat([NotNull] MiniLanguageParser.ForStatContext context)
        {
            _currentFunction?.ControlStructures.Add(
                new ControlStructureInfo { Type = "for", Line = context.Start.Line }
            );
            return base.VisitForStat(context);
        }

        private void AddError(int line, string msg)
        {
            Context.Errors.Add(new SemanticError { Line = line, Message = msg });
        }
    }
}
