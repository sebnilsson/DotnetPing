using DotnetPing.Ping;

namespace DotnetPing.Config;

public record FileConfigJson(FileConfigJson.UrlJson[] Urls, FileConfigJson.GroupJson[] Groups)
{
    public record UrlJson(string Url, uint? Timeout, uint? Sleep, uint[]? Expect);

    public record GroupJson(uint? Timeout, uint? Sleep, uint[]? Expect, string[]? Urls);

    public static FileConfigJson Empty => new([], []);

    public IEnumerable<UrlConfig> GetUrlConfigs()
    {
        foreach (var url in Urls ?? [])
        {
            yield return new UrlConfig(url.Url, url.Sleep, url.Timeout, url.Expect ?? []);
        }

        foreach (var group in Groups ?? [])
        {
            foreach (var url in group.Urls ?? [])
            {
                yield return new UrlConfig(url, group.Sleep, group.Timeout, group.Expect ?? []);
            }
        }
    }
}
