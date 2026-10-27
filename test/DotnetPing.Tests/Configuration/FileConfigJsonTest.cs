using DotnetPing.Configuration;

namespace DotnetPing.Tests.Configuration;

public class FileConfigJsonTest
{
    [Fact]
    public void Empty_IsEmpty()
    {
        // Arrange
        var empty = FileConfigJson.Empty;

        // Act
        var configs = empty.GetUrlConfigs().ToArray();

        // Assert
        Assert.Empty(empty.Urls);
        Assert.Empty(empty.Groups);
        Assert.Empty(configs);
    }

    [Fact]
    public void GetUrlConfigs_FlattensTopLevelUrls()
    {
        // Arrange
        var json = new FileConfigJson
        {
            Urls =
            [
                new FileConfigJson.UrlJson
                {
                    Url = "https://example.com",
                    Method = "POST",
                    Sleep = 100,
                    Timeout = 1_500,
                    Expect = [200, 201]
                }
            ]
        };

        // Act
        var configs = json.GetUrlConfigs().ToArray();

        // Assert
        var single = Assert.Single(configs);
        Assert.Equal("https://example.com", single.Url.Value);
        Assert.Equal(HttpMethod.Post, single.Method);
        Assert.Equal(100u, single.Config.Sleep);
        Assert.Equal(1_500u, single.Config.Timeout);
        Assert.Equal(new uint[] { 200, 201 }, single.Config.ExpectedStatusCodes);
    }

    [Fact]
    public void GetUrlConfigs_ExpandsGroupUrlsWithSharedSettings()
    {
        // Arrange
        var json = new FileConfigJson
        {
            Groups =
            [
                new FileConfigJson.GroupJson
                {
                    Method = "GET",
                    Urls = ["https://a.example.com", "https://b.example.com"],
                    Sleep = 250,
                    Timeout = 5_000,
                    Expect = [200]
                }
            ]
        };

        // Act
        var configs = json.GetUrlConfigs().ToArray();

        // Assert
        Assert.Equal(2, configs.Length);
        Assert.All(configs, c => Assert.Equal(HttpMethod.Get, c.Method));
        Assert.All(configs, c => Assert.Equal(250u, c.Config.Sleep));
        Assert.All(configs, c => Assert.Equal(5_000u, c.Config.Timeout));
        Assert.All(configs, c => Assert.Equal(new uint[] { 200 }, c.Config.ExpectedStatusCodes));
    }

    [Fact]
    public void GetUrlConfigs_ExpandsGroupUrlsWithBaseUrl()
    {
        // Arrange
        var json = new FileConfigJson
        {
            Groups =
            [
                new FileConfigJson.GroupJson
                {
                    BaseUrl = "https://learn.microsoft.com/",
                    Urls = ["/dotnet", "dotnet/csharp"]
                }
            ]
        };

        // Act
        var configs = json.GetUrlConfigs().ToArray();

        // Assert
        Assert.Collection(
            configs,
            first => Assert.Equal("https://learn.microsoft.com/dotnet", first.Url.Value),
            second => Assert.Equal("https://learn.microsoft.com/dotnet/csharp", second.Url.Value));
    }

    [Fact]
    public void GetUrlConfigs_HandlesMixedUrlsAndGroups()
    {
        // Arrange
        var json = new FileConfigJson
        {
            Urls =
            [
                new FileConfigJson.UrlJson { Url = "https://example.com" }
            ],
            Groups =
            [
                new FileConfigJson.GroupJson { Urls = ["https://a.example.com", "https://b.example.com"] }
            ]
        };

        // Act
        var configs = json.GetUrlConfigs().ToArray();

        // Assert
        Assert.Equal(3, configs.Length);
    }

    [Fact]
    public void GetUrlConfigs_ToleratesNullCollectionsFromJson()
    {
        // Arrange
        var json = new FileConfigJson
        {
            Urls = null!,
            Groups =
            [
                new FileConfigJson.GroupJson { Urls = null! }
            ]
        };

        // Act
        var configs = json.GetUrlConfigs().ToArray();

        // Assert
        Assert.Empty(configs);
    }
}
