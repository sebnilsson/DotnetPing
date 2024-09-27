using DotnetPing.Http;

namespace DotnetPing.Ping;

public record UrlConfig
{
    public UrlConfig(string url, string method, Config config)
    {
        Url = new Url(url, config.BaseUrl);

        Config = config;

        Method = HttpMethodResolver.Get(method);
    }

    public Config Config { get; }

    public HttpMethod Method { get; init; }

    public Url Url { get; init; } = Url.Empty;
}
