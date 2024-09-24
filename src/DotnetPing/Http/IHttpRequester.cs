using DotnetPing.Ping;

namespace DotnetPing.Http;
public interface IHttpRequester
{
    Task<HttpRequestResult> Get(UrlConfig url, PingContext context);
}