using DotnetPing.Ping;

namespace DotnetPing.Configuration;

public record FileConfigJson
{
    public UrlJson[] Urls { get; init; } = [];

    public GroupJson[] Groups { get; init; } = [];

    public record UrlJson : Config
    {
        public string Url { get; init; } = string.Empty;
    }

    public record GroupJson : Config
    {
        public string[] Urls { get; init; } = [];
    };

    public static FileConfigJson Empty => new();

    public IEnumerable<UrlConfig> GetUrlConfigs()
    {
        foreach (var url in Urls ?? [])
        {
            yield return new UrlConfig(url.Url, url);
        }

        foreach (var group in Groups ?? [])
        {
            foreach (var url in group.Urls ?? [])
            {
                yield return new UrlConfig(url, group);
            }
        }
    }
}
