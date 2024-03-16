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
		const Microsoft.Extensions.Logging.LogLevel DEFAULT_LOGLEVEL = Microsoft.Extensions.Logging.LogLevel.Information;

		readonly Microsoft.Extensions.Logging.ILogger<Testing.ITestLogger> _logger;

		static readonly System.Func<Microsoft.Extensions.Logging.ILogger, System.String, System.Int32, System.Exception, System.IDisposable?> _logAction = Microsoft.Extensions.Logging.LoggerMessage.DefineScope<System.String, System.Int32, System.Exception>("Test.Log: stringParam: {StringParam}, intParam: {IntParam}exception: {Exception}");

		public TestLoggerCore(Microsoft.Extensions.Logging.ILogger<Testing.ITestLogger> logger)
		{
			_logger = logger;
		}

		public System.IDisposable Log(System.String stringParam, System.Int32 intParam, System.Exception exception)
		{
			return _logAction(_logger, stringParam, intParam, exception);
		}

	}
}
