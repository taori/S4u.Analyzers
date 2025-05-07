using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace S4u.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class S4UL0001_Analyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rules.Usage.LogStatementIsNotLogged);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
    }

    private static readonly ImmutableHashSet<string> CalledMemberCandidates =
        ImmutableHashSet.Create("Debug", "Information", "Warning", "Error", "Fatal", "Exception");
    private static readonly ImmutableHashSet<string> ExpressionCandidates =
        ImmutableHashSet.Create("Arc4u.Diagnostics.CommonMessageLogger");

    /// <summary>
    /// Executed on the completion of the semantic analysis associated with the Invocation operation.
    /// </summary>
    /// <param name="context">Operation context.</param>
    private void AnalyzeOperation(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationOperation invocationOperation ||
            context.Operation.Syntax is not InvocationExpressionSyntax invocationSyntax)
            return;

        if (!CalledMemberCandidates.Contains(invocationOperation.TargetMethod.MetadataName))
            return;
        // If something on the parent expression is called it might be Log() and probably is, so we just skip checking further
        if (invocationOperation.Parent?.Type is not null)
            return;
        var instanceCandidates = ExpressionCandidates
            .Select(d => context.Compilation.GetTypeByMetadataName(d))
            .ToImmutableHashSet(SymbolEqualityComparer.Default);
        if (invocationOperation.Instance?.Type is not { } instanceType)
            return;
        if (!instanceCandidates.Contains(instanceType))
            return;

        if (invocationSyntax.Expression is MemberAccessExpressionSyntax maes)
        {
            var diagnostic = Diagnostic.Create(Rules.Usage.LogStatementIsNotLogged,
                maes.Name.GetLocation(),
                maes.Name.Identifier.ValueText
            );

            context.ReportDiagnostic(diagnostic);
        }
    }
}