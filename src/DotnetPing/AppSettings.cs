using System.ComponentModel;
using System.Net;
using DotnetPing.Ping;
using Spectre.Console.Cli;

namespace DotnetPing;

public class AppSettings : CommandSettings
{
    public static readonly uint[] DefaultExpect = [(uint)HttpStatusCode.OK];

    public const string DefaultMethod = "GET";

    public const string DefaultConfig = "ping.json";

    public const uint DefaultSleep = 1_000;

    public const uint DefaultTimeout = 15_000;

    [Description(AppSettingsConfig.UrlsDescription)]
    [CommandArgument(0, "[urls]")]
    public string[] Urls { get; init; } = [];

    [Description(AppSettingsConfig.ConfigDescription)]
    [CommandOption("-b|--base-url")]
    public string BaseUrl { get; init; } = string.Empty;

    [Description(AppSettingsConfig.ConfigDescription)]
    [CommandOption("-c|--config")]
    public string Config { get; init; } = string.Empty;

    [Description(AppSettingsConfig.DebugDescription)]
    [CommandOption("-d|--debug")]
    public bool Debug { get; init; }

    [Description(AppSettingsConfig.ExpectDescription)]
    [CommandOption("-e|--expect")]
    public uint[] Expect { get; init; } = DefaultExpect;

    [Description(AppSettingsConfig.MethodDescription)]
    [CommandOption("-X|--request")]
    public string Method { get; init; } = DefaultMethod;

    [Description(AppSettingsConfig.MinimalDescription)]
    [CommandOption("-m|--minimal")]
    public bool Minimal { get; init; }

    [Description(AppSettingsConfig.SleepDescription)]
    [CommandOption("-s|--sleep")]
    public uint[] Sleep { get; init; } = [DefaultSleep];

    [Description(AppSettingsConfig.TimeoutDescription)]
    [CommandOption("-t|--timeout")]
    public uint Timeout { get; init; } = DefaultTimeout;

    public Config ToConfig()
    {
        var sleep = GetSleep();

        return new Config(
            baseUrl: BaseUrl,
            sleep: sleep,
            timeout: Timeout,
            expectedStatusCodes: Expect);
    }

    private uint GetSleep()
    {
        var randomSleepMin = (int)Sleep.ElementAtOrDefault(0);
        var randomSleepMax = (int)Sleep.ElementAtOrDefault(1);

        randomSleepMin = randomSleepMin > 0 ? randomSleepMin : (int)DefaultSleep;

        return (uint)(randomSleepMax > 0 ? Random.Shared.Next(randomSleepMin, randomSleepMax) : randomSleepMin);
    }
}

internal static class AppSettingsConfig
{
    public const string ConfigDescription = "The path to the JSON config file.";

    public const string ExpectDescription = "Sets the expected status code of requests. Default: 200.";

    public const string DebugDescription = "Use debug console messaging.";

    public const string MethodDescription = "Sets the request method. Default: GET.";

    public const string MinimalDescription = "Use minimal console messaging.";

    public const string SleepDescription = "Sets the sleep time between requests in milliseconds. Default: 1000ms.";

    public const string TimeoutDescription = "Sets the timeout for requests in milliseconds. Default: 15000ms. If two values are provided, a random number between the two numbers will be generated for each request.";

    public const string UrlsDescription = "The URLs to ping. If not specified, the URLs are read from the JSON config file.";
}
