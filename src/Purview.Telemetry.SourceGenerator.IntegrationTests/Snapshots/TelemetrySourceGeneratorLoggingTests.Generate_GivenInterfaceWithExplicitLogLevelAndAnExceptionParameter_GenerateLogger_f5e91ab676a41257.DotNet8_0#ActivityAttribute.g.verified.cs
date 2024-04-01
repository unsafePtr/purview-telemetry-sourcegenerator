﻿//HintName: ActivityAttribute.g.cs
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

namespace Purview.Telemetry.Activities;

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
[System.Diagnostics.Conditional("PURVIEW_TELEMETRY_ATTRIBUTES")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
sealed class ActivityAttribute : System.Attribute {

	public ActivityAttribute() {
	}

	public ActivityAttribute(string name) {
		Name = name;
	}

	public ActivityAttribute(System.Diagnostics.ActivityKind kind) {
		Kind = kind;
	}

	public ActivityAttribute(string name, System.Diagnostics.ActivityKind kind, bool createOnly = false) {
		Name = name;
		Kind = kind;
		CreateOnly = createOnly;
	}

	/// <summary>
	/// Optional. Gets the name of the activity.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Optional. Gets the <see cref="System.Diagnostics.ActivityKind">kind</see> of the
	/// activity. Defaults to <see cref="System.Diagnostics.ActivityKind.Internal"/>.
	/// </summary>
	public System.Diagnostics.ActivityKind Kind { get; set; } = System.Diagnostics.ActivityKind.Internal;

	/// <summary>
	/// If true, the Activity is crated via ActivitySource.CreateActivity, meaning it is not started by default. Otherwise
	/// ActivitySource.StartActivity is used. The default is false.
	/// </summary>
	public bool CreateOnly { get; set; }
}
