using Spectre.Console;

namespace DotnetPing.Output;

public static class ConsoleConfigWriter
{
    public static void WriteOnReaderRead(string filePath, AppSettings settings)
    {
        if (!settings.Debug)
        {
            return;
        }

        var text = new Text("Reading config file at path:", new Style(ConsoleColor.Debug));
        var path = new TextPath(filePath);

        AnsiConsole.Write(text);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();
    }

    public static void WriteOnReaderError(string filePath, Exception ex, AppSettings settings)
    {
        var text = new Text("Error reading config file at path:", new Style(ConsoleColor.Error));
        var path = new TextPath(filePath);

        AnsiConsole.Write(text);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();
    }



}
