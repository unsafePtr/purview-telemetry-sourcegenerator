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

		System.Diagnostics.Metrics.ObservableCounter<System.Int32> _counterInstrument;
		System.Diagnostics.Metrics.ObservableGauge<System.Int32> _gaugeInstrument;
		System.Diagnostics.Metrics.ObservableUpDownCounter<System.Int32> _upDownInstrument;

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

		}

		partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);

		public System.Boolean Counter(System.Func<int> counterValue, int intParam, bool boolParam)
		{
			if (_counterInstrument != null)
			{
				throw new System.Exception("Counter has already been initialized.");
			}

			System.Diagnostics.TagList counterTagList = new System.Diagnostics.TagList();

			counterTagList.Add("intparam", intParam);
			counterTagList.Add("boolparam", boolParam);

			_counterInstrument = _meter.CreateObservableCounter<System.Int32>("Counter", counterValue, unit: null, description: null, tags: counterTagList);

			return true;
		}

		public System.Boolean Gauge(System.Func<int> counterValue, int intParam, bool boolParam)
		{
			if (_gaugeInstrument != null)
			{
				throw new System.Exception("Gauge has already been initialized.");
			}

			System.Diagnostics.TagList gaugeTagList = new System.Diagnostics.TagList();

			gaugeTagList.Add("intparam", intParam);
			gaugeTagList.Add("boolparam", boolParam);

			_gaugeInstrument = _meter.CreateObservableGauge<System.Int32>("Gauge", counterValue, unit: null, description: null, tags: gaugeTagList);

			return true;
		}

		public System.Boolean UpDown(System.Func<int> counterValue, int intParam, bool boolParam)
		{
			if (_upDownInstrument != null)
			{
				throw new System.Exception("UpDown has already been initialized.");
			}

			System.Diagnostics.TagList upDownTagList = new System.Diagnostics.TagList();

			upDownTagList.Add("intparam", intParam);
			upDownTagList.Add("boolparam", boolParam);

			_upDownInstrument = _meter.CreateObservableUpDownCounter<System.Int32>("UpDown", counterValue, unit: null, description: null, tags: upDownTagList);

			return true;
		}
	}
}
