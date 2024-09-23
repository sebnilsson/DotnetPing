using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace DotnetPing;

internal sealed class AppCommand : Command<AppCommand.Settings>
{
    public override int Execute(
        [NotNull] CommandContext context,
        [NotNull] Settings settings)
    {
        // TODO: Write code

        OnEnd();

        return 0;
    }

    private static void OnEnd()
    {
        if (Debugger.IsAttached)
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to close application...");
            Console.ReadKey(intercept: true);
        }

        Console.ResetColor();
    }
    public sealed class Settings : CommandSettings
    {
        [Description(AppCommandConfig.UrlsDescription)]
        [CommandArgument(0, "[urls]")]
        public string[] Urls { get; init; } = [];

        [Description(AppCommandConfig.BackoffDescription)]
        [CommandOption("-b|--backoff")]
        public int Backoff { get; init; } = 0;

        [Description(AppCommandConfig.BackoffMaxDescription)]
        [CommandOption("-m|--backoff-max")]
        public int BackoffMax { get; init; } = 0;

        [Description(AppCommandConfig.ConfigDescription)]
        [CommandOption("-c|--config")]
        public string Config { get; init; } = string.Empty;
    }
}
