
using System.Threading.Tasks;
using Logging.Test.Utils;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<S4u.Analyzers.S4UL0001_Analyzer,
        S4u.Analyzers.S4UL0001_CodeFixProvider>;

namespace S4u.Analyzers.Tests;

public class S4UL0001_CodeFixProviderTests : CustomCodeFixTest<S4u.Analyzers.S4UL0001_Analyzer, S4u.Analyzers.S4UL0001_CodeFixProvider>
{
    [Fact]
    public async Task VerifyRewrite()
    {
        /* lang=c#-test */
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
                logger.Technical().[|Information|]("asdf");
                logger.Technical().Information("asdf").Log();
                logger.Business().[|Information|]("asdf");
                logger.Business().Information("asdf").Log();
            }
        }
        """;

        /* lang=c#-test */
        const string fixedText = """
         using System;
         using Arc4u.Diagnostics;
         using Microsoft.Extensions.Logging;
         
         namespace S4u.Analyzers.Sample;
         
         public class Logging
         {
             public Logging()
             {
                 ILogger<Logging> logger = new LoggerFactory().CreateLogger<Logging>();
                 logger.Technical().LogInformation("asdf");
                 logger.Technical().Information("asdf").Log();
                 logger.Business().[|Information|]("asdf");
                 logger.Business().Information("asdf").Log();
             }
         }
         """;
        
        /* lang=c#-test */
        const string batchfixedText = """
         using System;
         using Arc4u.Diagnostics;
         using Microsoft.Extensions.Logging;
         
         namespace S4u.Analyzers.Sample;
         
         public class Logging
         {
             public Logging()
             {
                 ILogger<Logging> logger = new LoggerFactory().CreateLogger<Logging>();
                 logger.Technical().LogInformation("asdf");
                 logger.Technical().Information("asdf").Log();
                 logger.Business().LogInformation("asdf");
                 logger.Business().Information("asdf").Log();
             }
         }
         """;

        
        var test = CreateCodeFix(text, fixedText, batchfixedText, 0);
        test.NumberOfIncrementalIterations = 1;
        test.NumberOfFixAllIterations = 1;
        await test.RunAsync();
    }
}