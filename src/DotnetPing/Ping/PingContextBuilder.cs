using DotnetPing.Configuration;

namespace DotnetPing.Ping;

public class PingContextBuilder(IConfigReader configReader)
{
    public event EventHandler<string>? OnConfigReaderError;

    public async Task<PingContext> Build(AppSettings settings)
    {
        configReader.OnError += (sender, filePath) => OnConfigReaderError?.Invoke(this, filePath);

        var urls = settings.Urls.Select(url => GetUrlConfig(url, settings)).ToList();

        if (!urls.Any())
        {
            var configFilePath = GetConfigFilePath(settings);

            var config = await configReader.Read(configFilePath, useMinimal: settings.Minimal);

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

        return Path.Combine(AppContext.BaseDirectory, path);
    }

    private static UrlConfig GetUrlConfig(string url, AppSettings settings)
    {
        var config = settings.ToConfig();

        return new UrlConfig(url, config);
    }
}
