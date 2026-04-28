namespace DotnetPing.Tests;

public class AppSettingsTest
{
    [Fact]
    public void ToConfig_AppliesSettings()
    {
        var settings = new AppSettings
        {
            BaseUrl = "https://example.com",
            Sleep = [123],
            Timeout = 4_321,
            Expect = [200, 404]
        };

        var config = settings.ToConfig();

        Assert.Equal("https://example.com", config.BaseUrl.Value);
        Assert.Equal(123u, config.Sleep);
        Assert.Equal(4_321u, config.Timeout);
        Assert.Equal(new uint[] { 200, 404 }, config.ExpectedStatusCodes);
    }

    [Fact]
    public void ToConfig_DefaultsApplyWhenSettingsAreEmpty()
    {
        var settings = new AppSettings { Sleep = [] };

        var config = settings.ToConfig();

        Assert.Equal(AppSettings.DefaultSleep, config.Sleep);
        Assert.Equal(AppSettings.DefaultTimeout, config.Timeout);
        Assert.Equal(AppSettings.DefaultExpect, config.ExpectedStatusCodes);
        Assert.False(config.BaseUrl.HasValue);
    }
}
