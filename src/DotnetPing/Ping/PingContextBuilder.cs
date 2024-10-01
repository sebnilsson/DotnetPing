using DotnetPing.Configuration;

namespace DotnetPing.Ping;

public class PingContextBuilder(IConfigReader configReader)
{
    public async Task<PingContext> Build(AppSettings settings, PingContextOptions? options = null)
    {
        var urls = settings.Urls.Select(url => GetUrlConfig(url, settings)).ToList();

        if (urls.Count == 0)
        {
            var configFilePath = GetConfigFilePath(settings);

            var config = await configReader.Read(configFilePath, options);

            var configUrls = config.GetUrlConfigs();

            urls.AddRange(configUrls);
        }

        return new PingContext(urls, useMinimal: settings.Minimal, useDebug: settings.Debug);
    }

    private static string GetConfigFilePath(AppSettings settings)
    {
        if (Path.IsPathFullyQualified(settings.Config))
        {
            return settings.Config;
        }

        string path = !string.IsNullOrWhiteSpace(settings.Config) ? settings.Config : AppSettings.DefaultConfig;

        return Path.Combine(Environment.CurrentDirectory, path);
    }

    private static UrlConfig GetUrlConfig(string url, AppSettings settings)
    {
        var config = settings.ToConfig();

        return new UrlConfig(url, settings.Method, config);
    }
}
