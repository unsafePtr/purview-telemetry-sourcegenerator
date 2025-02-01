﻿//HintName: LoggingTelemetryCore.Logging.g.cs
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

sealed partial class LoggingTelemetryCore : ILoggingTelemetry
{
	readonly Microsoft.Extensions.Logging.ILogger<ILoggingTelemetry> _logger;

	static readonly System.Func<Microsoft.Extensions.Logging.ILogger, System.Guid, System.IDisposable?> _processingWorkItemAction = Microsoft.Extensions.Logging.LoggerMessage.DefineScope<System.Guid>("LoggingTelemetry.ProcessingWorkItem: id: {Id}");
	static readonly System.Action<Microsoft.Extensions.Logging.ILogger, ItemTypes, System.Exception?> _processingItemTypeAction = Microsoft.Extensions.Logging.LoggerMessage.Define<ItemTypes>(Microsoft.Extensions.Logging.LogLevel.Trace, default, "LoggingTelemetry.ProcessingItemType: itemType: {ItemType}");
	static readonly System.Action<Microsoft.Extensions.Logging.ILogger, System.Exception?> _failedToProcessWorkItemAction = Microsoft.Extensions.Logging.LoggerMessage.Define(Microsoft.Extensions.Logging.LogLevel.Error, default, "LoggingTelemetry.FailedToProcessWorkItem");
	static readonly System.Action<Microsoft.Extensions.Logging.ILogger, bool, System.TimeSpan, System.Exception?> _processingCompleteAction = Microsoft.Extensions.Logging.LoggerMessage.Define<bool, System.TimeSpan>(Microsoft.Extensions.Logging.LogLevel.Information, default, "LoggingTelemetry.ProcessingComplete: success: {Success}, duration: {Duration}");

	public LoggingTelemetryCore(Microsoft.Extensions.Logging.ILogger<ILoggingTelemetry> logger)
	{
		_logger = logger;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public System.IDisposable? ProcessingWorkItem(System.Guid id)
	{
		return _processingWorkItemAction(_logger, id);
	}


	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void ProcessingItemType(ItemTypes itemType)
	{
		if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
		{
			return;
		}

		_processingItemTypeAction(_logger, itemType, null);
	}


	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void FailedToProcessWorkItem(System.Exception ex)
	{
		if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error))
		{
			return;
		}

		_failedToProcessWorkItemAction(_logger, ex);
	}


	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void ProcessingComplete(bool success, System.TimeSpan duration)
	{
		if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
		{
			return;
		}

		_processingCompleteAction(_logger, success, duration, null);
	}

}
