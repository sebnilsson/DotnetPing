using DotnetPing.Config;

namespace DotnetPing.Ping;

public class PingContextBuilder(IConfigReader configReader)
{
    public async Task<PingContext> Build(AppSettings settings)
    {
        var urls = settings.Urls.Select(url => GetUrlConfig(url, settings)).ToList();

        if (!urls.Any())
        {
            var configFilePath = GetConfigFilePath(settings);

            var config = await configReader.Read(configFilePath, useMinimal: settings.Minimal);

            var configUrls = config.GetUrlConfigs().ToList();

            urls.AddRange(configUrls);
        }

        return new PingContext(urls, useMinimal: settings.Minimal, useDebug: settings.Debug);
    }

    private static string GetConfigFilePath(AppSettings settings)
    {
        return Path.IsPathFullyQualified(settings.Config)
            ? settings.Config
            : Path.Combine(AppContext.BaseDirectory, settings.Config);
    }

    private static UrlConfig GetUrlConfig(string url, AppSettings settings)
    {
        var sleep = GetSleep(settings);

        return new UrlConfig(url, sleep: sleep, timeout: settings.Timeout, expectedStatusCodes: settings.Expect);
    }

    private static uint GetSleep(AppSettings settings)
    {
        var randomSleepMin = (int)settings.Sleep.ElementAtOrDefault(0);
        var randomSleepMax = (int)settings.Sleep.ElementAtOrDefault(1);

        randomSleepMin = randomSleepMin > 0 ? randomSleepMin : (int)AppSettings.DefaultSleep;

        return (uint)(randomSleepMax > 0 ? Random.Shared.Next(randomSleepMin, randomSleepMax) : randomSleepMin);
    }
}
