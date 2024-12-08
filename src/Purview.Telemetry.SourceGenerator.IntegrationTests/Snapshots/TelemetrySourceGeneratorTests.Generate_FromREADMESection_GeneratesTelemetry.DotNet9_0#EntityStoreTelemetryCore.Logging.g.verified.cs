﻿//HintName: EntityStoreTelemetryCore.Logging.g.cs
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

sealed partial class EntityStoreTelemetryCore : IEntityStoreTelemetry
{
	readonly Microsoft.Extensions.Logging.ILogger<IEntityStoreTelemetry> _logger = default!;

	static readonly System.Action<Microsoft.Extensions.Logging.ILogger, int, string, System.Exception?> _processingEntityAction = Microsoft.Extensions.Logging.LoggerMessage.Define<int, string>(Microsoft.Extensions.Logging.LogLevel.Information, default, "EntityStoreTelemetry.ProcessingEntity: entityId: {EntityId}, updateState: {UpdateState}");

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public void ProcessingEntity(int entityId, string updateState)
	{
		if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
		{
			return;
		}

		_processingEntityAction(_logger, entityId, updateState, null);
	}

}
