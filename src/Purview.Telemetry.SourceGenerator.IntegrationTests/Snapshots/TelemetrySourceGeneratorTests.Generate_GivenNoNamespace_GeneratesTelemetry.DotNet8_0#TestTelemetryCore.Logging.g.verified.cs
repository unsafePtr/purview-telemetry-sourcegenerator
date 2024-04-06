﻿//HintName: TestTelemetryCore.Logging.g.cs
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
	readonly Microsoft.Extensions.Logging.ILogger<ITestTelemetry> _logger = default!;

	static readonly System.Action<Microsoft.Extensions.Logging.ILogger, System.Int32, System.Boolean, System.Exception?> _logAction = Microsoft.Extensions.Logging.LoggerMessage.Define<System.Int32, System.Boolean>(Microsoft.Extensions.Logging.LogLevel.Information, default, "TestTelemetry.Log: intParam: {IntParam}, boolParam: {BoolParam}");
	static readonly System.Func<Microsoft.Extensions.Logging.ILogger, System.Int32, System.Boolean, System.IDisposable?> _logScopeAction = Microsoft.Extensions.Logging.LoggerMessage.DefineScope<System.Int32, System.Boolean>("TestTelemetry.LogScope: intParam: {IntParam}, boolParam: {BoolParam}");

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void Log(System.Int32 intParam, System.Boolean boolParam)
	{
		if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
		{
			return;
		}

		_logAction(_logger, intParam, boolParam, null);
	}


	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public System.IDisposable? LogScope(System.Int32 intParam, System.Boolean boolParam)
	{
		return _logScopeAction(_logger, intParam, boolParam);
	}

}
