﻿namespace Purview.Telemetry.Logging;

/// <summary>
/// Sets defaults for the generation of loggers and log entries.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false)]
[System.Diagnostics.Conditional("PURVIEW_TELEMETRY_ATTRIBUTES")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
sealed public class LoggerGenerationAttribute : System.Attribute {
	public LoggerGenerationAttribute() {
	}

	public LoggerGenerationAttribute(Microsoft.Extensions.Logging.LogLevel defaultLevel) {
		DefaultLevel = defaultLevel;
	}

	/// <summary>
	/// Gets/ sets the default <see cref="Microsoft.Extensions.Logging.LogLevel">level</see> of the
	/// logger. Defaults to <see cref="Microsoft.Extensions.Logging.LogLevel.Information"/>.
	/// </summary>
	public Microsoft.Extensions.Logging.LogLevel DefaultLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Information;
}
