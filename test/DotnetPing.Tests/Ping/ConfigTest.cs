using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class ConfigTest
{
    [Fact]
    public void Defaults_AreAppliedWhenArgumentsAreOmitted()
    {
        var config = new Config();

        Assert.Equal(AppSettings.DefaultSleep, config.Sleep);
        Assert.Equal(AppSettings.DefaultTimeout, config.Timeout);
        Assert.Equal(AppSettings.DefaultExpect, config.ExpectedStatusCodes);
        Assert.False(config.BaseUrl.HasValue);
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(null)]
    public void Sleep_ZeroOrNull_FallsBackToDefault(uint? sleep)
    {
        var config = new Config(sleep: sleep);

        Assert.Equal(AppSettings.DefaultSleep, config.Sleep);
    }

    [Fact]
    public void Sleep_PositiveValue_IsUsed()
    {
        var config = new Config(sleep: 250);

        Assert.Equal(250u, config.Sleep);
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(null)]
    public void Timeout_ZeroOrNull_FallsBackToDefault(uint? timeout)
    {
        var config = new Config(timeout: timeout);

        Assert.Equal(AppSettings.DefaultTimeout, config.Timeout);
    }

    [Fact]
    public void Timeout_PositiveValue_IsUsed()
    {
        var config = new Config(timeout: 7_000);

        Assert.Equal(7_000u, config.Timeout);
    }

    [Fact]
    public void ExpectedStatusCodes_NullOrEmpty_FallsBackToDefault()
    {
        var nullConfig = new Config(expectedStatusCodes: null);
        var emptyConfig = new Config(expectedStatusCodes: []);

        Assert.Equal(AppSettings.DefaultExpect, nullConfig.ExpectedStatusCodes);
        Assert.Equal(AppSettings.DefaultExpect, emptyConfig.ExpectedStatusCodes);
    }

    [Fact]
    public void ExpectedStatusCodes_FiltersOutOfRangeValues()
    {
        var config = new Config(expectedStatusCodes: [50, 100, 200, 404, 600, 700]);

        // Only values strictly between 100 and 600 are kept
        Assert.Equal(new uint[] { 200, 404 }, config.ExpectedStatusCodes);
    }

    [Fact]
    public void ExpectedStatusCodes_AllInvalid_FallsBackToDefault()
    {
        var config = new Config(expectedStatusCodes: [50, 600, 700]);

        Assert.Equal(AppSettings.DefaultExpect, config.ExpectedStatusCodes);
    }

    [Fact]
    public void BaseUrl_IsBuiltFromString()
    {
        var config = new Config(baseUrl: "https://example.com");

        Assert.Equal("https://example.com", config.BaseUrl.Value);
        Assert.True(config.BaseUrl.HasValue);
    }
}
