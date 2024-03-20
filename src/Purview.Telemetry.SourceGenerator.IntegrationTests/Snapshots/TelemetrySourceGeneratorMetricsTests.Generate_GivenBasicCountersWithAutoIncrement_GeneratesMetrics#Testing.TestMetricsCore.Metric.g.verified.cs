﻿//HintName: Testing.TestMetricsCore.Metric.g.cs
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
	sealed partial class TestMetricsCore : Testing.ITestMetrics
	{
		System.Diagnostics.Metrics.Meter _meter;

		System.Diagnostics.Metrics.Counter<System.Int32> _counter1Instrument;
		System.Diagnostics.Metrics.Counter<System.Int32> _counter2Instrument;

		public TestMetricsCore(System.Diagnostics.Metrics.IMeterFactory meterFactory)
		{
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

			_meter = meterFactory.Create(new System.Diagnostics.Metrics.MeterOptions("testing-meter")
			{
				Version = null,
				Tags = meterTags
			});

			_counter1Instrument = _meter.CreateCounter<System.Int32>(name: "Counter1", unit: null, description: null, tags: null);
			_counter2Instrument = _meter.CreateCounter<System.Int32>(name: "Counter2", unit: null, description: null, tags: null);
		}

		partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);

		public void Counter1(int intParam, bool boolParam)
		{
			System.Diagnostics.TagList counter1TagList = new System.Diagnostics.TagList();

			counter1TagList.Add("intparam", intParam);
			counter1TagList.Add("boolparam", boolParam);

			_counter1Instrument.Add(1, tagList: counter1TagList);
		}

		public void Counter2(int intParam, bool boolParam)
		{
			System.Diagnostics.TagList counter2TagList = new System.Diagnostics.TagList();

			counter2TagList.Add("intparam", intParam);
			counter2TagList.Add("boolparam", boolParam);

			_counter2Instrument.Add(1, tagList: counter2TagList);
		}
	}
}
