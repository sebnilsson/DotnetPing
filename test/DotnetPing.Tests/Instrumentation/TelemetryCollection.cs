namespace DotnetPing.Tests.Instrumentation;

// SDK providers and HttpClient diagnostic listeners are process-wide.
[CollectionDefinition("Telemetry", DisableParallelization = true)]
public class TelemetryCollection
{
}
