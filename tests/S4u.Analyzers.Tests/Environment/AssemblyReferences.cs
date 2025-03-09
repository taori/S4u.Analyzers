using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace S4u.Analyzers.Tests.Environment;

internal static class AssemblyReferences
{
    public static MetadataReference[] GetReferences => [
        MetadataReference.CreateFromFile(typeof(Arc4u.Diagnostics.LoggerMessage).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ILogger<>).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(LoggerFactory).Assembly.Location),
    ];
}