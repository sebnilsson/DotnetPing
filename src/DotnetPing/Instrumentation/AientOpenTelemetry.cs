using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotnetPing.Instrumentation;

internal static class AientOpenTelemetry
{
    public static TracerProvider? Start(Action<string>? reportWarning = null)
    {
        return Start(Environment.GetEnvironmentVariable, reportWarning);
    }

    internal static TracerProvider? Start(
        Func<string, string?> getEnvironmentVariable,
        Action<string>? reportWarning = null,
        Func<HttpClient>? exporterHttpClientFactory = null)
    {
        var settings = AientTelemetrySettings.Read(getEnvironmentVariable, out var warning);
        if (warning is not null)
        {
            reportWarning?.Invoke(warning);
        }

        if (settings is null)
        {
            return null;
        }

        try
        {
            return Sdk.CreateTracerProviderBuilder()
                .AddSource("DotnetPing")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dotnet-ping"))
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    options.Endpoint = settings.Endpoint;
                    options.Headers = $"x-aient-org-id={settings.OrganizationId},x-aient-publishable-key={settings.PublishableKey}";
                    options.TimeoutMilliseconds = 1_000;
                    if (exporterHttpClientFactory is not null)
                    {
                        options.HttpClientFactory = exporterHttpClientFactory;
                    }
                })
                .Build();
        }
        catch (Exception)
        {
            // Telemetry is optional. SDK errors must not break ping or expose configuration values.
            reportWarning?.Invoke("Aient telemetry disabled: OpenTelemetry initialization failed.");
            return null;
        }
    }
}
