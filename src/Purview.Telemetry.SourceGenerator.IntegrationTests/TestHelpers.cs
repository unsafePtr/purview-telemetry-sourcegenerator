﻿using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

namespace Purview.Telemetry.SourceGenerator;

static partial class TestHelpers {
	readonly static Assembly _ownerAssembly = typeof(TestHelpers).Assembly;
	readonly static string _namespaceRoot = typeof(TestHelpers).Namespace!;

	readonly static public string DefaultUsingSet = @$"
using System;

";

	static public string Wrap(this string value, char c = '"')
		=> c + value + c;

	static public string LoadEmbeddedResource(string folder, string resourceName) {
		resourceName = $"{_namespaceRoot}.Resources.{folder}.{resourceName}";

		var resourceStream = _ownerAssembly.GetManifestResourceStream(resourceName);
		if (resourceStream is null) {
			var existingResources = _ownerAssembly.GetManifestResourceNames();
			throw new ArgumentException($"Could not find embedded resource {resourceName}. Available resource names: {string.Join(", ", existingResources)}");
		}

		using StreamReader reader = new(resourceStream, Encoding.UTF8);

		return reader.ReadToEnd();
	}

	static public bool IsModifierPresent(MemberDeclarationSyntax member, SyntaxKind modifier)
		=> member.Modifiers.Any(m => m.IsKind(modifier));

	static public List<string> GetCasePermutations(string input) {
		List<string> result = [];

		if (string.IsNullOrWhiteSpace(input)) {
			result.Add(input);
			return result;
		}

		char currentChar = input[0];
		string remainder = input[1..];
		List<string> remainderPermutations = GetCasePermutations(remainder);

		if (char.IsLetter(currentChar)) {
			foreach (string s in remainderPermutations) {
				result.Add(char.ToLower(currentChar, System.Globalization.CultureInfo.InvariantCulture) + s);
				result.Add(char.ToUpper(currentChar, System.Globalization.CultureInfo.InvariantCulture) + s);
			}
		}
		else {
			foreach (string s in remainderPermutations) {
				result.Add(currentChar + s);
			}
		}

		return result;
	}

	async static public Task Verify(GenerationResult generationResult,
		Action<SettingsTask>? config = null,
		bool validateNonEmptyDiagnostics = false,
		bool validationCompilation = true,
		bool includeTemplates = true) {

		var verifierTask = Verifier
			.Verify(generationResult.Result)
			.UseDirectory("Snapshots")
			.DisableRequireUniquePrefix()
			.DisableDateCounting()
			.ScrubInlineDateTimeOffsets("yyyy-MM-dd HH:mm:ss zzzz") // 2024-22-02 14:43:22 +00:00

		//.UseStringComparer(StringCompareX)
		//.UseStreamComparer(StreamCompareX)
		;

		if (includeTemplates) {
			foreach (var template in Constants.GetAllTemplates()) {
				// Ignores predefined templates, e.g. ActivityAttribute.g.cs
				verifierTask = verifierTask.AppendContentAsFile(template.TemplateData, name: template.GetGeneratedFilename());
			}
		}

		config?.Invoke(verifierTask);

		//verifierTask = verifierTask.AutoVerify();

		await verifierTask;

		if (validateNonEmptyDiagnostics) {
			generationResult.Diagnostics.Should().NotBeEmpty();
		}
		else {
			generationResult.Diagnostics.Should().BeEmpty();
		}

		if (!validationCompilation) {
			return;
		}

		await using MemoryStream ms = new();
		EmitResult result = generationResult.Compilation.Emit(ms);

		if (!result.Success) {
			result.Diagnostics.Should().NotBeEmpty();
		}
	}

	async static Task<CompareResult> StringCompareX(string received, string verified, IReadOnlyDictionary<string, object> context) {
		return CompareResult.Equal;
	}

	async static Task<CompareResult> StreamCompareX(Stream received, Stream verified, IReadOnlyDictionary<string, object> context) {
		return CompareResult.Equal;
	}
}
