﻿//HintName: Testing.TestTelemetryCore.Metric.g.cs
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
	sealed partial class TestTelemetryCore : Testing.ITestTelemetry
	{
		System.Diagnostics.Metrics.Meter _meter;


		public TestTelemetryCore(Microsoft.Extensions.Logging.ILogger<Testing.ITestTelemetry> logger, System.Diagnostics.Metrics.IMeterFactory meterFactory)
		{
			_logger = logger;
			InitializeMeters(meterFactory);
		}

		void InitializeMeters(System.Diagnostics.Metrics.IMeterFactory meterFactory)
		{
			if (_meter != null)
			{
				throw new System.Exception("The metrics have already been initialized.");
			}

			System.Collections.Generic.Dictionary<string, object?> meterTags = new System.Collections.Generic.Dictionary<string, object?>();

			PopulateMeterTags(meterTags);

			_meter = meterFactory.Create(new System.Diagnostics.Metrics.MeterOptions("")
			{
				Version = null,
				Tags = meterTags
			});

		}

		partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);
	}
}
