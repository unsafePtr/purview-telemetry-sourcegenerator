﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Purview.Telemetry.SourceGenerator.Records;

namespace Purview.Telemetry.SourceGenerator.Helpers;

partial class PipelineHelpers
{
	public static bool HasActivityTargetAttribute(SyntaxNode _, CancellationToken __) => true;

	public static ActivitySourceTarget? BuildActivityTransform(GeneratorAttributeSyntaxContext context, IGenerationLogger? logger, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		if (context.TargetNode is not InterfaceDeclarationSyntax interfaceDeclaration)
		{
			logger?.Error($"Could not find interface syntax from the target node '{context.TargetNode.Flatten()}'.");
			return null;
		}

		if (context.TargetSymbol is not INamedTypeSymbol interfaceSymbol)
		{
			logger?.Error($"Could not find interface symbol '{interfaceDeclaration.Flatten()}'.");
			return null;
		}

		var semanticModel = context.SemanticModel;
		var activitySourceAttribute = SharedHelpers.GetActivitySourceAttribute(context.Attributes[0], semanticModel, logger, token);
		if (activitySourceAttribute == null)
		{
			logger?.Error($"Could not find {Constants.Activities.ActivitySourceAttribute} when one was expected '{interfaceDeclaration.Flatten()}'.");
			return null;
		}

		var telemetryGeneration = SharedHelpers.GetTelemetryGenerationAttribute(interfaceSymbol, semanticModel, logger, token);
		var className = telemetryGeneration.ClassName.IsSet
			? telemetryGeneration.ClassName.Value!
			: GenerateClassName(interfaceSymbol.Name);

		var activitySourceGenerationAttribute = SharedHelpers.GetActivitySourceGenerationAttribute(semanticModel, logger, token);
		var activitySourceName = activitySourceGenerationAttribute?.Name.IsSet == true
			? activitySourceGenerationAttribute.Name.Value!
			: activitySourceAttribute.Name.IsSet
				? activitySourceAttribute.Name.Value!
				: null;

		var fullNamespace = Utilities.GetFullNamespace(interfaceDeclaration, true);
		var generationType = SharedHelpers.GetGenerationTypes(interfaceSymbol, token);
		var activityMethods = BuildActivityMethods(
			generationType,
			activitySourceAttribute,
			activitySourceGenerationAttribute,
			semanticModel,
			interfaceSymbol,
			logger,
			token
		);

		return new(
			TelemetryGeneration: telemetryGeneration,
			GenerationType: generationType,

			ClassNameToGenerate: className,
			ClassNamespace: Utilities.GetNamespace(interfaceDeclaration),

			ParentClasses: Utilities.GetParentClasses(interfaceDeclaration),
			FullNamespace: fullNamespace,
			FullyQualifiedName: fullNamespace + className,

			InterfaceName: interfaceSymbol.Name,
			FullyQualifiedInterfaceName: fullNamespace + interfaceSymbol.Name,

			ActivitySourceAttribute: activitySourceGenerationAttribute,
			ActivitySourceName: activitySourceName,

			ActivityTargetAttributeRecord: activitySourceAttribute,

			ActivityMethods: activityMethods
		);
	}

	static ImmutableArray<ActivityBasedGenerationTarget> BuildActivityMethods(
		GenerationType generationType,
		ActivitySourceAttributeRecord activitySourceAttribute,
		ActivitySourceGenerationAttributeRecord? activitySourceGenerationAttribute,
		SemanticModel semanticModel,
		INamedTypeSymbol interfaceSymbol,
		IGenerationLogger? logger,
		CancellationToken token)
	{

		token.ThrowIfCancellationRequested();

		var prefix = GeneratePrefix(activitySourceGenerationAttribute, activitySourceAttribute, token);
		var defaultToTags = activitySourceGenerationAttribute?.DefaultToTags.IsSet == true
			? activitySourceGenerationAttribute.DefaultToTags.Value!.Value
			: activitySourceAttribute.DefaultToTags.Value!.Value;
		var lowercaseBaggageAndTagKeys = activitySourceAttribute.LowercaseBaggageAndTagKeys!.Value!.Value;

		List<ActivityBasedGenerationTarget> methodTargets = [];
		foreach (var method in interfaceSymbol.GetMembers().OfType<IMethodSymbol>())
		{
			token.ThrowIfCancellationRequested();

			if (Utilities.ContainsAttribute(method, Constants.Shared.ExcludeAttribute, token))
			{
				logger?.Debug($"Skipping {interfaceSymbol.Name}.{method.Name}, explicitly excluded.");
				continue;
			}

			var (methodType, isInferred) = GetMethodType(method, semanticModel, logger, token,
				out var activityAttribute,
				out var eventAttribute
			);
			var activityOrEventName = activityAttribute?.Name.IsSet == true
				? activityAttribute.Name.Value
				: eventAttribute?.Name.Value;

			if (string.IsNullOrWhiteSpace(activityOrEventName))
				activityOrEventName = method.Name;

			logger?.Debug($"Found {methodType} method {interfaceSymbol.Name}.{method.Name}.");

			var parameters = GetActivityParameters(method, prefix, defaultToTags, lowercaseBaggageAndTagKeys, semanticModel, logger, token);
			var baggageParameters = parameters.Where(m => m.ParamDestination == ActivityParameterDestination.Baggage).ToImmutableArray();
			var tagParameters = parameters.Where(m => m.ParamDestination == ActivityParameterDestination.Tag).ToImmutableArray();

			var returnType = method.ReturnsVoid
				? Constants.System.VoidKeyword
				: Utilities.GetFullyQualifiedOrSystemName(method.ReturnType);

			methodTargets.Add(new(
				MethodName: method.Name,
				ReturnType: returnType,
				IsNullableReturn: method.ReturnType.NullableAnnotation == NullableAnnotation.Annotated,
				ActivityOrEventName: activityOrEventName!,
				MethodLocation: method.Locations.FirstOrDefault(),

				ActivityAttribute: activityAttribute,
				EventAttribute: eventAttribute,

				MethodType: methodType,

				Parameters: parameters,
				Baggage: baggageParameters,
				Tags: tagParameters,

				TargetGenerationState: Utilities.IsValidGenerationTarget(method, generationType, GenerationType.Activities)
			));
		}

		return [.. methodTargets];
	}

	static ImmutableArray<ActivityBasedParameterTarget> GetActivityParameters(IMethodSymbol method,
		string? prefix,
		bool defaultToTags,
		bool lowercaseBaggageAndTagKeys,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token)
	{
		List<ActivityBasedParameterTarget> parameterTargets = [];
		foreach (var parameter in method.Parameters)
		{
			token.ThrowIfCancellationRequested();

			var destination = defaultToTags
				? ActivityParameterDestination.Tag
				: ActivityParameterDestination.Baggage;
			if (Utilities.TryContainsAttribute(parameter, Constants.Shared.TagAttribute, token, out var attribute))
			{
				logger?.Debug($"Found explicit tag: {parameter.Name}.");
				destination = ActivityParameterDestination.Tag;
			}
			else if (Utilities.TryContainsAttribute(parameter, Constants.Activities.BaggageAttribute, token, out attribute))
			{
				logger?.Debug($"Found explicit baggage: {parameter.Name}.");
				destination = ActivityParameterDestination.Baggage;
			}
			else if (Utilities.ContainsAttribute(parameter, Constants.Activities.EscapeAttribute, token))
			{
				logger?.Debug($"Found escape parameter: {parameter.Name}.");
				destination = ActivityParameterDestination.Escape;
			}
			else if (Constants.Activities.SystemDiagnostics.Activity.Equals(parameter.Type))
				destination = ActivityParameterDestination.Activity;
			else if (Constants.Activities.SystemDiagnostics.ActivityTagsCollection.Equals(parameter.Type)
				|| Constants.Activities.SystemDiagnostics.ActivityTagIEnumerable.Equals(parameter.Type)
				|| Constants.System.TagList.Equals(parameter.Type))
				destination = ActivityParameterDestination.TagsEnumerable;
			else if (Constants.Activities.SystemDiagnostics.ActivityContext.Equals(parameter.Type)
				|| (parameter.Name == Constants.Activities.ParentIdParameterName && Utilities.IsString(parameter.Type)))
				destination = ActivityParameterDestination.ParentContextOrId;
			else if (Constants.Activities.SystemDiagnostics.ActivityLinkArray.Equals(parameter.Type)
				|| Constants.Activities.SystemDiagnostics.ActivityLinkIEnumerable.Equals(parameter.Type))
				destination = ActivityParameterDestination.LinksEnumerable;
			else if (parameter.Name == Constants.Activities.StartTimeParameterName && Constants.System.DateTimeOffset.Equals(parameter.Type))
				destination = ActivityParameterDestination.StartTime;
			else if (parameter.Name == Constants.Activities.TimeStampParameterName && Constants.System.DateTimeOffset.Equals(parameter.Type))
				destination = ActivityParameterDestination.Timestamp;
			else
				logger?.Debug($"Inferring {(defaultToTags ? "tag" : "baggage")}: {parameter.Name}.");
			// destination is already set to default.

			TagOrBaggageAttributeRecord? tagOrBaggageAttribute = null;
			if (attribute != null)
				tagOrBaggageAttribute = SharedHelpers.GetTagOrBaggageAttribute(attribute, semanticModel, logger, token);

			var parameterName = parameter.Name;
			var parameterType = Utilities.GetFullyQualifiedOrSystemName(parameter.Type);
			var generatedName = GenerateParameterName(tagOrBaggageAttribute?.Name.Value ?? parameterName, prefix, lowercaseBaggageAndTagKeys);

			parameterTargets.Add(new(
				ParameterName: parameterName,
				ParameterType: parameterType,
				IsNullable: parameter.Type.NullableAnnotation == NullableAnnotation.Annotated,
				IsException: Utilities.IsExceptionType(parameter.Type),
				GeneratedName: generatedName,
				ParamDestination: destination,
				SkipOnNullOrEmpty: GetSkipOnNullOrEmptyValue(tagOrBaggageAttribute),
				Location: parameter.Locations.FirstOrDefault()
			));
		}

		return [.. parameterTargets];
	}

	static (ActivityMethodType, bool) GetMethodType(
		IMethodSymbol method,
		SemanticModel semanticModel,
		IGenerationLogger? logger,
		CancellationToken token,
		out ActivityAttributeRecord? activityAttribute,
		out EventAttributeRecord? eventAttribute)
	{
		activityAttribute = null;
		eventAttribute = null;

		token.ThrowIfCancellationRequested();

		if (Utilities.TryContainsAttribute(method, Constants.Activities.ActivityAttribute, token, out var attributeData))
		{
			activityAttribute = SharedHelpers.GetActivityGenAttribute(attributeData!, semanticModel, logger, token);

			logger?.Debug($"Found explicit activity: {method.Name}.");

			return (ActivityMethodType.Activity, false);
		}

		if (Utilities.TryContainsAttribute(method, Constants.Activities.EventAttribute, token, out attributeData))
		{
			eventAttribute = SharedHelpers.GetActivityEventAttribute(attributeData!, semanticModel, logger, token);

			logger?.Debug($"Found explicit event: {method.Name}.");

			return (ActivityMethodType.Event, false);
		}

		if (Utilities.ContainsAttribute(method, Constants.Activities.ContextAttribute, token))
		{
			logger?.Debug($"Found explicit context: {method.Name}.");

			return (ActivityMethodType.Context, false);
		}

		var returnType = method.ReturnType;
		if (Constants.Activities.SystemDiagnostics.Activity.Equals(returnType))
		{
			logger?.Debug($"Inferring activity due to return type ({returnType.ToDisplayString()}): {method.Name}.");

			return (ActivityMethodType.Activity, true);
		}

		if (method.Name.EndsWith("Event", StringComparison.Ordinal))
		{
			logger?.Debug($"Inferring event as the method name ends in 'Event': {method.Name}.");

			return (ActivityMethodType.Event, true);
		}

		if (method.Name.EndsWith("Context", StringComparison.Ordinal))
		{
			logger?.Debug($"Inferring context as the method name ends in 'Context': {method.Name}.");

			return (ActivityMethodType.Context, true);
		}

		logger?.Debug($"Defaulting to activity: {method.Name}.");

		return (ActivityMethodType.Activity, true);
	}

	static string? GeneratePrefix(
		ActivitySourceGenerationAttributeRecord? activitySourceGenerationRecord,
		ActivitySourceAttributeRecord activitySourceRecord,
		CancellationToken token)
	{

		token.ThrowIfCancellationRequested();

		string? prefix = null;
		var separator = activitySourceGenerationRecord?.BaggageAndTagSeparator.IsSet == true
			? activitySourceGenerationRecord.BaggageAndTagSeparator.Or(".")
			: ".";

		var activitySourceGenPrefix = activitySourceGenerationRecord?.BaggageAndTagPrefix.Value;
		var activitySourcePrefix = activitySourceRecord.BaggageAndTagPrefix.Value;
		var includeActivitySource = activitySourceRecord.IncludeActivitySourcePrefix.Value ?? true;

		if (!string.IsNullOrWhiteSpace(activitySourceGenPrefix))
			prefix = activitySourceGenPrefix + separator;

		if (!string.IsNullOrWhiteSpace(activitySourcePrefix))
		{
			prefix = includeActivitySource
				? prefix + activitySourcePrefix + separator
				: activitySourcePrefix + separator;
		}

		return prefix;
	}
}
