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
    Console.ResetColor();
}

static void AppConfig(IConfigurator config)
{
    config.SetApplicationName("dotnet-guid");

    // TODO: Add examples
    //config.AddExample(["5", "-f", "N"]);
    //config.AddExample(["-f", "X", "-u"]);
    //config.AddExample(["-f", "B64"]);
    //config.AddExample(["-e"]);

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
