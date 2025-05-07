using System;
using System.Linq.Expressions;
using Logging.Analyzers;
using Microsoft.CodeAnalysis;

namespace S4u.Analyzers;

internal static class Rules
{
	internal static class Usage
	{
		internal static readonly DiagnosticDescriptor LogStatementIsNotLogged = RuleFrom("S4U0001", 
			nameof(Translations.S4U0001Title),
			nameof(Translations.S4U0001MessageFormat),
			nameof(Translations.S4U0001Description),
			DiagnosticSeverity.Warning, Categories.Usage, true);
	}

	internal static DiagnosticDescriptor RuleFrom(string id, string title, string messageFormat, string description, DiagnosticSeverity severity, string category, bool enabledByDefault, Action<DiagnosticDescriptor>? modification = null)
	{
		var rule = new DiagnosticDescriptor(id, GetLocalizedString(title), GetLocalizedString(messageFormat), category, severity, enabledByDefault, GetLocalizedString(description));
		modification?.Invoke(rule);
		return rule;
	}

	internal static LocalizableResourceString GetLocalizedString(string resourceKey)
	{
		return new LocalizableResourceString(resourceKey, Translations.ResourceManager, typeof(Translations));
	}
}