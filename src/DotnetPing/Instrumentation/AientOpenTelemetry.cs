using System;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace DotnetPing.Instrumentation
{
    internal static class AientOpenTelemetry
    {
        public static TracerProvider? Start()
        {
            var orgId = Environment.GetEnvironmentVariable("AIENT_ORG_ID") ?? "57b0ff8b-15a4-4fb6-af4f-e36a5e636097";
            var publishableKey = Environment.GetEnvironmentVariable("AIENT_PUBLISHABLE_KEY") ?? "pk_live_db7c2339aa8948e5b9f2882703d91960";

            // Minimal OpenTelemetry tracer provider. Exporter: OTLP over HTTP to Aient ingest endpoint.
            // Aient recommends using their runtime SDK, but to avoid inventing package names we use generic OTLP exporter
            // and configure endpoint via environment variable. Users can set AIENT_OTLP_ENDPOINT if needed.

            var endpoint = Environment.GetEnvironmentVariable("AIENT_OTLP_ENDPOINT") ?? "https://ingest.aient.dev/v1/traces";

            var provider = Sdk.CreateTracerProviderBuilder()
                .AddSource("DotnetPing")
                .SetResourceBuilder(OpenTelemetry.Resources.ResourceBuilder.CreateDefault().AddService("dotnet-ping"))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpoint);
                    // Pass org and publishable key as headers if exporter supports it
                    options.Headers = $"x-aient-org-id={orgId},x-aient-publishable-key={publishableKey}";
                })
                .Build();

            return provider;
        }
    }
}
