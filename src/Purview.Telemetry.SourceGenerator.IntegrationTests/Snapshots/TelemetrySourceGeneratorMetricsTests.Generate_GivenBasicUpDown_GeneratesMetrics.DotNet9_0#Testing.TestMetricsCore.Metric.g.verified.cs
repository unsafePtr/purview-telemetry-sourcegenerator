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

		System.Diagnostics.Metrics.UpDownCounter<int>? _upDownInstrument = null;
		System.Diagnostics.Metrics.UpDownCounter<int>? _upDown2Instrument = null;

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

			System.Collections.Generic.Dictionary<string, object?> upDownTags = new System.Collections.Generic.Dictionary<string, object?>();

			PopulateUpDownTags(upDownTags);

			_upDownInstrument = _meter.CreateUpDownCounter<int>(name: "updown", unit: null, description: null, tags: upDownTags);
			System.Collections.Generic.Dictionary<string, object?> upDown2Tags = new System.Collections.Generic.Dictionary<string, object?>();

			PopulateUpDown2Tags(upDown2Tags);

			_upDown2Instrument = _meter.CreateUpDownCounter<int>(name: "updown2", unit: null, description: null, tags: upDown2Tags);
		}

		partial void PopulateMeterTags(System.Collections.Generic.Dictionary<string, object?> meterTags);

		partial void PopulateUpDownTags(System.Collections.Generic.Dictionary<string, object?> instrumentTags);

		partial void PopulateUpDown2Tags(System.Collections.Generic.Dictionary<string, object?> instrumentTags);

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public void UpDown(int counterValue, int intParam, bool boolParam)
		{
			if (_upDownInstrument == null)
			{
				return;
			}

			System.Diagnostics.TagList upDownTagList = new System.Diagnostics.TagList();

			upDownTagList.Add("intparam", intParam);
			upDownTagList.Add("boolparam", boolParam);

			_upDownInstrument.Add(counterValue, tagList: upDownTagList);
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public void UpDown2(int counterValue, int intParam, bool boolParam)
		{
			if (_upDown2Instrument == null)
			{
				return;
			}

			System.Diagnostics.TagList upDown2TagList = new System.Diagnostics.TagList();

			upDown2TagList.Add("intparam", intParam);
			upDown2TagList.Add("boolparam", boolParam);

			_upDown2Instrument.Add(counterValue, tagList: upDown2TagList);
		}
	}
}
