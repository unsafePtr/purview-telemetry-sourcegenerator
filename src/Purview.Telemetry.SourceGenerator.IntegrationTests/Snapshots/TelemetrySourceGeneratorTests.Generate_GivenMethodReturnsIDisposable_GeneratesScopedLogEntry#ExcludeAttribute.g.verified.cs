﻿//HintName: ExcludeAttribute.g.cs
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
/// Excludes the method from any log entry generation.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
sealed class ExcludeAttribute : Attribute {
}

#endif
