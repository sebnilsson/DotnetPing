using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class PingContextTest
{
    [Fact]
    public void Construction_FiltersOutEmptyUrls()
    {
        var config = new Config();
        var urls = new[]
        {
            new UrlConfig("https://example.com", "GET", config),
            new UrlConfig("", "GET", config),       // empty -> filtered
            new UrlConfig("   ", "GET", config),    // whitespace -> filtered
        };

        var context = new PingContext(urls, useMinimal: false, useDebug: false);

        var only = Assert.Single(context.Urls);
        Assert.Equal("https://example.com", only.Url.Value);
    }

    [Fact]
    public void Construction_DeduplicatesByUrlAndMethod()
    {
        var config = new Config();
        var urls = new[]
        {
            new UrlConfig("https://example.com", "GET", config),
            new UrlConfig("https://example.com", "GET", config), // duplicate
            new UrlConfig("https://example.com", "POST", config), // same url, different method
            new UrlConfig("https://other.example.com", "GET", config),
        };

        var context = new PingContext(urls, useMinimal: false, useDebug: false);

        Assert.Equal(3, context.Urls.Length);
    }
}
