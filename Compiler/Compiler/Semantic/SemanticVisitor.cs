using Compiler.Models;
using Antlr4.Runtime.Misc;
using System.Linq;

namespace Compiler.Semantic
{
    public class SemanticVisitor : MiniLanguageBaseVisitor<object?>
    {
        public SemanticContext Context { get; } = new SemanticContext();
        private FunctionInfo? currentFunction = null;

        public override object? VisitVarDeclaration([NotNull] MiniLanguageParser.VarDeclarationContext context)
        {
            var decl = context.varDecl();
            var variable = new VariableInfo
            {
                Name = decl.ID().GetText(),
                Type = decl.type().GetText(),
                Line = decl.Start.Line,
                Value = decl.expression()?.GetText() ?? "null",
                IsConst = decl.type().GetText().StartsWith("const")
            };

            if (variable.IsConst && variable.Value == "null")
                AddError(variable.Line, $"Variabila const '{variable.Name}' trebuie inițializată.");

            if (currentFunction == null)
            {
                if (!Context.GlobalScope.Declare(variable))
                    AddError(variable.Line, $"Variabila globală '{variable.Name}' duplicată.");
            }
            else
            {
                if (currentFunction.LocalVariables.Any(v => v.Name == variable.Name))
                    AddError(variable.Line, $"Variabila locală '{variable.Name}' duplicată.");
                currentFunction.LocalVariables.Add(variable);
            }

            return base.VisitVarDeclaration(context);
        }

        public override object? VisitFuncDecl([NotNull] MiniLanguageParser.FuncDeclContext context)
        {
            var func = new FunctionInfo
            {
                Name = context.ID().GetText(),
                ReturnType = context.type()?.GetText() ?? "void",
                Line = context.Start.Line,
                IsMain = context.ID().GetText() == "main"
            };

            if (func.IsMain && Context.Functions.Any(f => f.IsMain))
                AddError(func.Line, "Există deja o funcție main.");

            Context.Functions.Add(func);
            currentFunction = func;

            var result = base.VisitFuncDecl(context);

            if (func.ReturnType != "void" && !func.HasReturn)
                AddError(func.Line, $"Funcția '{func.Name}' trebuie să aibă return.");

            currentFunction = null;
            return result;
        }

        public override object? VisitParam([NotNull] MiniLanguageParser.ParamContext context)
        {
            var param = new VariableInfo
            {
                Name = context.ID().GetText(),
                Type = context.type().GetText(),
                Line = context.Start.Line
            };

            currentFunction?.Parameters.Add(param);
            return null;
        }

        public override object? VisitReturnStat([NotNull] MiniLanguageParser.ReturnStatContext context)
        {
            if (currentFunction != null)
            {
                currentFunction.HasReturn = true;
                if (currentFunction.ReturnType != "void" && context.expression() == null)
                    AddError(context.Start.Line, $"Funcția '{currentFunction.Name}' trebuie să returneze o valoare.");
            }
            return null;
        }

        public override object? VisitIdentifierExpr([NotNull] MiniLanguageParser.IdentifierExprContext context)
        {
            string name = context.ID().GetText();
            bool declared =
                (currentFunction != null && currentFunction.LocalVariables.Any(v => v.Name == name)) ||
                Context.GlobalScope.Lookup(name) != null;

            if (!declared)
                AddError(context.Start.Line, $"Variabila '{name}' nu a fost declarată.");

            return base.VisitIdentifierExpr(context);
        }

        public override object? VisitFuncCallExpr([NotNull] MiniLanguageParser.FuncCallExprContext context)
        {
            string name = context.ID().GetText();
            var func = Context.Functions.FirstOrDefault(f => f.Name == name);

            if (func == null)
                AddError(context.Start.Line, $"Funcția '{name}' nu este declarată.");
            else
            {
                int expected = func.Parameters.Count;
                int received = context.argList()?.expression().Length ?? 0;
                if (expected != received)
                    AddError(context.Start.Line, $"Funcția '{name}' așteaptă {expected} parametri, dar a primit {received}.");
            }

            if (currentFunction != null && currentFunction.Name == name)
                currentFunction.IsRecursive = true;

            return base.VisitFuncCallExpr(context);
        }

        public override object? VisitIfStat([NotNull] MiniLanguageParser.IfStatContext context)
        {
            currentFunction?.ControlStructures.Add(new ControlStructureInfo { Type = "if", Line = context.Start.Line });
            return base.VisitIfStat(context);
        }

        public override object? VisitForStat([NotNull] MiniLanguageParser.ForStatContext context)
        {
            currentFunction?.ControlStructures.Add(new ControlStructureInfo { Type = "for", Line = context.Start.Line });
            return base.VisitForStat(context);
        }

        public override object? VisitWhileStat([NotNull] MiniLanguageParser.WhileStatContext context)
        {
            currentFunction?.ControlStructures.Add(new ControlStructureInfo { Type = "while", Line = context.Start.Line });
            return base.VisitWhileStat(context);
        }

        private void AddError(int line, string message)
        {
            Context.Errors.Add(new SemanticError { Line = line, Message = message });
        }
    }
}
