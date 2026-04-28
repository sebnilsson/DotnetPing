using DotnetPing.Http;

namespace DotnetPing.Tests.Http;

public class HttpMethodResolverTest
{
    [Theory]
    [InlineData("get", "GET")]
    [InlineData("POST", "POST")]
    [InlineData("put", "PUT")]
    [InlineData("Delete", "DELETE")]
    [InlineData("PATCH", "PATCH")]
    [InlineData("head", "HEAD")]
    [InlineData("options", "OPTIONS")]
    public void Get_KnownMethods_AreResolvedCaseInsensitively(string input, string expectedMethod)
    {
        var result = HttpMethodResolver.Get(input);

        Assert.Equal(expectedMethod, result.Method);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("FOO")]
    [InlineData("connect")]
    [InlineData("trace")]
    public void Get_UnknownOrEmpty_FallsBackToGet(string? input)
    {
        Assert.Equal(HttpMethod.Get, HttpMethodResolver.Get(input));
    }
}
