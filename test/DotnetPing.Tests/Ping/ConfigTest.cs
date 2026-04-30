using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class ConfigTest
{
    [Fact]
    public void Defaults_AreAppliedWhenArgumentsAreOmitted()
    {
        // Arrange

        // Act
        var config = new Config();

        // Assert
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
        // Arrange

        // Act
        var config = new Config(sleep: sleep);

        // Assert
        Assert.Equal(AppSettings.DefaultSleep, config.Sleep);
    }

    [Fact]
    public void Sleep_PositiveValue_IsUsed()
    {
        // Arrange

        // Act
        var config = new Config(sleep: 250);

        // Assert
        Assert.Equal(250u, config.Sleep);
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(null)]
    public void Timeout_ZeroOrNull_FallsBackToDefault(uint? timeout)
    {
        // Arrange

        // Act
        var config = new Config(timeout: timeout);

        // Assert
        Assert.Equal(AppSettings.DefaultTimeout, config.Timeout);
    }

    [Fact]
    public void Timeout_PositiveValue_IsUsed()
    {
        // Arrange

        // Act
        var config = new Config(timeout: 7_000);

        // Assert
        Assert.Equal(7_000u, config.Timeout);
    }

    [Fact]
    public void ExpectedStatusCodes_NullOrEmpty_FallsBackToDefault()
    {
        // Arrange

        // Act
        var nullConfig = new Config(expectedStatusCodes: null);
        var emptyConfig = new Config(expectedStatusCodes: []);

        // Assert
        Assert.Equal(AppSettings.DefaultExpect, nullConfig.ExpectedStatusCodes);
        Assert.Equal(AppSettings.DefaultExpect, emptyConfig.ExpectedStatusCodes);
    }

    [Fact]
    public void ExpectedStatusCodes_FiltersOutOfRangeValues()
    {
        // Arrange

        // Act
        var config = new Config(expectedStatusCodes: [50, 100, 200, 404, 600, 700]);

        // Assert
        // Only values strictly between 100 and 600 are kept
        Assert.Equal(new uint[] { 200, 404 }, config.ExpectedStatusCodes);
    }

    [Fact]
    public void ExpectedStatusCodes_AllInvalid_FallsBackToDefault()
    {
        // Arrange

        // Act
        var config = new Config(expectedStatusCodes: [50, 600, 700]);

        // Assert
        Assert.Equal(AppSettings.DefaultExpect, config.ExpectedStatusCodes);
    }

    [Fact]
    public void BaseUrl_IsBuiltFromString()
    {
        // Arrange

        // Act
        var config = new Config(baseUrl: "https://example.com");

        // Assert
        Assert.Equal("https://example.com", config.BaseUrl.Value);
        Assert.True(config.BaseUrl.HasValue);
    }
}
