using System.Diagnostics;
using DotnetPing;
using Spectre.Console;
using Spectre.Console.Cli;

Console.CancelKeyPress += OnCancelKeyPress;

var app = new CommandApp<AppCommand>();
app.Configure(AppConfig);

try
{
    return app.Run(args);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    return CommandLineExitCode.UnhandledException;
}
finally
{
    if (Debugger.IsAttached)
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to close application...");
        Console.ReadKey(intercept: true);
    }

    Console.ResetColor();
}

static void AppConfig(IConfigurator config)
{
    config.SetApplicationName("dotnet-ping");

    config.AddExample(["https://example.com"]);
    config.AddExample(["https://example.com", "other.com", "-s", "1000"]);
    config.AddExample(["https://example.com", "other.com", "-s", "1000", "-s", "2000", "-t", "5000"]);
    config.AddExample(["/about", "/contact", "-b", "https://example.com"]);
    config.AddExample(["-c", "ping.json"]);

#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
}

static void OnCancelKeyPress(
    object? sender,
    ConsoleCancelEventArgs e)
{
    Console.ResetColor();
}
