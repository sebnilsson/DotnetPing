using DotnetPing.Ping;

namespace DotnetPing.Http;

public interface IHttpRequester
{
    public event EventHandler<UrlConfig>? OnResultStarted;

    public event EventHandler<HttpResult>? OnResultCompleted;

    Task<HttpResult> Get(UrlConfig url, PingContext context);
}
