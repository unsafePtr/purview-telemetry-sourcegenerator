﻿//HintName: Testing.TestActivitiesCore.Activity.g.cs
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

#nullable enable

namespace Testing
{
	sealed partial class TestActivitiesCore : Testing.ITestActivities
	{
		readonly static System.Diagnostics.ActivitySource _activitySource = new System.Diagnostics.ActivitySource("testing-activity-source");

		static void RecordExceptionInternal(System.Diagnostics.Activity? activity, System.Exception? exception, bool escape)
		{
			if (activity == null || exception == null)
			{
				return;
			}

			System.Diagnostics.ActivityTagsCollection tagsCollection = new System.Diagnostics.ActivityTagsCollection();
			tagsCollection.Add("exception.escaped", escape);
			tagsCollection.Add("exception.message", exception.Message);
			tagsCollection.Add("exception.type", exception.GetType().FullName);
			tagsCollection.Add("exception.stacktrace", exception.StackTrace);

			System.Diagnostics.ActivityEvent recordExceptionEvent = new System.Diagnostics.ActivityEvent(name: "exception", timestamp: default, tags: tagsCollection);

			activity.AddEvent(recordExceptionEvent);
		}

		public System.Diagnostics.Activity Context(System.Diagnostics.Activity activityParameter, string? stringParam, int? intParam, bool? boolParam)
		{
			if (activityParameter != null)
			{
				activityParameter.SetTag("intparam", intParam);
				activityParameter.SetTag("boolparam", boolParam);
				activityParameter.SetBaggage("stringparam", stringParam);
			}

			return activityParameter;
		}

		public System.Diagnostics.Activity? ContextWithNullableParams(System.Diagnostics.Activity? activityParameter, string? stringParam, int? intParam, bool? boolParam)
		{
			if (activityParameter != null)
			{
				activityParameter.SetTag("intparam", intParam);
				activityParameter.SetTag("boolparam", boolParam);
				activityParameter.SetBaggage("stringparam", stringParam);
			}

			return activityParameter;
		}

	}
}
