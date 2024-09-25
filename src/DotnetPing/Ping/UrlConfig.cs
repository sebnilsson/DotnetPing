namespace DotnetPing.Ping;

public record UrlConfig : Config
{
    public UrlConfig(string url, Config config)
        : base(sleep: config.Sleep, timeout: config.Timeout, expectedStatusCodes: config.ExpectedStatusCodes)
    {
        Url = EnsureUrl(url, config);
    }

    public bool IsValid => !string.IsNullOrEmpty(Url);

    public string Url { get; init; } = string.Empty;

    private static string EnsureUrl(string url, Config config)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var isUrlAbsolute = Uri.IsWellFormedUriString(url, UriKind.Absolute);
        if (isUrlAbsolute)
        {
            return url;
        }

        if (!string.IsNullOrWhiteSpace(config.BaseUrl))
        {
            return $"{config.BaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
        }

        var urlResult = $"https://{url}";

        return Uri.IsWellFormedUriString(urlResult, UriKind.Absolute) ? urlResult : string.Empty;
    }
}
