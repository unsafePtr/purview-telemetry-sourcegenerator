﻿//HintName: Testing.Test1.Test2.TestClass1.TestClass2.TestClass3.TestLoggerCore.Logging.g.cs
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

namespace Testing.Test1.Test2
{
	partial class TestClass1
	{
		partial class TestClass2
		{
			partial class TestClass3
			{
				sealed partial class TestLoggerCore : Testing.Test1.Test2.TestClass1.TestClass2.TestClass3.ITestLogger
				{
					readonly Microsoft.Extensions.Logging.ILogger<Testing.Test1.Test2.TestClass1.TestClass2.TestClass3.ITestLogger> _logger = default!;

					static readonly System.Func<Microsoft.Extensions.Logging.ILogger, string, int, System.IDisposable?> _logAction = Microsoft.Extensions.Logging.LoggerMessage.DefineScope<string, int>("Test.Log: stringParam: {StringParam}, intParam: {IntParam}");

					public TestLoggerCore(Microsoft.Extensions.Logging.ILogger<Testing.Test1.Test2.TestClass1.TestClass2.TestClass3.ITestLogger> logger)
					{
						_logger = logger;
					}

					[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public System.IDisposable? Log(string stringParam, int intParam)
					{
						return _logAction(_logger, stringParam, intParam);
					}

				}
			}
		}
	}
}
