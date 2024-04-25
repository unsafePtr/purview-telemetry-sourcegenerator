﻿using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Purview.Telemetry.SourceGenerator.Records;

namespace Purview.Telemetry.SourceGenerator.Helpers;

static class Utilities
{
	static readonly SymbolDisplayFormat SymbolDisplayFormat = new(
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
	);

	static readonly Lazy<ImmutableDictionary<Templates.TypeInfo, string>> _typeInfoToSystemTypeMapper = new(GenerateSystemTypeMap);
	static readonly Lazy<ImmutableDictionary<string, string>> _fullTypeNameToSystemTypeMapper = new(() => _typeInfoToSystemTypeMapper.Value.ToImmutableDictionary(x => x.Key.FullName, x => x.Value));

	static ImmutableDictionary<Templates.TypeInfo, string> GenerateSystemTypeMap()
		// Putting this here ensures it's not accessed
		// before the static fields have been initialised.
		=> new Dictionary<Templates.TypeInfo, string>
		{
			{ Constants.System.String, Constants.System.StringKeyword },
			{ Constants.System.Boolean, Constants.System.BoolKeyword },
			{ Constants.System.Byte, Constants.System.ByteKeyword },
			{ Constants.System.Int16, Constants.System.ShortKeyword },
			{ Constants.System.Int32, Constants.System.IntKeyword },
			{ Constants.System.Int64, Constants.System.LongKeyword },
			{ Constants.System.Single, Constants.System.FloatKeyword },
			{ Constants.System.Double, Constants.System.DoubleKeyword },
			{ Constants.System.Decimal, Constants.System.DecimalKeyword }
		}.ToImmutableDictionary();

	static public string Convert(this Templates.TypeInfo type)
		=> _typeInfoToSystemTypeMapper.Value.GetValueOrDefault(type, type.FullName);

	static public string Convert(this string type)
		=> _fullTypeNameToSystemTypeMapper.Value.GetValueOrDefault(type, type);

	public static TargetGeneration IsValidGenerationTarget(IMethodSymbol method, GenerationType generationType, GenerationType requestedType)
	{
		var attributes = method.GetAttributes();
		var activityCount = attributes.Count(m =>
			Constants.Activities.ActivityAttribute.Equals(m)
			|| Constants.Activities.EventAttribute.Equals(m)
			|| Constants.Activities.ContextAttribute.Equals(m)
		);
		var loggingCount = attributes.Count(Constants.Logging.LogAttribute.Equals);
		var metricsCount = attributes.Count(m =>
			Constants.Metrics.CounterAttribute.Equals(m)
			|| Constants.Metrics.UpDownCounterAttribute.Equals(m)
			|| Constants.Metrics.HistogramAttribute.Equals(m)
			|| Constants.Metrics.ObservableCounterAttribute.Equals(m)
			|| Constants.Metrics.ObservableGaugeAttribute.Equals(m)
			|| Constants.Metrics.ObservableUpDownCounterAttribute.Equals(m)
		);

		var count = activityCount + loggingCount + metricsCount;
		var inferenceNotSupportedWithMultiTargeting = false;
		var multiGenerationTargetsNotSupported = false;
		if (generationType != requestedType)
		{
			// This means it's multi-target generation so we need everything to be explicit.
			if (count == 0)
				inferenceNotSupportedWithMultiTargeting = true;
		}

		if (count > 1)
			multiGenerationTargetsNotSupported = true;

		var isValid = !multiGenerationTargetsNotSupported && !inferenceNotSupportedWithMultiTargeting;
		if (isValid)
		{
			if (generationType.HasFlag(GenerationType.Activities) && requestedType == GenerationType.Activities)
				isValid = loggingCount == 0 && metricsCount == 0;

			if (generationType.HasFlag(GenerationType.Logging) && requestedType == GenerationType.Logging)
				isValid = activityCount == 0 && metricsCount == 0;

			if (generationType.HasFlag(GenerationType.Metrics) && requestedType == GenerationType.Metrics)
				isValid = activityCount == 0 && loggingCount == 0;
		}

		return new(
			IsValid: isValid,
			RaiseInferenceNotSupportedWithMultiTargeting: inferenceNotSupportedWithMultiTargeting,
			RaiseMultiGenerationTargetsNotSupported: multiGenerationTargetsNotSupported
		);
	}

	public static string WithNull(this string value) => value + "?";

	public static StringBuilder AggressiveInlining(this StringBuilder builder, int indent)
		=> builder.Append(indent, Constants.System.AggressiveInlining);

	public static StringBuilder IfDefines(this StringBuilder builder, string condition, int indent, params string[] values)
	{
		builder
			.AppendLine()
			.Append("#if ")
			.AppendLine(condition)
			.AppendTabs(indent)
		;

		foreach (var value in values)
			builder.Append(value);

		builder
			.AppendLine()
			.AppendLine("#endif")
		;

		return builder;
	}

	public static StringBuilder AppendTabs(this StringBuilder builder, int tabs)
	{
		for (var i = 0; i < tabs; i++)
			builder.Append('\t');

		return builder;
	}

	public static StringBuilder Append(this StringBuilder builder, int tabs, char value, bool withNewLine = true)
	{
		builder
			.AppendTabs(tabs)
			.Append(value);

		if (withNewLine)
			builder.AppendLine();

		return builder;
	}

	public static StringBuilder Append(this StringBuilder builder, int tabs, string value, bool withNewLine = true)
	{
		builder
			.AppendTabs(tabs)
			.Append(value);

		if (withNewLine)
			builder.AppendLine();

		return builder;
	}

	public static StringBuilder Append(this StringBuilder builder, int tabs, Templates.TypeInfo typeInfo, bool withNewLine = true)
	{
		builder
			.AppendTabs(tabs)
			.Append(typeInfo.Convert());

		if (withNewLine)
			builder.AppendLine();

		return builder;
	}


	public static StringBuilder Append(this StringBuilder builder, Templates.TypeInfo typeInfo)
	{
		builder.Append(typeInfo.Convert());

		return builder;
	}

	//static public StringBuilder AppendLines(this StringBuilder builder, int lineCount = 2) {
	//	for (var i = 0; i < lineCount; i++) {
	//		builder.AppendLine();
	//	}

	//	return builder;
	//}

	public static StringBuilder AppendLine(this StringBuilder builder, char @char)
		=> builder
			.Append(@char)
			.AppendLine();

	//static public StringBuilder AppendWrap(this StringBuilder builder, string value, char c = '"')
	//	=> builder
	//			.Append(c)
	//			.Append(value)
	//			.Append(c);

	public static string Wrap(this string value, char c = '"')
		=> c + value + c;

	public static string Strip(this string value, char c = '"')
	{
		if (value.Length > 1 && value[0] == c)
			value = value.Substring(1);

		if (value.Length > 1 && value[value.Length - 1] == c)
			value = value.Substring(0, value.Length - 1);

		return value;
	}

	//static public string? GetMemberIdentity(MemberDeclarationSyntax memberSyntax) {
	//	if (memberSyntax is MethodDeclarationSyntax method) {
	//		return method.Identifier.ValueText;
	//	}
	//	else if (memberSyntax is PropertyDeclarationSyntax property) {
	//		return property.Identifier.ValueText;
	//	}
	//	else if (memberSyntax is FieldDeclarationSyntax field) {
	//		var variable = field.Declaration.Variables.FirstOrDefault();
	//		return variable?.Identifier.ValueText;
	//	}
	//	else if (memberSyntax is EventFieldDeclarationSyntax @event) {
	//		var variable = @event.Declaration.Variables.FirstOrDefault();
	//		return variable?.Identifier.ValueText;
	//	}
	//	else if (memberSyntax is IndexerDeclarationSyntax indexer) {
	//		return indexer.ToString();
	//	}

	//	return null;
	//}

	public static ClassDeclarationSyntax? GetParentClass(SyntaxNode? node)
	{
		while (node != null)
		{
			if (node.Parent is ClassDeclarationSyntax classNode)
				return classNode;

			node = node.Parent;
		}

		return null;
	}

	public static string[] GetParentClasses(TypeDeclarationSyntax classDeclaration)
	{
		var parentClass = classDeclaration.Parent as ClassDeclarationSyntax;

		List<string> parentClassList = [];
		while (parentClass != null)
		{
			parentClassList.Add(parentClass.Identifier.Text);

			parentClass = parentClass.Parent as ClassDeclarationSyntax;
		}

		return [.. parentClassList];
	}

	public static string? GetParentClassesAsNamespace(TypeDeclarationSyntax classDeclaration)
	{
		var parentClass = classDeclaration.Parent as ClassDeclarationSyntax;

		List<string> parentClasses = [];
		while (parentClass != null)
		{
			parentClasses.Insert(0, parentClass.Identifier.Text);

			parentClass = parentClass.Parent as ClassDeclarationSyntax;
		}

		return parentClasses.Count == 0
			? null
			: string.Join(".", parentClasses);
	}

	public static string? GetNamespace(TypeDeclarationSyntax typeSymbol)
	{
		// Determine the namespace the type is declared in, if any
		var potentialNamespaceParent = typeSymbol.Parent;
		while (potentialNamespaceParent != null &&
			   potentialNamespaceParent is not NamespaceDeclarationSyntax
			   && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
		{
			potentialNamespaceParent = potentialNamespaceParent.Parent;
		}

		if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
		{
			var @namespace = namespaceParent.Name.ToString();
			while (true)
			{
				if (namespaceParent.Parent is not NamespaceDeclarationSyntax namespaceParentParent)
					break;

				namespaceParent = namespaceParentParent;
				@namespace = $"{namespaceParent.Name}.{@namespace}";
			}

			return @namespace;
		}

		return null;
	}

	public static string GetFullyQualifiedOrSystemName(ITypeSymbol namedType, bool trimNullableAnnotation = true)
	{
		var result = namedType.ToDisplayString(SymbolDisplayFormat) ?? namedType.ToString();
		if (trimNullableAnnotation && namedType.NullableAnnotation == NullableAnnotation.Annotated)
			result = result.TrimEnd('?');

		return result.Convert();
	}

	//static public string GetFullyQualifiedName(TypeDeclarationSyntax type)
	//	=> GetFullNamespace(type, true) + type.Identifier.Text;

	public static string? GetFullNamespace(TypeDeclarationSyntax type, bool includeTrailingSeparator)
	{
		var typeNamespace = GetNamespace(type);
		var parentClasses = GetParentClassesAsNamespace(type);

		string? fullNamespace = null;
		if (typeNamespace != null)
			fullNamespace = typeNamespace;

		if (parentClasses != null)
		{
			if (fullNamespace != null)
				fullNamespace += ".";

			fullNamespace += parentClasses;

			if (includeTrailingSeparator)
				fullNamespace += ".";
		}
		else if (includeTrailingSeparator && fullNamespace != null)
			fullNamespace += ".";

		return fullNamespace;
	}

	public static object? GetTypedConstantValue(TypedConstant arg)
		=> arg.Kind == TypedConstantKind.Array
			? arg.Values
			: arg.Value;

	public static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(this IncrementalValuesProvider<TSource> source)
		=> source.Where(static m => m is not null);

	public static bool IsEnumerableOrArray(string parameterType, string fullTypeName)
		=> IsArray(parameterType, fullTypeName)
			|| IsEnumerable(parameterType, fullTypeName);

	public static bool IsArray(string parameterType, string fullTypeName)
		=> parameterType == (fullTypeName + "[]");

	public static bool IsEnumerable(string parameterType, string fullTypeName)
		=> parameterType == (Constants.System.IEnumerable.FullName + "<" + fullTypeName + ">")
		|| parameterType.StartsWith(Constants.System.IEnumerable.FullName + "<" + fullTypeName, StringComparison.Ordinal);

	public static bool IsBoolean(ITypeSymbol type)
		=> Constants.System.Boolean.Equals(type);

	public static bool IsBoolean(string type)
		=> type == Constants.System.BoolKeyword
			|| Constants.System.Boolean.Equals(type);

	public static bool IsString(ITypeSymbol type)
		=> type.ToDisplayString() == Constants.System.StringKeyword
			|| Constants.System.String.Equals(type);

	public static bool IsString(string type)
		=> type == Constants.System.StringKeyword
			|| Constants.System.String.Equals(type);

	public static bool IsExceptionType(ITypeSymbol? typeSymbol)
	{
		while (typeSymbol != null)
		{
			if (Constants.System.Exception.Equals(typeSymbol))
				return true;

			typeSymbol = typeSymbol.BaseType;
		}

		return false;
	}

	public static string Flatten(this SyntaxNode syntax)
		=> syntax.WithoutTrivia()
			.ToString()
			.Flatten();

	public static string Flatten(this SyntaxToken syntax)
		=> syntax.WithoutTrivia()
			.ToString()
			.Flatten();

	public static string Flatten(this string value)
		=> Regex.Replace(value, @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(2000));

	public static bool ContainsAttribute(ISymbol symbol, Templates.TypeInfo typeInfo, CancellationToken token)
		=> TryContainsAttribute(symbol, typeInfo, token, out _);

	public static bool TryContainsAttribute(ISymbol symbol, Templates.TypeInfo typeInfo, CancellationToken token, out AttributeData? attributeData)
	{
		attributeData = null;

		var attributes = symbol.GetAttributes();
		foreach (var attribute in attributes)
		{
			token.ThrowIfCancellationRequested();

			if (attribute.AttributeClass != null && typeInfo.Equals(attribute.AttributeClass))
			{
				attributeData = attribute;
				return true;
			}
		}

		return false;
	}

	public static bool ContainsAttribute(ISymbol symbol, Templates.TemplateInfo templateInfo, CancellationToken token)
		=> TryContainsAttribute(symbol, templateInfo, token, out _);

	public static bool TryContainsAttribute(ISymbol symbol, Templates.TemplateInfo templateInfo, CancellationToken token, out AttributeData? attributeData)
	{
		attributeData = null;

		var attributes = symbol.GetAttributes();
		foreach (var attribute in attributes)
		{
			token.ThrowIfCancellationRequested();

			if (attribute.AttributeClass != null && templateInfo.Equals(attribute.AttributeClass))
			{
				attributeData = attribute;
				return true;
			}
		}

		return false;
	}

	public static string LowercaseFirstChar(string value)
	{
		if (value.Length > 0)
		{
			var firstChar = char.ToLowerInvariant(value[0]);
			value = firstChar + value.Substring(1);
		}

		return value;
	}

	public static string UppercaseFirstChar(string value)
	{
		if (value.Length > 0)
		{
			var firstChar = char.ToUpperInvariant(value[0]);
			value = firstChar + value.Substring(1);
		}

		return value;
	}
}
