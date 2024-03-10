﻿using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Purview.Telemetry.Logging;
using Purview.Telemetry.SourceGenerator.Targets;

namespace Purview.Telemetry.SourceGenerator.Helpers;

static class SharedHelpers {
	static public LogEntryAttributeRecord? GetLogEntryAttribute(
		ISymbol symbol,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token
	) {
		if (!Utilities.TryContainsAttribute(symbol, Constants.Logging.LogEntryAttribute, token, out var attributeData)) {
			return null;
		}

		AttributeValue<LogGeneratedLevel>? level = null;
		AttributeStringValue? messageTemplate = null;
		AttributeValue<int>? eventId = null;
		AttributeStringValue? nameValue = null;

		if (!AttributeParser(attributeData!,
		(name, value) => {
			if (name.Equals(nameof(LogEntryAttribute.Level), StringComparison.OrdinalIgnoreCase)) {
				level = new((LogGeneratedLevel)value);
			}
			else if (name.Equals(nameof(LogEntryAttribute.MessageTemplate), StringComparison.OrdinalIgnoreCase)) {
				messageTemplate = new((string)value);
			}
			else if (name.Equals(nameof(LogEntryAttribute.EventId), StringComparison.OrdinalIgnoreCase)) {
				eventId = new((int)value);
			}
			else if (name.Equals(nameof(LogEntryAttribute.EntryName), StringComparison.OrdinalIgnoreCase)) {
				nameValue = new((string)value);
			}
		}, semanticModel, logger, token)) {
			// Failed to parse correctly, so null it out.
			return null;
		}

		return new(
			Level: level ?? new(),
			MessageTemplate: messageTemplate ?? new(),
			EventId: eventId ?? new(),
			Name: nameValue ?? new()
		);
	}

	static public LoggerTargetAttributeRecord? GetLoggerTargetAttribute(
		AttributeData attributeData,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token) {

		AttributeValue<LogGeneratedLevel>? defaultLevel = null;
		AttributeStringValue? className = null;
		AttributeStringValue? customPrefix = null;
		AttributeValue<LogPrefixType>? prefixType = null;

		if (!AttributeParser(attributeData,
		(name, value) => {
			if (name.Equals(nameof(LoggerTargetAttribute.DefaultLevel), StringComparison.OrdinalIgnoreCase)) {
				defaultLevel = new((LogGeneratedLevel)value);
			}
			else if (name.Equals(nameof(LoggerTargetAttribute.ClassName), StringComparison.OrdinalIgnoreCase)) {
				className = new((string)value);
			}
			else if (name.Equals(nameof(LoggerTargetAttribute.CustomPrefix), StringComparison.OrdinalIgnoreCase)) {
				customPrefix = new((string)value);
			}
			else if (name.Equals(nameof(LoggerTargetAttribute.PrefixType), StringComparison.OrdinalIgnoreCase)) {
				prefixType = new((LogPrefixType)value);
			}
		}, semanticModel, logger, token)) {
			// Failed to parse correctly, so null it out.
			return null;
		}

		return new(
			DefaultLevel: defaultLevel ?? new(),
			ClassName: className ?? new(),
			CustomPrefix: customPrefix ?? new(),
			PrefixType: prefixType ?? new()
		);
	}

	static public LoggerDefaultsAttributeRecord? GetLoggerDefaultsAttribute(
		AttributeData attributeData,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token) {

		AttributeValue<LogGeneratedLevel>? defaultLevel = null;

		if (!AttributeParser(attributeData,
		(name, value) => {
			if (name.Equals(nameof(LoggerDefaultsAttribute.DefaultLevel), StringComparison.OrdinalIgnoreCase)) {
				defaultLevel = new((LogGeneratedLevel)value);
			}
		}, semanticModel, logger, token)) {
			// Failed to parse correctly, so null it out.
			return null;
		}

		return new(
			DefaultLevel: defaultLevel ?? new()
		);
	}

	static public bool AttributeParser(
		AttributeData attributeData,
		Action<string, object> namedArguments,
		SemanticModel? semanticModel,
		IGenerationLogger? logger,
		CancellationToken cancellationToken) {
		logger?.Debug($"Found attribute: {attributeData}");

		if (semanticModel != null && HasErrors(attributeData, semanticModel, logger, cancellationToken)) {
			logger?.Warning($"Attribute has error: {attributeData}");
			return false;
		}

		var constructorMethod = attributeData.AttributeConstructor;
		if (constructorMethod == null) {
			logger?.Warning("Could not locate the attribute's constructor.");

			return false;
		}

		if (attributeData.ConstructorArguments.Any(t => t.Kind == TypedConstantKind.Error)) {
			logger?.Warning("Constructor arguments have an error.");

			return false;
		}

		if (attributeData.NamedArguments.Any(t => t.Value.Kind == TypedConstantKind.Error)) {
			logger?.Warning("Named arguments have an error.");

			return false;
		}

		// supports: [DefaultLogLevel(LogGeneratedLevel.Information)]
		// supports: [DefaultLogLevel(level: LogGeneratedLevel.Information)]
		var items = attributeData.ConstructorArguments;
		if (items.Length > 0) {
			for (var i = 0; i < items.Length; i++) {
				cancellationToken.ThrowIfCancellationRequested();

				if (items[i].IsNull) {
					continue;
				}

				var name = constructorMethod.Parameters[i].Name;
				var value = Utilities.GetTypedConstantValue(items[i])!;
				if (Constants.System.String.Equals(constructorMethod.Parameters[i].Type)) {
					var v = (string)value;
					if (string.IsNullOrWhiteSpace(v)) {
						continue;
					}
				}

				namedArguments(name, value);
			}
		}

		// argument syntax takes parameters. e.g. Level = LogGeneratedLevel.Information
		// supports: e.g. [DefaultLogLevel(Level = LogGeneratedLevel.Information )]
		if (attributeData.NamedArguments.Any()) {
			foreach (var namedArgument in attributeData.NamedArguments) {
				cancellationToken.ThrowIfCancellationRequested();

				var value = Utilities.GetTypedConstantValue(namedArgument.Value)!;
				if (namedArgument.Value.Type == null) {
					logger?.Error($"Named argument {namedArgument.Key}'s type could not be determined.");
					continue;
				}

				if (Constants.System.String.Equals(namedArgument.Value.Type)) {
					var v = (string)value;
					if (string.IsNullOrWhiteSpace(v)) {
						continue;
					}
				}

				namedArguments(namedArgument.Key, value!);
			}
		}

		return true;
	}

	static public bool AttributeParser(
		AttributeSyntax attributeSyntax,
		Action<string, string> namedArguments,
		Action<string, OutputType>? logger,
		CancellationToken cancellationToken) {
		logger?.Invoke($"Found attribute (syntax): {attributeSyntax}", OutputType.Debug);

		var arguments = attributeSyntax.ArgumentList?.Arguments;
		if (arguments != null) {
			foreach (var argument in arguments) {
				cancellationToken.ThrowIfCancellationRequested();
				var name = argument.NameEquals?.Name.ToString() ?? argument.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault()?.ToString();
				var value = argument.Expression.ToString();
				if (name == null || value == null) {
					continue;
				}

				namedArguments(name!, value);
			}
		}

		return true;
	}

	static bool HasErrors(AttributeData attributeData,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken cancellationToken) {
		if (attributeData.ApplicationSyntaxReference?.GetSyntax(cancellationToken) is not AttributeSyntax attributeSyntax) {
			return false;
		}

		var diagnostics = semanticModel.GetDiagnostics(attributeSyntax.Span, cancellationToken);
		if (diagnostics.Length > 0 && logger != null) {
			var d = diagnostics.Select(m => m.GetMessage(CultureInfo.InvariantCulture));
			logger.Debug("Attribute has diagnostics: \n" + string.Join("\n - ", d));
		}

		return diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
	}
}
