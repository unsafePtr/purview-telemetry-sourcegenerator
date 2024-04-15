﻿using System.Globalization;
using System.Reflection;
using System.Text;

namespace Purview.Telemetry.SourceGenerator.Templates;

sealed class EmbeddedResources
{
	readonly Assembly _ownerAssembly = typeof(EmbeddedResources).Assembly;
	readonly string _namespaceRoot = typeof(EmbeddedResources).Namespace;

	// Make sure this is above any calls to LoadTemplateForEmitting.
	readonly string _autoGeneratedHeader;

	EmbeddedResources()
	{
		_autoGeneratedHeader = LoadEmbeddedResource("AutoGeneratedHeader.cs");
	}

	public static EmbeddedResources Instance { get; } = new();

	string LoadEmbeddedResource(string resourceName)
	{
		resourceName = $"{_namespaceRoot}.Sources.{resourceName}";

		var resourceStream = _ownerAssembly.GetManifestResourceStream(resourceName);
		if (resourceStream is null)
		{
			var existingResources = _ownerAssembly.GetManifestResourceNames();
			throw new ArgumentException($"Could not find embedded resource {resourceName}. Available resource names: {string.Join(", ", existingResources)}");
		}

		using StreamReader reader = new(resourceStream, Encoding.UTF8);
		var result = reader.ReadToEnd();

		return result.Trim();
	}

	public string AddHeader(string text)
	{
		var result = _autoGeneratedHeader
			.Replace("{DateTime}", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture))
			+ "\n\n"
			+ text.Trim();

		return result + "\n";
	}

	public string LoadTemplateForEmitting(string? source, string name, bool attachHeader = true)
	{
		source = source == null
			? null
			: source + ".";

		var resource = LoadEmbeddedResource($"{source}{name}.cs");
		return attachHeader
			? AddHeader(resource)
			: resource;
	}
}
