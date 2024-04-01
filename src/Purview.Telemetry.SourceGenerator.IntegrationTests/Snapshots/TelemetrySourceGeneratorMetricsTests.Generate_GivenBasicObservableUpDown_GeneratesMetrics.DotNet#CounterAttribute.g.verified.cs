﻿//HintName: CounterAttribute.g.cs
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

namespace Purview.Telemetry.Metrics;

[System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.Conditional("PURVIEW_TELEMETRY_ATTRIBUTES")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
sealed class CounterAttribute : InstrumentAttributeBase {
	public CounterAttribute() {
	}

	public CounterAttribute(bool autoIncrement) {
		AutoIncrement = autoIncrement;
	}

	public CounterAttribute(string name, string? unit = null, string? description = null, bool autoIncrement = false)
		: base(name, unit, description) {
		AutoIncrement = autoIncrement;
	}

	public bool AutoIncrement { get; set; }
}

#endif
