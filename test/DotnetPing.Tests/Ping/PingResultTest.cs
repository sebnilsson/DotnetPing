using DotnetPing.Http;
using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class PingResultTest
{
    [Fact]
    public void Constructor_CopiesRequestAndResponseDetails()
    {
        // Arrange
        var exception = new HttpRequestException("failed");
        var config = new Config(expectedStatusCodes: [200, 204]);
        var url = new UrlConfig("https://example.com", "PATCH", config);
        var httpResult = new HttpResult("https://example.com", 500, Exception: exception);
        var elapsed = TimeSpan.FromMilliseconds(42);

        // Act
        var result = new PingResult(isSuccess: false, elapsed, httpResult, url);

        // Assert
        Assert.Equal(elapsed, result.Elapsed);
        Assert.Same(exception, result.Exception);
        Assert.Equal(new uint[] { 200, 204 }, result.ExpectedStatusCodes);
        Assert.Equal(500u, result.HttpStatusCode);
        Assert.Equal(HttpMethod.Patch.ToString(), result.Method);
        Assert.Equal("https://example.com", result.Url);
        Assert.Equal(PingResultType.Failure, result.Result);
    }

    [Fact]
    public void Constructor_SuccessTakesPrecedenceOverTimeoutFlag()
    {
        // Arrange
        var url = new UrlConfig("https://example.com", "GET", new Config());
        var httpResult = new HttpResult("https://example.com", 200, IsTimeout: true);

        // Act
        var result = new PingResult(isSuccess: true, TimeSpan.Zero, httpResult, url);

        // Assert
        Assert.Equal(PingResultType.Success, result.Result);
    }
}
