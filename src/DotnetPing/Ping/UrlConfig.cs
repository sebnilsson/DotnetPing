namespace DotnetPing.Ping;

public record UrlConfig
{
    public UrlConfig(string url, uint? sleep = null, uint? timeout = null, uint[]? expectedStatusCodes = null)
    {
        Url = EnsureUrl(url);
        Sleep = sleep > 0 ? sleep.Value : AppSettings.DefaultSleep;
        Timeout = timeout > 0 ? timeout.Value : AppSettings.DefaultTimeout;

        var statusCodes = (expectedStatusCodes ?? []).Where(x => x > 100 && x < 600).ToArray();

        ExpectedStatusCodes = statusCodes.Length > 0 ? statusCodes : AppSettings.DefaultExpect;
    }

    public uint[] ExpectedStatusCodes { get; }

    public bool IsValid => !string.IsNullOrEmpty(Url);

    public uint Sleep { get; }

    public uint Timeout { get; }

    public string Url { get; }

    private static string EnsureUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = $"https://{url}";
        }

        return Uri.IsWellFormedUriString(url, UriKind.Absolute) ? url : string.Empty;
    }
}
