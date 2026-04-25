using System;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace DotnetPing.Tests.Instrumentation;

public static class TelemetryInitializer
{
    private static bool _initialized = false;

    public static void EnsureInitialized()
    {
        if (_initialized) return;

        // Minimal OpenTelemetry setup for tests: traces to console for visibility.
        Sdk.CreateTracerProviderBuilder()
           .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DotnetPing.Tests"))
           .AddSource("DotnetPing")
           .AddConsoleExporter()
           .Build();

        _initialized = true;
    }
}
