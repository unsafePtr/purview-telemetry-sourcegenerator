﻿//HintName: LoggerGenerationAttribute.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Purview.Telemetry.SourceGenerator
//     on {Scrubbed}.
//
//     Changes to this file may cause incorrect behaviour and will be lost
//     when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591 // publicly visible type or member must be documented

#if PURVIEW_TELEMETRY_EMBED_ATTRIBUTES

namespace Purview.Telemetry.Logging;

/// <summary>
/// Sets defaults for the generation of loggers and log entries.
/// </summary>
[System.AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
[System.Diagnostics.Conditional("PURVIEW_TELEMETRY_ATTRIBUTES")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
sealed class LoggerGenerationAttribute : System.Attribute {
	public LoggerGenerationAttribute() {
	}

	public LoggerGenerationAttribute(LogGeneratedLevel defaultLevel) {
		DefaultLevel = defaultLevel;
	}

	/// <summary>
	/// Gets/ sets the default <see cref="LogGeneratedLevel">level</see> of the
	/// logger. Defaults to <see cref="LogGeneratedLevel.Information"/>.
	/// </summary>
	public LogGeneratedLevel DefaultLevel { get; set; } = LogGeneratedLevel.Information;
}

#endif
