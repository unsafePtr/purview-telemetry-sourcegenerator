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

		static readonly System.Action<Microsoft.Extensions.Logging.ILogger, System.Collections.Generic.List<string>, System.Exception?> _logEntryWithGenericTypeParamAction = Microsoft.Extensions.Logging.LoggerMessage.Define<System.Collections.Generic.List<string>>(Microsoft.Extensions.Logging.LogLevel.Information, default, "Test.LogEntryWithGenericTypeParam: paramName: {ParamName}");

		public TestLoggerCore(Microsoft.Extensions.Logging.ILogger<Testing.ITestLogger> logger)
		{
			_logger = logger;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public void LogEntryWithGenericTypeParam(System.Collections.Generic.List<string> paramName)
		{
			if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
			{
				return;
			}

			_logEntryWithGenericTypeParamAction(_logger, paramName, null);
		}

	}
}
