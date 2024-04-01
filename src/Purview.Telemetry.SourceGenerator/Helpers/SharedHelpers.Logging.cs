﻿using Microsoft.CodeAnalysis;
using Purview.Telemetry.SourceGenerator.Records;

namespace Purview.Telemetry.SourceGenerator.Helpers;

partial class SharedHelpers {
	static public LogAttributeRecord? GetLogAttribute(
		ISymbol symbol,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token
	) {
		if (!Utilities.TryContainsAttribute(symbol, Constants.Logging.LogAttribute, token, out var attributeData)) {
			return null;
		}

		AttributeValue<int>? level = null;
		AttributeStringValue? messageTemplate = null;
		AttributeValue<int>? eventId = null;
		AttributeStringValue? nameValue = null;

		if (!AttributeParser(attributeData!,
		(name, value) => {
			if (name.Equals("Level", StringComparison.OrdinalIgnoreCase)) {
				level = new((int)value);
			}
			else if (name.Equals("MessageTemplate", StringComparison.OrdinalIgnoreCase)) {
				messageTemplate = new((string)value);
			}
			else if (name.Equals("EventId", StringComparison.OrdinalIgnoreCase)) {
				eventId = new((int)value);
			}
			else if (name.Equals("Name", StringComparison.OrdinalIgnoreCase)) {
				nameValue = new((string)value);
			}
		}, semanticModel, logger, token)) {
			// Failed to parse correctly, so null it out.
			return null;
		}

		return new(
			Level: level ?? new(Constants.Logging.DefaultLevel),
			MessageTemplate: messageTemplate ?? new(),
			EventId: eventId ?? new(),
			Name: nameValue ?? new()
		);
	}

	static public LoggerAttributeRecord? GetLoggerAttribute(
		AttributeData attributeData,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token) {

		AttributeValue<int>? defaultLevel = null;
		AttributeStringValue? customPrefix = null;
		AttributeValue<int>? prefixType = null;

		if (!AttributeParser(attributeData,
		(name, value) => {
			if (name.Equals("DefaultLevel", StringComparison.OrdinalIgnoreCase)) {
				defaultLevel = new((int)value);
			}
			else if (name.Equals("CustomPrefix", StringComparison.OrdinalIgnoreCase)) {
				customPrefix = new((string)value);
			}
			else if (name.Equals("PrefixType", StringComparison.OrdinalIgnoreCase)) {
				prefixType = new((int)value);
			}
		}, semanticModel, logger, token)) {
			// Failed to parse correctly, so null it out.
			return null;
		}

		return new(
			DefaultLevel: defaultLevel ?? new(),
			CustomPrefix: customPrefix ?? new(),
			PrefixType: prefixType ?? new()
		);
	}

	static public LoggerGenerationAttributeRecord? GetLoggerGenerationAttribute(
		AttributeData attributeData,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token) {

		AttributeValue<int>? defaultLevel = null;

		if (!AttributeParser(attributeData,
		(name, value) => {
			if (name.Equals("DefaultLevel", StringComparison.OrdinalIgnoreCase)) {
				defaultLevel = new((int)value);
			}
		}, semanticModel, logger, token)) {
			// Failed to parse correctly, so null it out.
			return null;
		}

		return new(
			DefaultLevel: defaultLevel ?? new()
		);
	}

	static public LoggerGenerationAttributeRecord? GetLoggerDefaultsAttribute(SemanticModel semanticModel, IGenerationLogger? logger, CancellationToken token) {
		token.ThrowIfCancellationRequested();

		if (!Utilities.TryContainsAttribute(semanticModel.Compilation.Assembly, Constants.Logging.LoggerGenerationAttribute, token, out var attributeData))
			return null;

		return GetLoggerGenerationAttribute(attributeData!, semanticModel, logger, token);
	}

	static public bool IsLogMethod(IMethodSymbol method, CancellationToken token)
		=> Utilities.ContainsAttribute(method, Constants.Logging.LogAttribute, token);
}
