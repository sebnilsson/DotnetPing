using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class UrlConfigTest
{
    [Theory]
    [InlineData("https://learn.microsoft.com/", "/dotnet/", "https://learn.microsoft.com/dotnet/")]
    [InlineData("", "dot.net", "https://dot.net")]
    [InlineData("https://learn.microsoft.com/", "", "")]
    public void BaseUrl_ReturnsFullUrl(string baseUrl, string url, string expected)
    {
        var config = new Config(baseUrl: baseUrl);

        var urlConfig = new UrlConfig(url, AppSettings.DefaultMethod, config);

        Assert.Equal(expected, urlConfig.Url.Value);
    }

    [Fact]
    public void Method_IsResolvedThroughHttpMethodResolver()
    {
        var urlConfig = new UrlConfig("https://example.com", "delete", new Config());

        Assert.Equal(HttpMethod.Delete, urlConfig.Method);
    }
}
