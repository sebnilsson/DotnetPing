using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class UrlTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyOrWhitespace_ProducesEmptyValue(string? input)
    {
        var url = new Url(input);

        Assert.Equal(string.Empty, url.Value);
        Assert.False(url.HasValue);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path?q=1")]
    [InlineData("https://sub.example.com:8443/x")]
    public void AbsoluteUrl_IsReturnedUnchanged(string input)
    {
        var url = new Url(input);

        Assert.Equal(input, url.Value);
        Assert.True(url.HasValue);
    }

    [Theory]
    [InlineData("example.com", "https://example.com")]
    [InlineData("sub.example.com/path", "https://sub.example.com/path")]
    public void BareHost_GetsHttpsScheme(string input, string expected)
    {
        var url = new Url(input);

        Assert.Equal(expected, url.Value);
    }

    [Fact]
    public void RelativePath_WithoutBase_ProducesEmptyValueWhenMalformed()
    {
        // "/dotnet" alone is not a well-formed absolute URL, even with the https:// prefix attempt
        var url = new Url("/dotnet");

        Assert.Equal(string.Empty, url.Value);
    }

    [Theory]
    [InlineData("https://learn.microsoft.com/", "/dotnet", "https://learn.microsoft.com/dotnet")]
    [InlineData("https://learn.microsoft.com", "dotnet", "https://learn.microsoft.com/dotnet")]
    [InlineData("https://learn.microsoft.com/", "dotnet/", "https://learn.microsoft.com/dotnet/")]
    [InlineData("https://learn.microsoft.com/", "/dotnet/csharp", "https://learn.microsoft.com/dotnet/csharp")]
    public void RelativePath_WithBase_IsCombinedAndNormalised(string baseUrl, string path, string expected)
    {
        var url = new Url(path, new Url(baseUrl));

        Assert.Equal(expected, url.Value);
    }

    [Fact]
    public void AbsoluteUrl_IgnoresBase()
    {
        var url = new Url("https://other.example.com/x", new Url("https://learn.microsoft.com/"));

        Assert.Equal("https://other.example.com/x", url.Value);
    }

    [Fact]
    public void Empty_StaticInstance_HasNoValue()
    {
        Assert.Equal(string.Empty, Url.Empty.Value);
        Assert.False(Url.Empty.HasValue);
    }
}
