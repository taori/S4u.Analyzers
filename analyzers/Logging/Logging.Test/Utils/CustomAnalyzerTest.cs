using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Logging.Test.Utils;

public class CustomAnalyzerTest<TAnalyzer>
	where TAnalyzer : DiagnosticAnalyzer, new()
{
	protected CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateSmartTest(string testCode)
	{
		var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
		{
			TestCode = testCode,
			ReferenceAssemblies = TestConfiguration.ReferenceAssemblies,
		};

		return test;

	}
}

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
			ReferenceAssemblies = TestConfiguration.ReferenceAssemblies,
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

internal static class TestConfiguration
{
	public static ReferenceAssemblies ReferenceAssemblies { get; } = ReferenceAssemblies.Net.Net80
		// .AddPackages([new PackageIdentity("SomePackageId", "8.1.0")])
		;
}