using System.Text.Json;
using DotnetPing.Http;
using DotnetPing.Ping;
using Spectre.Console;
using Spectre.Console.Json;

namespace DotnetPing;

public static class ConsoleWriter
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        WriteIndented = false
    };

    public static void WriteOnConfigReaderError(string filePath, AppSettings settings)
    {
        var path = new TextPath(filePath);

        AnsiConsole.WriteLine("Error reading config file at path:");
        AnsiConsole.Write(path);
        AnsiConsole.WriteLine();
    }

    public static void WriteContext(PingContext context)
    {
        if (!context.UseDebug)
        {
            return;
        }

        var json = GetJsonText(context);
        var text = new Markup("[blue]Context used[/]");

        AnsiConsole.Write(text);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(json);
        AnsiConsole.WriteLine();
    }

    public static void WriteOnResultStarted(UrlConfig url, PingContext context)
    {
        if (context.UseMinimal)
        {
            return;
        }

        var text = new Text(
            $"{url.Method}: {url.Url} (Timeout: {url.Timeout}, Sleep: {url.Sleep}, Expect: {GetJson(url.ExpectedStatusCodes)})",
            new Style(Color.Yellow));

        lock (AnsiConsole.Console)
        {
            AnsiConsole.Write(text);
            AnsiConsole.WriteLine();
        }
    }

    public static void WriteOnResultCompleted(HttpResult result, PingContext context)
    {
        if (context.UseMinimal)
        {
            return;
        }

        var text = new Markup($"[green]{result.HttpStatusCode}: {result.Url}{(result.IsTimeout ? " (Timeout)" : null)}[/]");

        lock (AnsiConsole.Console)
        {
            AnsiConsole.Write(text);
            AnsiConsole.WriteLine();
        }
    }
    public static void WriteResults(PingResult[] results, PingContext context)
    {
        var table = new Table();

        table.AddColumn("Tests");
        table.AddColumn("Success");
        table.AddColumn("Failed");
        table.AddColumn("Timeouts");

        var totalCount = results.Length;
        var successCount = results.Count(x => x.IsSuccess);
        var failedCount = results.Count(x => !x.IsSuccess);
        var timeoutCount = results.Count(x => x.IsTimeout);

        table.AddRow(
            $"{totalCount}",
            $"[green]{successCount}[/]",
            $"[red]{failedCount}[/]",
            $"[yellow]({timeoutCount})[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string GetJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, s_jsonSerializerOptions);
    }

    private static JsonText GetJsonText<T>(T obj)
    {
        var json = GetJson(obj);

        return new JsonText(json);
    }
}
