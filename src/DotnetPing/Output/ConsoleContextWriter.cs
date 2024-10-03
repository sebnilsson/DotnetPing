using System.Diagnostics;
using DotnetPing.Ping;
using Spectre.Console;
using Spectre.Console.Json;

namespace DotnetPing.Output;

public static class ConsoleContextWriter
{
    public static void Start(AppSettings settings)
    {
        if (!settings.Debug)
        {
            return;
        }

        var header = ConsoleWriter.GetHeader("Context", ConsoleColor.Debug);

        AnsiConsole.Write(header);
    }

    public static void Done(PingContext context, Stopwatch contextTimer)
    {
        if (!context.UseDebug)
        {
            return;
        }

        var json = JsonUtility.Serialize(context);
        var jsonText = new JsonText(json);
        var jsonPanel = new Panel(jsonText)
            .Header("Context used")
            .RoundedBorder()
            .BorderColor(ConsoleColor.Debug);

        AnsiConsole.Write(jsonPanel);

        var text = new Text($"Context created with {context.Urls.Length} URLs " +
            $"in {contextTimer.Elapsed.TotalSeconds:##0.00}s", new Style(ConsoleColor.Debug));

        AnsiConsole.Write(text);
        AnsiConsole.WriteLine();
    }
}
