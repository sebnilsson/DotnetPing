using DotnetPing.Ping;

namespace DotnetPing.Http;

public class HttpRequester(IHttpClientFactory httpClientFactory) : IHttpRequester
{
    public async Task<HttpResult> Get(UrlConfig url, PingContext context)
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(url.Config.Timeout);

        var method = HttpMethodResolver.Get(url.Method);

        var request = new HttpRequestMessage { Method = method, RequestUri = new Uri(url.Url) };

        var result = await GetResult(client, request, url);

        return result;
    }

    private static async Task<HttpResult> GetResult(HttpClient client, HttpRequestMessage request, UrlConfig url)
    {
        try
        {
            var response = await client.SendAsync(request);

            return new HttpResult(url.Url, (uint)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            var statusCode = ex.StatusCode != null ? (uint)ex.StatusCode.Value : 0;

            return new HttpResult(url.Url, statusCode, Exception: ex);

        }
        catch (Exception ex)
        {
            return new HttpResult(url.Url, 0, IsTimeout: true, ex);
        }
    }
}
