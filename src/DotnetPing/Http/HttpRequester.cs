using DotnetPing.Ping;

namespace DotnetPing.Http;

public class HttpRequester(IHttpClientFactory httpClientFactory) : IHttpRequester
{
    public async Task<HttpRequestResult> Get(UrlConfig url, PingContext context)
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMilliseconds(url.Timeout);

        var method = GetHttpMethod(url);

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url.Url) };

        try
        {
            var response = await client.SendAsync(request);

            return new HttpRequestResult((uint)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            var statusCode = ex.StatusCode != null ? (uint)ex.StatusCode.Value : 0;

            return new HttpRequestResult(statusCode, ex);

        }
        catch (Exception ex)
        {
            return new HttpRequestResult(0, ex);
        }
    }

    private static HttpMethod GetHttpMethod(UrlConfig url)
    {
        // TODO: Extend method to support more than just GET
        return HttpMethod.Get;
    }
}
