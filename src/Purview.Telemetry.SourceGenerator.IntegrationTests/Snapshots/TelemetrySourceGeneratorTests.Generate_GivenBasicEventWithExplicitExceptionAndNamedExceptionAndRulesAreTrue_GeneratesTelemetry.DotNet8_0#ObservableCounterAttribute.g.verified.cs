﻿//HintName: ObservableCounterAttribute.g.cs
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

namespace Purview.Telemetry.Metrics;

#if NETSTANDARD1_6_OR_GREATER

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.Conditional("PURVIEW_TELEMETRY_ATTRIBUTES")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
sealed class ObservableCounterAttribute : InstrumentAttributeBase {
	public ObservableCounterAttribute() {
	}

	public ObservableCounterAttribute(string name, string? unit = null, string? description = null, bool throwOnAlreadyInitialized = false)
		: base(name, unit, description) {
		ThrowOnAlreadyInitialized = throwOnAlreadyInitialized;
	}

	public bool ThrowOnAlreadyInitialized { get; set; }
}

#endif
