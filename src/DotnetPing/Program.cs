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
    return -99;
}
finally
{
    if (Debugger.IsAttached)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Press any key to close application...");
        Console.ReadKey(intercept: true);
    }

    Console.ResetColor();
}

static void AppConfig(IConfigurator config)
{
    config.SetApplicationName("dotnet-guid");

    // TODO: Update
    //config.AddExample(["https://dot.net"]);
    //config.AddExample(["https://dot.net https://www.nuget.org", "-b", "1000", "-m", "2000"]);
    //config.AddExample(["-c", "ping.json"]);

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
