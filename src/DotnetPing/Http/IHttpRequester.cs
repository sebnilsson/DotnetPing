using DotnetPing.Ping;

namespace DotnetPing.Http;

public interface IHttpRequester
{
    Task<HttpResult> Get(UrlConfig url, PingContext context);
}
