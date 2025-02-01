﻿//HintName: TestTelemetryCore.Metric.g.cs
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

sealed partial class TestTelemetryCore : ITestTelemetry
{
	System.Diagnostics.Metrics.Meter _meter = default!;

	System.Diagnostics.Metrics.Counter<int>? _counterInstrument = null;

	public TestTelemetryCore(Microsoft.Extensions.Logging.ILogger<ITestTelemetry> logger, System.Diagnostics.Metrics.IMeterFactory meterFactory)
	{
		_logger = logger;
		InitializeMeters(meterFactory);
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	void InitializeMeters(System.Diagnostics.Metrics.IMeterFactory meterFactory)
	{
		if (_meter != null)
		{
			throw new System.Exception("The meters have already been initialized.");
		}

		System.Collections.Generic.Dictionary<string, object?> meterTags = new System.Collections.Generic.Dictionary<string, object?>();

		PopulateMeterTags(meterTags);

		_meter = meterFactory.Create(new System.Diagnostics.Metrics.MeterOptions("TestTelemetry")
		{
			Version = null,
			Tags = meterTags
		});

		System.Collections.Generic.Dictionary<string, object?> counterTags = new System.Collections.Generic.Dictionary<string, object?>();

		PopulateCounterTags(counterTags);

		_counterInstrument = _meter.CreateCounter<int>(name: "counter", unit: null, description: null, tags: counterTags);
	}

	partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);

	partial void PopulateCounterTags(System.Collections.Generic.Dictionary<string, object?> instrumentTags);

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public bool Counter(int counterValue, int intParam, bool boolParam)
	{
		if (_counterInstrument == null)
		{
			return false;
		}

		System.Diagnostics.TagList counterTagList = new System.Diagnostics.TagList();

		counterTagList.Add("intparam", intParam);
		counterTagList.Add("boolparam", boolParam);

		_counterInstrument.Add(counterValue, tagList: counterTagList);

		return true;
	}
}
