using DotnetPing.Ping;

namespace DotnetPing.Configuration;

public record FileConfigJson
{
    public UrlJson[] Urls { get; init; } = [];

    public GroupJson[] Groups { get; init; } = [];

    public class UrlJson : JsonBase
    {
        public string Url { get; init; } = string.Empty;
    }

    public class GroupJson : JsonBase
    {
        public string[] Urls { get; init; } = [];
    }

    public abstract class JsonBase
    {
        public string BaseUrl { get; init; } = string.Empty;

        public uint[] Expect { get; init; } = [];

        public string Method { get; init; } = string.Empty;

        public uint Sleep { get; init; }

        public uint Timeout { get; init; }
    }

    public static FileConfigJson Empty => new();

    public IEnumerable<UrlConfig> GetUrlConfigs()
    {
        foreach (var url in Urls ?? [])
        {
            var config = GetConfig(url);
            yield return new UrlConfig(url.Url, url.Method, config);
        }

        foreach (var group in Groups ?? [])
        {
            foreach (var url in group.Urls ?? [])
            {
                var config = GetConfig(group);
                yield return new UrlConfig(url, group.Method, config);
            }
        }
    }

    private static Config GetConfig(JsonBase jsonBase)
    {
        return new(
            baseUrl: jsonBase.BaseUrl,
            sleep: jsonBase.Sleep,
            timeout: jsonBase.Timeout,
            expectedStatusCodes: jsonBase.Expect);
    }
}
