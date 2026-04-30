namespace DotnetPing.Tests;

public class AppSettingsTest
{
    [Fact]
    public void ToConfig_AppliesSettings()
    {
        // Arrange
        var settings = new AppSettings
        {
            BaseUrl = "https://example.com",
            Sleep = [123],
            Timeout = 4_321,
            Expect = [200, 404]
        };

        // Act
        var config = settings.ToConfig();

        // Assert
        Assert.Equal("https://example.com", config.BaseUrl.Value);
        Assert.Equal(123u, config.Sleep);
        Assert.Equal(4_321u, config.Timeout);
        Assert.Equal(new uint[] { 200, 404 }, config.ExpectedStatusCodes);
    }

    [Fact]
    public void ToConfig_DefaultsApplyWhenSettingsAreEmpty()
    {
        // Arrange
        var settings = new AppSettings { Sleep = [] };

        // Act
        var config = settings.ToConfig();

        // Assert
        Assert.Equal(AppSettings.DefaultSleep, config.Sleep);
        Assert.Equal(AppSettings.DefaultTimeout, config.Timeout);
        Assert.Equal(AppSettings.DefaultExpect, config.ExpectedStatusCodes);
        Assert.False(config.BaseUrl.HasValue);
    }

    [Fact]
    public void ToConfig_UsesRandomSleepWithinConfiguredExclusiveRange()
    {
        // Arrange
        var settings = new AppSettings { Sleep = [25, 26] };

        // Act
        var config = settings.ToConfig();

        // Assert
        Assert.Equal(25u, config.Sleep);
    }
}
