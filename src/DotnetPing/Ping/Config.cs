using DotnetPing.Http;

namespace DotnetPing.Ping;

public record Config
{
    public Config(
        string? baseUrl = null,
        uint? sleep = null,
        uint? timeout = null,
        uint[]? expectedStatusCodes = null,
        string? method = null)
    {
        Sleep = sleep > 0 ? sleep.Value : AppSettings.DefaultSleep;
        Timeout = timeout > 0 ? timeout.Value : AppSettings.DefaultTimeout;

        ExpectedStatusCodes = EnsureExpectedStatusCodes(expectedStatusCodes ?? []);
        BaseUrl = baseUrl ?? string.Empty;
        Method = EnsureMethod(method);
    }

    public string BaseUrl { get; init; }

    public uint[] ExpectedStatusCodes { get; init; }

    public string Method { get; init; }

    public uint Sleep { get; init; }

    public uint Timeout { get; init; }

    private static uint[] EnsureExpectedStatusCodes(uint[] expectedStatusCodes)
    {
        var statusCodes = expectedStatusCodes.Where(x => x > 100 && x < 600).ToArray();

        return statusCodes.Length > 0 ? statusCodes : AppSettings.DefaultExpect;
    }

    private static string EnsureMethod(string? method)
    {
        return HttpMethodResolver.Get(method).ToString();
    }
}
