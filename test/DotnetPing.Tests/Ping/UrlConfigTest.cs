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
        // Arrange
        var config = new Config { BaseUrl = baseUrl };

        // Act
        var urlConfig = new UrlConfig(url, config);

        // Assert
        Assert.Equal(expected, urlConfig.Url);
    }
}
