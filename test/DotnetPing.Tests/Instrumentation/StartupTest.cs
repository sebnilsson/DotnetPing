using System.Diagnostics;

namespace DotnetPing.Tests.Instrumentation;

public class StartupTest
{
    [Theory]
    [InlineData("absent", false)]
    [InlineData("partial", true)]
    [InlineData("invalid", true)]
    [InlineData("complete", false)]
    [InlineData("disabled", false)]
    public async Task Help_OptionalTelemetry_DoesNotBreakStartupOrExposeCredentials(string configuration, bool expectsWarning)
    {
        // Arrange
        var testAssembly = typeof(StartupTest).Assembly.Location;
        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("exec");
        startInfo.ArgumentList.Add("--runtimeconfig");
        startInfo.ArgumentList.Add(Path.ChangeExtension(testAssembly, ".runtimeconfig.json"));
        startInfo.ArgumentList.Add("--depsfile");
        startInfo.ArgumentList.Add(Path.ChangeExtension(testAssembly, ".deps.json"));
        startInfo.ArgumentList.Add(typeof(AppSettings).Assembly.Location);
        startInfo.ArgumentList.Add("--help");
        foreach (var name in startInfo.Environment.Keys
            .Where(name => name.StartsWith("AIENT_", StringComparison.Ordinal) || name.StartsWith("OTEL_", StringComparison.Ordinal)).ToArray())
        {
            startInfo.Environment.Remove(name);
        }

        if (configuration != "absent")
        {
            startInfo.Environment["AIENT_ORG_ID"] = "startup-test-org";
        }

        if (configuration is "invalid" or "complete" or "disabled")
        {
            startInfo.Environment["AIENT_PUBLISHABLE_KEY"] = "startup-test-key";
            startInfo.Environment["AIENT_OTLP_ENDPOINT"] = configuration == "complete"
                ? "http://127.0.0.1:4318/v1/traces"
                : "invalid-startup-endpoint";
        }

        if (configuration == "disabled")
        {
            startInfo.Environment["OTEL_SDK_DISABLED"] = "true";
        }

        // Act
        using var process = Process.Start(startInfo)!;
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        var output = await outputTask;
        var error = await errorTask;

        // Assert
        Assert.Equal(0, process.ExitCode);
        Assert.Contains("USAGE", output.ToUpperInvariant());
        Assert.Equal(expectsWarning, error.Contains("Aient telemetry disabled:", StringComparison.Ordinal));
        Assert.DoesNotContain("startup-test-key", output + error);
        Assert.DoesNotContain("startup-test-org", output + error);
        Assert.DoesNotContain("invalid-startup-endpoint", output + error);
    }
}
