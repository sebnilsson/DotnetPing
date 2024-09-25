using DotnetPing.Http;

namespace DotnetPing.Ping;

public record UrlConfig
{
    public UrlConfig(string url, Config config)
        : this(url, AppSettings.DefaultMethod, config)
    {
    }

    public UrlConfig(string url, string method, Config config)
    {
        Url = EnsureUrl(url, config);

        Config = config;

        Method = EnsureMethod(method);
    }

    public Config Config { get; }

    public bool IsValidUrl => !string.IsNullOrEmpty(Url);

    public string Method { get; init; }

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

    private static string EnsureMethod(string? method)
    {
        return HttpMethodResolver.Get(method).ToString();
    }
}
