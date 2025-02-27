﻿using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Purview.Telemetry.SourceGenerator.Templates;

namespace Purview.Telemetry.SourceGenerator;

static partial class TestHelpers
{
	static readonly Assembly OwnerAssembly = typeof(TestHelpers).Assembly;
	static readonly string NamespaceRoot = typeof(TestHelpers).Namespace!;

	public const string DefaultUsingSet = @"
using System;
using Purview.Telemetry;

";

	public static string Wrap(this string value, char c = '"')
		=> c + value + c;

	public static string LoadEmbeddedResource(string folder, string resourceName)
	{
		resourceName = $"{NamespaceRoot}.Resources.{folder}.{resourceName}";

		var resourceStream = OwnerAssembly.GetManifestResourceStream(resourceName);
		if (resourceStream is null)
		{
			var existingResources = OwnerAssembly.GetManifestResourceNames();
			throw new ArgumentException($"Could not find embedded resource {resourceName}. Available resource names: {string.Join(", ", existingResources)}");
		}

		using StreamReader reader = new(resourceStream, Encoding.UTF8);

		return reader.ReadToEnd();
	}

	public static bool IsModifierPresent(MemberDeclarationSyntax member, SyntaxKind modifier)
		=> member.Modifiers.Any(m => m.IsKind(modifier));

	public static List<string> GetCasePermutations(string input)
	{
		List<string> result = [];

		if (string.IsNullOrWhiteSpace(input))
		{
			result.Add(input);
			return result;
		}

		var currentChar = input[0];
		var remainder = input.Substring(1);
		var remainderPermutations = GetCasePermutations(remainder);

		if (char.IsLetter(currentChar))
		{
			foreach (var s in remainderPermutations)
			{
				result.Add(char.ToLower(currentChar, System.Globalization.CultureInfo.InvariantCulture) + s);
				result.Add(char.ToUpper(currentChar, System.Globalization.CultureInfo.InvariantCulture) + s);
			}
		}
		else
		{
			foreach (var s in remainderPermutations)
				result.Add(currentChar + s);
		}

		return result;
	}

	public static string GetFriendlyTypeName<T>()
		=> GetFriendlyTypeName(typeof(T));

	public static string GetFriendlyTypeName(Type type, bool useSystemType = true)
	{
		if (type.IsGenericParameter)
			return type.Name;

		if (!type.IsGenericType)
		{
			if (useSystemType)
			{
				switch (type.FullName)
				{
					case "System.String":
						return "string";
					case "System.Int32":
						return "int";
					case "System.Int64":
						return "long";
					case "System.Int16":
						return "short";
					case "System.Boolean":
						return "bool";
					case "System.Single":
						return "float";
					case "System.Double":
						return "double";
					case "System.Decimal":
						return "decimal";
					case "System.Byte":
						return "byte";
					case "System.SByte":
						return "sbyte";
					case "System.UInt16":
						return "ushort";
					case "System.UInt32":
						return "uint";
					case "System.UInt64":
						return "ulong";
					case "System.Char":
						return "char";
					case "System.Object":
						return "object";
				}
			}

			return type.FullName ?? type.Name;
		}

		var name = type.Name;
		var index = name.IndexOf('`', StringComparison.OrdinalIgnoreCase);

		StringBuilder builder = new();
		builder
			.Append(type.Namespace)
			.Append('.')
			.Append(name.Substring(0, index))
			.Append('<');

		var first = true;
		foreach (var arg in type.GetGenericArguments())
		{
			if (!first)
				builder.Append(',');

			builder.Append(GetFriendlyTypeName(arg));
			first = false;
		}

		builder.Append('>');
		return builder.ToString();
	}

	public static async Task Verify(GenerationResult generationResult,
		Action<SettingsTask>? config = null,
		bool validateNonEmptyDiagnostics = false,
		bool whenValidatingDiagnosticsIgnoreNonErrors = false,
		bool validationCompilation = true,
		bool autoVerifyTemplates = true)
	{

		var verifierTask = Verifier
			.Verify(generationResult.Result)
			.UseDirectory("Snapshots")
			.DisableRequireUniquePrefix()
			.DisableDateCounting()
			.HashParameters()
			.UniqueForTargetFrameworkAndVersion(typeof(TestHelpers).Assembly)
			.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzzz") // 2024-22-02 14:43:22 +00:00
			.AutoVerify(file =>
			{
				if (autoVerifyTemplates)
				{
					foreach (TemplateInfo template in Constants.GetAllTemplates())
					{
						var potentialName = $"#{template.Name}.g.";
						if (file.IndexOf(potentialName, StringComparison.Ordinal) > -1)
							return true;
					}
				}

				return false;
			});

		config?.Invoke(verifierTask);

		//verifierTask = verifierTask.AutoVerify();

		await verifierTask;

		var diag = generationResult.Diagnostics.AsEnumerable();
		if (whenValidatingDiagnosticsIgnoreNonErrors)
			diag = diag.Where(m => m.Severity == DiagnosticSeverity.Error);

		if (validateNonEmptyDiagnostics)
			diag.ShouldNotBeEmpty();
		else
			diag.ShouldBeEmpty();

		if (!validationCompilation)
			return;

		await using MemoryStream ms = new();

		EmitResult result = generationResult.Compilation.Emit(ms);

		if (!result.Success)
		{
			result
				.Diagnostics
				.Where(m => !m.Id.StartsWith("TSG", StringComparison.Ordinal))
				.ShouldBeEmpty(string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString() + Environment.NewLine + "-----------------------------------------------------")));
		}
	}
}
