using DotnetPing.Http;
using DotnetPing.Ping;

namespace DotnetPing.Tests.Http;

public class HttpRequesterTest
{
    [Fact]
    public async Task Get_SendsConfiguredRequestAndReturnsStatusCode()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
        });
        var factory = new StubHttpClientFactory(handler);
        var requester = new HttpRequester(factory);
        var url = new UrlConfig("https://example.com/items", "POST", new Config(timeout: 1_234));
        var context = new PingContext([url], useMinimal: true, useDebug: false);

        // Act
        var result = await requester.Get(url, context);

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal("https://example.com/items", capturedRequest.RequestUri?.ToString());
        Assert.Equal(TimeSpan.FromMilliseconds(1_234), factory.LastClient?.Timeout);
        Assert.Equal("https://example.com/items", result.Url);
        Assert.Equal(201u, result.HttpStatusCode);
        Assert.False(result.IsTimeout);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task Get_HttpRequestException_ReturnsAvailableStatusCode()
    {
        // Arrange
        var handler = new StubHttpMessageHandler(_ =>
            throw new HttpRequestException("Forbidden", null, System.Net.HttpStatusCode.Forbidden));
        var requester = new HttpRequester(new StubHttpClientFactory(handler));
        var url = new UrlConfig("https://example.com", "GET", new Config());
        var context = new PingContext([url], useMinimal: true, useDebug: false);

        // Act
        var result = await requester.Get(url, context);

        // Assert
        Assert.Equal(403u, result.HttpStatusCode);
        Assert.False(result.IsTimeout);
        Assert.IsType<HttpRequestException>(result.Exception);
    }

    [Fact]
    public async Task Get_NonHttpException_ReturnsTimeoutResult()
    {
        // Arrange
        var exception = new TaskCanceledException("Timed out");
        var handler = new StubHttpMessageHandler(_ => throw exception);
        var requester = new HttpRequester(new StubHttpClientFactory(handler));
        var url = new UrlConfig("https://example.com", "GET", new Config());
        var context = new PingContext([url], useMinimal: true, useDebug: false);

        // Act
        var result = await requester.Get(url, context);

        // Assert
        Assert.Equal(0u, result.HttpStatusCode);
        Assert.True(result.IsTimeout);
        Assert.Same(exception, result.Exception);
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient? LastClient { get; private set; }

        public HttpClient CreateClient(string name)
        {
            LastClient = new HttpClient(handler);
            return LastClient;
        }
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(send(request));
        }
    }
}
