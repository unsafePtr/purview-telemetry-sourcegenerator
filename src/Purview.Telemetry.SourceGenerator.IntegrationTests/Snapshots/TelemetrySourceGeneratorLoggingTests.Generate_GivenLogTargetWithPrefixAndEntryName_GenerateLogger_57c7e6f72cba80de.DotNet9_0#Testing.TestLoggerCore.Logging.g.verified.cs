﻿//HintName: Testing.TestLoggerCore.Logging.g.cs
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
	sealed partial class TestLoggerCore : Testing.ITestLogger
	{
		readonly Microsoft.Extensions.Logging.ILogger<Testing.ITestLogger> _logger;

		static readonly System.Action<Microsoft.Extensions.Logging.ILogger, string, int, bool, System.Exception?> _logAction = Microsoft.Extensions.Logging.LoggerMessage.Define<string, int, bool>(Microsoft.Extensions.Logging.LogLevel.Information, default, "LogNameSetViaLogTargetAttribute: stringParam: {StringParam}, intParam: {IntParam}, boolParam: {BoolParam}");

		public TestLoggerCore(Microsoft.Extensions.Logging.ILogger<Testing.ITestLogger> logger)
		{
			_logger = logger;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public void Log(string stringParam, int intParam, bool boolParam)
		{
			if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
			{
				return;
			}

			_logAction(_logger, stringParam, intParam, boolParam, null);
		}

	}
}
