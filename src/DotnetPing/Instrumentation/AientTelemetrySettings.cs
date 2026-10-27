namespace DotnetPing.Instrumentation;

internal sealed class AientTelemetrySettings
{
    private AientTelemetrySettings(Uri endpoint, string organizationId, string publishableKey)
    {
        Endpoint = endpoint;
        OrganizationId = organizationId;
        PublishableKey = publishableKey;
    }

    public Uri Endpoint { get; }

    public string OrganizationId { get; }

    public string PublishableKey { get; }

    public static AientTelemetrySettings? Read(Func<string, string?> getEnvironmentVariable, out string? warning)
    {
        warning = null;
        if (string.Equals(getEnvironmentVariable("OTEL_SDK_DISABLED"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var organizationId = getEnvironmentVariable("AIENT_ORG_ID");
        var publishableKey = getEnvironmentVariable("AIENT_PUBLISHABLE_KEY");
        var endpointValue = getEnvironmentVariable("AIENT_OTLP_ENDPOINT");
        if (string.IsNullOrWhiteSpace(organizationId)
            && string.IsNullOrWhiteSpace(publishableKey)
            && string.IsNullOrWhiteSpace(endpointValue))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(organizationId)
            || string.IsNullOrWhiteSpace(publishableKey)
            || string.IsNullOrWhiteSpace(endpointValue))
        {
            warning = "Aient telemetry disabled: set AIENT_ORG_ID, AIENT_PUBLISHABLE_KEY and AIENT_OTLP_ENDPOINT together.";
            return null;
        }

        if (!IsHeaderValue(organizationId) || !IsHeaderValue(publishableKey))
        {
            warning = "Aient telemetry disabled: credentials must contain printable ASCII without whitespace, commas or equals signs.";
            return null;
        }

        if (!Uri.TryCreate(endpointValue, UriKind.Absolute, out var endpoint)
            || (endpoint.Scheme != Uri.UriSchemeHttps && !(endpoint.Scheme == Uri.UriSchemeHttp && endpoint.IsLoopback))
            || string.IsNullOrEmpty(endpoint.Host)
            || !string.IsNullOrEmpty(endpoint.UserInfo)
            || !string.IsNullOrEmpty(endpoint.Query)
            || !string.IsNullOrEmpty(endpoint.Fragment))
        {
            warning = "Aient telemetry disabled: AIENT_OTLP_ENDPOINT must be an absolute HTTPS trace URL (HTTP is allowed only on loopback), without user info, query or fragment.";
            return null;
        }

        return new AientTelemetrySettings(endpoint, organizationId, publishableKey);
    }

    private static bool IsHeaderValue(string value)
    {
        return value.All(character => character > ' ' && character < 127 && character != ',' && character != '=');
    }
}
