using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace S4u.Analyzers.Tests.Environment;

public class CustomAnalyzerTest<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateSmartTest(string testCode)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = testCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Arc4u.Standard.Diagnostics", "8.1.0")]),
        };

        return test;

    }
}