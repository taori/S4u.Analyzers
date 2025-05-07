using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace S4u.Analyzers.Tests.Environment;

public class CustomCodeFixTest<TAnalyzer, TCodeFixProvider>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFixProvider : CodeFixProvider, new()
{
    protected CSharpCodeFixTest<TAnalyzer, TCodeFixProvider, DefaultVerifier> CreateCodeFix(string testCode,
        string fixedCode, string batchFixedCode, int? fixIndex = null)
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFixProvider, DefaultVerifier>
        {
            CodeFixTestBehaviors = CodeFixTestBehaviors.FixOne,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Arc4u.Standard.Diagnostics", "8.1.0")]),
            TestCode = testCode,
            FixedState =
            {
                Sources =
                {
                    fixedCode
                },
                MarkupHandling = MarkupMode.Allow
            },
            BatchFixedCode = batchFixedCode,
            CodeActionIndex = fixIndex,
        };

        return test;
    }
}