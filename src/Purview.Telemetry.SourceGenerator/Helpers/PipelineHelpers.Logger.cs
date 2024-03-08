﻿using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Purview.Telemetry.Logging;
using Purview.Telemetry.SourceGenerator.Targets;

namespace Purview.Telemetry.SourceGenerator.Helpers;
partial class PipelineHelpers {
	static public bool HasLoggerAttribute(SyntaxNode _, CancellationToken __) => true;

	static public LoggerGenerationTarget? BuildLoggerTransform(GeneratorAttributeSyntaxContext context, IGenerationLogger? logger, CancellationToken token) {
		token.ThrowIfCancellationRequested();

		if (context.TargetNode is not InterfaceDeclarationSyntax interfaceDeclaration) {
			logger?.Error($"Could not find interface syntax from the target node '{context.TargetNode.Flatten()}'.");
			return null;
		}

		if (context.TargetSymbol is not INamedTypeSymbol interfaceSymbol) {
			logger?.Error($"Could not find interface symbol '{interfaceDeclaration.Flatten()}'.");
			return null;
		}

		var semanticModel = context.SemanticModel;
		var loggerTargetAttribute = SharedHelpers.GetLoggerTargetAttribute(context.Attributes[0], semanticModel, logger, token);
		if (loggerTargetAttribute == null) {
			logger?.Error($"Could not find {Constants.Logging.LoggerTargetAttribute} when one was expected '{interfaceDeclaration.Flatten()}'.");

			return null;
		}

		var className = loggerTargetAttribute.ClassName.IsSet
			? loggerTargetAttribute.ClassName.Value!
			: GenerateClassName(interfaceSymbol.Name);

		var loggerDefaults = GetLoggerTargetAttributeRecord(semanticModel, logger, token);
		var defaultLogLevel = loggerDefaults?.DefaultLevel?.IsSet == true
			? loggerDefaults.DefaultLevel.Value!
			: LogGeneratedLevel.Information;

		var fullNamespace = Utilities.GetFullNamespace(interfaceDeclaration, true);
		var logEntryMethods = BuildLoggerMethods(
			className,
			defaultLogLevel,
			loggerTargetAttribute,
			context,
			semanticModel,
			interfaceSymbol,
			logger,
			token);

		return new(
			ClassNameToGenerate: className,
			ClassNamespace: Utilities.GetNamespace(interfaceDeclaration),
			ParentClasses: Utilities.GetParentClasses(interfaceDeclaration),
			FullNamespace: fullNamespace,
			FullyQualifiedName: fullNamespace + className,

			InterfaceName: interfaceSymbol.Name,
			FullyQualifiedInterfaceName: fullNamespace + interfaceSymbol.Name,

			LoggerTargetAttribute: loggerTargetAttribute,
			DefaultLevel: (LogGeneratedLevel)defaultLogLevel,

			LogEntryMethods: logEntryMethods
		);
	}

	static ImmutableArray<LogEntryMethodGenerationTarget> BuildLoggerMethods(
		string className,
		LogGeneratedLevel? defaultLogLevel,
		LoggerTargetAttributeRecord loggerTarget,
		GeneratorAttributeSyntaxContext context,
		SemanticModel semanticModel,
		INamedTypeSymbol interfaceSymbol,
		IGenerationLogger? logger,
		CancellationToken token) {

		token.ThrowIfCancellationRequested();

		List<LogEntryMethodGenerationTarget> methodTargets = [];
		foreach (var method in interfaceSymbol.GetMembers().OfType<IMethodSymbol>()) {
			if (Utilities.ContainsAttribute(method, Constants.Logging.ExcludeAttribute, token)) {
				logger?.Debug($"Skipping {interfaceSymbol.Name}.{method.Name}, explicitly excluded.");
				continue;
			}

			logger?.Debug($"Found method {interfaceSymbol.Name}.{method.Name}.");

			var methodParameters = GetLoggerMethodParameters(method, logger, token);
			var logEntry = SharedHelpers.GetLogEntryAttribute(method, semanticModel, logger, token);
			var isKnownReturnType = method.ReturnsVoid || Constants.System.IDisposable.Equals(method.ReturnType);
			var loggerActionFieldName = $"_{Utilities.LowercaseFirstChar(method.Name)}Action";

			var messageTemplate = logEntry?.MessageTemplate?.Value ?? GenerateTemplateMessage(methodParameters);
			var logEntryName = GetLogEntryName(interfaceSymbol.Name, className, loggerTarget, logEntry, method.Name);
			var hasMultipleExceptions = methodParameters.Count(m => m.IsException) > 1;
			var exceptionParam = hasMultipleExceptions
				? null
				: methodParameters.FirstOrDefault(m => m.IsException);

			var inferredErrorLevel = logEntry?.Level?.IsSet ?? false == false && exceptionParam != null;
			var level = (LogGeneratedLevel)(logEntry?.Level?.IsSet == true
				? logEntry.Level.Value!
				: exceptionParam == null
					? defaultLogLevel
					: LogGeneratedLevel.Error
				)!;

			methodTargets.Add(new(
				MethodName: method.Name,
				IsScoped: !method.ReturnsVoid,
				LoggerActionFieldName: loggerActionFieldName,

				UnknownReturnType: !isKnownReturnType,

				EventId: logEntry?.EventId?.Value,
				Level: level,
				MessageTemplate: messageTemplate,

				LogEntryName: logEntryName,
				MSLevel: Utilities.ConvertToMSLogLevel(level),

				AllParameters: methodParameters,
				ParametersSansException: [.. methodParameters.Where(m => !m.IsException)],
				ExceptionParameter: exceptionParam,

				HasMultipleExceptions: hasMultipleExceptions,
				InferredErrorLevel: inferredErrorLevel,

				Location: method.Locations.FirstOrDefault()
			));
		}

		return [.. methodTargets];
	}

	static string GetLogEntryName(string interfaceName, string className, LoggerTargetAttributeRecord loggerTarget, LogEntryAttributeRecord? logEntry, string methodName) {
		if (logEntry?.Name?.Value != null) {
			methodName = logEntry.Name.Value;
		};

		if (loggerTarget.PrefixType.Value == LogPrefixType.Default) {
			if (interfaceName[0] == 'I') {
				interfaceName = interfaceName.Substring(1);
			}

			if (interfaceName.EndsWith("Logs", StringComparison.Ordinal)) {
				interfaceName = interfaceName.Substring(0, interfaceName.Length - 4);
			}
			else if (interfaceName.EndsWith("Logger", StringComparison.Ordinal)) {
				interfaceName = interfaceName.Substring(0, interfaceName.Length - 6);
			}
			else if (interfaceName.EndsWith("Telemetry", StringComparison.Ordinal)) {
				interfaceName = interfaceName.Substring(0, interfaceName.Length - 9);
			}

			return $"{interfaceName}.{methodName}";
		}
		else if (loggerTarget.PrefixType.Value == LogPrefixType.Interface) {
			return $"{interfaceName}.{methodName}";
		}
		else if (loggerTarget.PrefixType.Value == LogPrefixType.Class) {
			return $"{className}.{methodName}";
		}
		else if (loggerTarget.PrefixType.Value == LogPrefixType.Custom) {
			if (!string.IsNullOrWhiteSpace(loggerTarget.CustomPrefix.Value)) {
				return $"{loggerTarget.CustomPrefix.Value}.{methodName}";
			}
		}

		// This is the NoSuffix case or if it's Custom and the CustomPrefix is null, empty or whitespace.
		return methodName;
	}

	static string GenerateTemplateMessage(ImmutableArray<LogEntryMethodParameterTarget> methodParameters) {
		StringBuilder builder = new();

		var count = methodParameters.Count(m => !m.IsException);
		var index = 0;
		foreach (var parameter in methodParameters) {
			if (parameter.IsException) {
				continue;
			}

			builder
				.Append(parameter.Name)
				.Append(": ")
				.Append('{')
				.Append(parameter.UpperCasedName)
				.Append('}')
			;

			if (index < count - 1) {
				builder
					.Append(", ")
				;
			}

			index++;
		}

		return builder.ToString();
	}

	static ImmutableArray<LogEntryMethodParameterTarget> GetLoggerMethodParameters(IMethodSymbol method, IGenerationLogger? logger, CancellationToken token) {

		List<LogEntryMethodParameterTarget> parameters = [];
		foreach (var parameter in method.Parameters) {
			token.ThrowIfCancellationRequested();

			parameters.Add(new(
				Name: parameter.Name,
				UpperCasedName: Utilities.UppercaseFirstChar(parameter.Name),
				FullyQualifiedType: Utilities.GetFullyQualifiedName(parameter.Type),
				IsNullable: parameter.NullableAnnotation == NullableAnnotation.Annotated,
				IsException: Utilities.IsExceptionType(parameter.Type)
			));
		}

		return [.. parameters];
	}

	static string GenerateClassName(string name) {
		if (name[0] == 'I') {
			name = name.Substring(1);
		}

		return name + "Core";
	}

	static LoggerDefaultsAttributeRecord? GetLoggerTargetAttributeRecord(SemanticModel semanticModel, IGenerationLogger? logger, CancellationToken token) {
		token.ThrowIfCancellationRequested();

		if (!Utilities.TryContainsAttribute(semanticModel.Compilation.Assembly, Constants.Logging.LogGeneratedLevel, token, out var attributeData))
			return null;

		return SharedHelpers.GetLoggerDefaultsAttribute(attributeData!, semanticModel, logger, token);
	}
}
