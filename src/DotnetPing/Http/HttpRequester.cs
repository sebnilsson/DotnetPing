using DotnetPing.Ping;

namespace DotnetPing.Http;

public class HttpRequester(IHttpClientFactory httpClientFactory) : IHttpRequester
{
    public event EventHandler<UrlConfig>? OnResultStarted;

    public event EventHandler<HttpResult>? OnResultCompleted;

    public async Task<HttpResult> Get(UrlConfig url, PingContext context)
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(url.Timeout);

        var method = HttpMethodResolver.Get(url.Method);

        var request = new HttpRequestMessage { Method = method, RequestUri = new Uri(url.Url) };

        OnResultStarted?.Invoke(this, url);

        var result = await GetResult(client, request, url);

        OnResultCompleted?.Invoke(this, result);

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

            return new HttpResult(url.Url, statusCode, IsTimeout: false, ex);

        }
        catch (Exception ex)
        {
            return new HttpResult(url.Url, 0, IsTimeout: true, ex);
        }
    }
}
