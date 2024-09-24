using System.Diagnostics.CodeAnalysis;
using DotnetPing.Config;
using DotnetPing.Http;
using DotnetPing.Ping;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace DotnetPing;

public sealed class AppCommand : AsyncCommand<AppSettings>
{
    public const int COMMAND_LINE_SUCCESS = 0;

    public const int COMMAND_LINE_ERROR = 1;

    public const int COMMAND_LINE_USAGE_ERROR = 64;

    public override async Task<int> ExecuteAsync(
        [NotNull] CommandContext commandContext,
        [NotNull] AppSettings settings)
    {
        var services = GetServices(settings);

        var builder = services.GetRequiredService<PingContextBuilder>();

        var context = await builder.Build(settings);

        if (!context.Urls.Any())
        {
            return COMMAND_LINE_USAGE_ERROR;
        }

        var service = services.GetRequiredService<PingService>();

        var results = await service.Run(context);

        var isAllSuccess = results.All(x => x.IsSuccess);

        if (!isAllSuccess)
        {
            return COMMAND_LINE_ERROR;
        }

        return COMMAND_LINE_SUCCESS;
    }

    private static ServiceProvider GetServices(AppSettings settings)
    {
        var services = new ServiceCollection();

        services.ConfigureHttpClientDefaults(config =>
        {
            config.ConfigureHttpClient(configClient =>
            {
                configClient.Timeout = TimeSpan.FromMilliseconds(settings.Timeout);
            });
        });

        services.AddSingleton<PingContextBuilder>();
        services.AddSingleton<PingService>();
        services.AddSingleton<IConfigReader, FileConfigReader>();
        services.AddSingleton<IHttpRequester, HttpRequester>();

        return services.BuildServiceProvider();
    }
}
