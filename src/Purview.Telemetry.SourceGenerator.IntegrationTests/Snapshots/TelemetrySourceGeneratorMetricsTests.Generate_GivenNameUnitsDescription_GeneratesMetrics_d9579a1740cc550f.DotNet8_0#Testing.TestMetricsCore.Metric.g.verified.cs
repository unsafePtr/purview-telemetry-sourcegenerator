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

		System.Diagnostics.Metrics.Counter<System.Int32>? _metricInstrument = null;

		public TestMetricsCore(
#if NET8_0_OR_GREATER
			System.Diagnostics.Metrics.IMeterFactory meterFactory
#endif
		)
		{
			InitializeMeters(
#if NET8_0_OR_GREATER
				meterFactory
#endif
			);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		void InitializeMeters(
#if NET8_0_OR_GREATER
			System.Diagnostics.Metrics.IMeterFactory meterFactory
#endif
		)
		{
			if (_meter != null)
			{
				throw new System.Exception("The meters have already been initialized.");
			}

#if NET8_0_OR_GREATER
			System.Collections.Generic.Dictionary<string, object?> meterTags = new System.Collections.Generic.Dictionary<string, object?>();

			PopulateMeterTags(meterTags);
#endif

			_meter = 
#if NET8_0_OR_GREATER
				meterFactory.Create(new System.Diagnostics.Metrics.MeterOptions("testing-meter")
				{
					Version = null,
					Tags = meterTags
				});
#else
				new System.Diagnostics.Metrics.Meter(name: "testing-meter", version: null);
#endif

#if !NET7_0

			System.Collections.Generic.Dictionary<string, object?> metricTags = new System.Collections.Generic.Dictionary<string, object?>();

			PopulateMetricTags(metricTags);

#endif

			_metricInstrument = _meter.CreateCounter<System.Int32>(name: "a-counter-name-param", unit: "cakes-param", description: "cake sales per-capita-param."
#if !NET7_0
				, tags: metricTags
#endif
			);
		}

#if NET8_0_OR_GREATER

		partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);

#endif

#if !NET7_0

		partial void PopulateMetricTags(System.Collections.Generic.Dictionary<string, object?> instrumentTags);

#endif

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public void Metric(int counterValue, int intParam, bool boolParam)
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
