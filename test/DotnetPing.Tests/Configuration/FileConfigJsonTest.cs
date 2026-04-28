using DotnetPing.Configuration;

namespace DotnetPing.Tests.Configuration;

public class FileConfigJsonTest
{
    [Fact]
    public void Empty_IsEmpty()
    {
        var empty = FileConfigJson.Empty;

        Assert.Empty(empty.Urls);
        Assert.Empty(empty.Groups);
        Assert.Empty(empty.GetUrlConfigs());
    }

    [Fact]
    public void GetUrlConfigs_FlattensTopLevelUrls()
    {
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

        var configs = json.GetUrlConfigs().ToArray();

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

        var configs = json.GetUrlConfigs().ToArray();

        Assert.Equal(2, configs.Length);
        Assert.All(configs, c => Assert.Equal(HttpMethod.Get, c.Method));
        Assert.All(configs, c => Assert.Equal(250u, c.Config.Sleep));
        Assert.All(configs, c => Assert.Equal(5_000u, c.Config.Timeout));
    }

    [Fact]
    public void GetUrlConfigs_HandlesMixedUrlsAndGroups()
    {
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

        var configs = json.GetUrlConfigs().ToArray();

        Assert.Equal(3, configs.Length);
    }
}
