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
		System.Diagnostics.Metrics.Meter _meter = default!;

		System.Diagnostics.Metrics.Counter<byte>? _metricInstrument = null;

		public TestMetricsCore(System.Diagnostics.Metrics.IMeterFactory meterFactory)
		{
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

			_meter = meterFactory.Create(new System.Diagnostics.Metrics.MeterOptions("testing-meter")
			{
				Version = null,
				Tags = meterTags
			});

			System.Collections.Generic.Dictionary<string, object?> metricTags = new System.Collections.Generic.Dictionary<string, object?>();

			PopulateMetricTags(metricTags);

			_metricInstrument = _meter.CreateCounter<byte>(name: "a-counter-name-property", unit: "cakes-property", description: "cake sales per-capita-property.", tags: metricTags);
		}

		partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);

		partial void PopulateMetricTags(System.Collections.Generic.Dictionary<string, object?> instrumentTags);

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public void Metric(byte counterValue, int intParam, bool boolParam)
		{
			if (_metricInstrument == null)
			{
				return;
			}

			System.Diagnostics.TagList metricTagList = new System.Diagnostics.TagList();

			metricTagList.Add("intparam", intParam);
			metricTagList.Add("boolparam", boolParam);

			_metricInstrument.Add(counterValue, tagList: metricTagList);
		}
	}
}
