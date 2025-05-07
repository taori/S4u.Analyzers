using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace S4u.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LoggingCodeFixProvider)), Shared]
public class LoggingCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Rules.Usage.LogStatementIsNotLogged.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find SyntaxNode corresponding to the diagnostic.
            var diagnosticNode = root?.FindNode(diagnosticSpan);

            // To get the required metadata, we should match the Node to the specific type: 'ClassDeclarationSyntax'.
            if (diagnosticNode is not IdentifierNameSyntax declaration)
                continue;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Resources.S4U0001Title,
                    createChangedDocument: token => PrependLog(context.Document, declaration, token),
                    equivalenceKey: $"{Rules.Usage.LogStatementIsNotLogged.Id}{declaration.Identifier.ValueText}"
                ),
                diagnostic
            );
        }
    }

    private async Task<Document> PrependLog(Document document, IdentifierNameSyntax declaration, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root is null)
            return document;

        var replacedRoot = root.ReplaceNode(declaration, SyntaxFactory.IdentifierName($"Log{declaration.Identifier.ValueText}"));
        return document.WithSyntaxRoot(replacedRoot);
    }
}