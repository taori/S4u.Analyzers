using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing;
using S4u.Analyzers.Tests.Environment;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        S4u.Analyzers.LoggingAnalyzer>;

namespace S4u.Analyzers.Tests;

public class LoggingAnalyzerTests : CustomAnalyzerTest<S4u.Analyzers.LoggingAnalyzer>
{
    [Fact]
    public async Task DetectsWithExclusionOfLoggedMethods()
    {
        const string text = """
        using System;
        using Arc4u.Diagnostics;
        using Microsoft.Extensions.Logging;

        namespace S4u.Analyzers.Sample;

        public class Logging
        {
            public Logging()
            {
                ILogger<Logging> logger = new LoggerFactory().CreateLogger<Logging>();
                logger.Technical().[|Debug|]("asdf");
                logger.Technical().[|Error|]("asdf");
                logger.Technical().[|Exception|](new Exception("asdf"));
                logger.Technical().[|Warning|]("asdf");
                logger.Technical().[|Fatal|]("asdf");
                logger.Technical().[|Information|]("asdf");
                logger.Technical().Information("asdf").Log();
                
                logger.Business().[|Debug|]("asdf");
                logger.Business().[|Error|]("asdf");
                logger.Business().[|Exception|](new Exception("asdf"));
                logger.Business().[|Warning|]("asdf");
                logger.Business().[|Fatal|]("asdf");
                logger.Business().[|Information|]("asdf");
                logger.Business().Information("asdf").Log();
            }
        }

        """;

        var test2 = CreateSmartTest(text);
        await test2.RunAsync();
    }
}