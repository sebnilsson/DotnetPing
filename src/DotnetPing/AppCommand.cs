using System.Diagnostics.CodeAnalysis;
using DotnetPing.Configuration;
using DotnetPing.Http;
using DotnetPing.Ping;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace DotnetPing;

public sealed class AppCommand : AsyncCommand<AppSettings>
{
    public override async Task<int> ExecuteAsync(
        [NotNull] CommandContext commandContext,
        [NotNull] AppSettings settings)
    {
        var services = GetServices(settings);

        var service = services.GetRequiredService<PingService>();

        var results = await service.Run(settings);

        if (results.ResultType == PingResultType.None)
        {
            return CommandLineExitCode.UsageError;
        }

        if (results.ResultType != PingResultType.Success)
        {
            return CommandLineExitCode.Error;
        }

        return CommandLineExitCode.Success;
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
