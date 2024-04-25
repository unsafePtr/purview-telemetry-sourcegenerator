﻿using Microsoft.CodeAnalysis;
using Purview.Telemetry.SourceGenerator.Records;

namespace Purview.Telemetry.SourceGenerator;

partial class TelemetryDiagnostics
{
	// Start at 3000
	public static class Activities
	{
		public static readonly TelemetryDiagnosticDescriptor BaggageParameterShouldBeString = new(
			Id: "TSG3000",
			Title: "Baggage parameter types only accept strings.",
			Description: "Baggage parameter types only accept strings, be aware this parameter will have ToString() called.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Warning
		);

		public static readonly TelemetryDiagnosticDescriptor NoActivitySourceSpecified = new(
			Id: "TSG3001",
			Title: "No activity source specified.",
			Description: $"An activity source helps to identify your application and it's telemetry. Defaulting to '{Constants.Activities.DefaultActivitySourceName}'.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Warning
		);

		public static readonly TelemetryDiagnosticDescriptor InvalidReturnType = new(
			Id: "TSG3002",
			Title: "Invalid return type.",
			Description: $"An activity or event must return either void or an {Constants.Activities.SystemDiagnostics.Activity}.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor DuplicateParameterTypes = new(
			Id: "TSG3003",
			Title: "Duplicate special parameters defined.",
			Description: "{0} are all the same type of parameter ({1}), a maximum or one is allowed. Explicitly define them as either a Tag or Baggage.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor ActivityParameterNotAllowed = new(
			Id: "TSG3004",
			Title: "Activity parameter is not valid.",
			Description: "The {0} parameter is not allowed when defining an activity, only an event.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor TimestampParameterNotAllowed = new(
			Id: "TSG3005",
			Title: "Timestamp parameter is not valid.",
			Description: "The {0} parameter is not allowed when defining an activity, only an event. You can specify this as a Tag or as Baggage to stop the inference.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor StartTimeParameterNotAllowed = new(
			Id: "TSG3006",
			Title: "Start time parameter is not valid on Create activity or Event method.",
			Description: "The {0} parameter is not allowed when defining an activity create or activity event method, only when starting an activity.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor ParentContextOrIdParameterNotAllowed = new(
			Id: "TSG3007",
			Title: "Parent context or Parent Id parameter is not valid on event.",
			Description: "The {0} parameter is not allowed when defining an activity event, only on the activity start/ create method.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor LinksParameterNotAllowed = new(
			Id: "TSG3008",
			Title: "Activity links parameters are not valid on events or context methods.",
			Description: "The {0} parameter is not allowed when defining an activity event or context, only on the activity start/ create method.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor TagsParameterNotAllowed = new(
			Id: "TSG3009",
			Title: "Activity tags parameter are not valid on context methods.",
			Description: "The {0} parameter is not allowed when defining an activity context, only on the activity start/ create methods or events.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor EscapedParameterInvalidType = new(
			Id: "TSG3010",
			Title: "Escaped parameters must be a boolean.",
			Description: "Only boolean parameter types are valid for the escape parameter.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);

		public static readonly TelemetryDiagnosticDescriptor EscapedParameterIsOnlyValidOnEvent = new(
			Id: "TSG3010",
			Title: "Escaped parameters are only valid on Events, not Activity or Context methods.",
			Description: "The parameters {0} is not valid on Activity or Context methods, only on Events.",
			Category: Constants.Diagnostics.Activity.Usage,
			Severity: DiagnosticSeverity.Error
		);
	}
}
