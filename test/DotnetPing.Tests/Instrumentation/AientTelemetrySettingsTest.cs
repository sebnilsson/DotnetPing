using DotnetPing.Instrumentation;

namespace DotnetPing.Tests.Instrumentation;

public class AientTelemetrySettingsTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Read_Unconfigured_IsSilentlyDisabled(string? value)
    {
        // Arrange
        string? GetVariable(string name) => name.StartsWith("AIENT_", StringComparison.Ordinal) ? value : null;

        // Act
        var settings = AientTelemetrySettings.Read(GetVariable, out var warning);

        // Assert
        Assert.Null(settings);
        Assert.Null(warning);
    }

    [Theory]
    [InlineData("AIENT_ORG_ID")]
    [InlineData("AIENT_PUBLISHABLE_KEY")]
    [InlineData("AIENT_OTLP_ENDPOINT")]
    public void Read_MissingOneRequiredVariable_DisablesWithRedactedWarning(string missingVariable)
    {
        // Arrange
        var environment = ValidEnvironment();
        environment.Remove(missingVariable);

        // Act
        var settings = AientTelemetrySettings.Read(name => environment.GetValueOrDefault(name), out var warning);

        // Assert
        Assert.Null(settings);
        Assert.NotNull(warning);
        Assert.DoesNotContain("test-org", warning);
        Assert.DoesNotContain("test-key", warning);
        Assert.Contains(missingVariable, warning);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("/v1/traces")]
    [InlineData("http://collector.example/v1/traces")]
    [InlineData("ftp://localhost/v1/traces")]
    [InlineData("https://test-key@collector.example/v1/traces")]
    [InlineData("https://collector.example/v1/traces?key=test-key")]
    [InlineData("https://collector.example/v1/traces#test-key")]
    public void Read_InvalidEndpoint_DisablesWithoutEchoingValue(string endpoint)
    {
        // Arrange
        var environment = ValidEnvironment();
        environment["AIENT_OTLP_ENDPOINT"] = endpoint;

        // Act
        var settings = AientTelemetrySettings.Read(name => environment.GetValueOrDefault(name), out var warning);

        // Assert
        Assert.Null(settings);
        Assert.NotNull(warning);
        Assert.DoesNotContain(endpoint, warning);
        Assert.DoesNotContain("test-key", warning);
    }

    [Theory]
    [InlineData("https://collector.example/v1/traces")]
    [InlineData("http://localhost:4318/v1/traces")]
    [InlineData("http://127.0.0.1:4318/v1/traces")]
    [InlineData("http://[::1]:4318/v1/traces")]
    public void Read_ExplicitValidConfiguration_PreservesValues(string endpoint)
    {
        // Arrange
        var environment = ValidEnvironment();
        environment["AIENT_OTLP_ENDPOINT"] = endpoint;

        // Act
        var settings = AientTelemetrySettings.Read(name => environment.GetValueOrDefault(name), out var warning);

        // Assert
        Assert.NotNull(settings);
        Assert.Equal(new Uri(endpoint), settings.Endpoint);
        Assert.Equal("test-org", settings.OrganizationId);
        Assert.Equal("test-key", settings.PublishableKey);
        Assert.Null(warning);
    }

    [Theory]
    [InlineData("AIENT_ORG_ID", "test-org,another=value")]
    [InlineData("AIENT_PUBLISHABLE_KEY", "test-key=extra")]
    [InlineData("AIENT_PUBLISHABLE_KEY", "test-key\r\nx-injected: true")]
    [InlineData("AIENT_PUBLISHABLE_KEY", "test key")]
    [InlineData("AIENT_PUBLISHABLE_KEY", "test-å")]
    public void Read_InvalidHeaderValue_DisablesWithoutEchoingCredentials(string name, string value)
    {
        // Arrange
        var environment = ValidEnvironment();
        environment[name] = value;

        // Act
        var settings = AientTelemetrySettings.Read(variable => environment.GetValueOrDefault(variable), out var warning);

        // Assert
        Assert.Null(settings);
        Assert.NotNull(warning);
        Assert.DoesNotContain(value, warning);
        Assert.DoesNotContain("test-key", warning);
    }

    [Theory]
    [InlineData("true")]
    [InlineData("TRUE")]
    public void Read_SdkDisabled_IgnoresInvalidConfiguration(string disabled)
    {
        // Arrange
        string? GetVariable(string name) => name == "OTEL_SDK_DISABLED" ? disabled : "invalid";

        // Act
        var settings = AientTelemetrySettings.Read(GetVariable, out var warning);

        // Assert
        Assert.Null(settings);
        Assert.Null(warning);
    }

    internal static Dictionary<string, string> ValidEnvironment()
    {
        return new Dictionary<string, string>
        {
            ["AIENT_ORG_ID"] = "test-org",
            ["AIENT_PUBLISHABLE_KEY"] = "test-key",
            ["AIENT_OTLP_ENDPOINT"] = "https://collector.invalid/v1/traces",
        };
    }
}
