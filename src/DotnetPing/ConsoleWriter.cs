using System.Text.Json;
using DotnetPing.Ping;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;

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

        var header = new Rule("Context used")
        {
            Border = BoxBorder.Double,
            Style = new Style(Color.Blue)
        };

        var json = GetJson(context);
        var jsonText = new JsonText(json);

        AnsiConsole.Write(header);
        AnsiConsole.Write(jsonText);
        AnsiConsole.WriteLine();
    }

    public static void WriteOnResultStarted(UrlConfig url, PingContext context)
    {
        if (context.UseMinimal)
        {
            return;
        }

        var text = new Text(
            $"{url.Method}: {url.Url} (" +
            (context.UseDebug ? $"Timeout: {url.Config.Timeout}, Sleep: {url.Config.Sleep}, " : string.Empty) +
            $"Expect: {GetJson(url.Config.ExpectedStatusCodes)})" +
            Environment.NewLine);

        AnsiConsole.Write(text);
    }

    public static void WriteOnResultCompleted(PingResult result, PingContext context)
    {
        if (context.UseMinimal)
        {
            return;
        }

        var style = result.Result switch
        {
            PingResultType.Success => new Style(Color.Green),
            PingResultType.Failure => new Style(Color.Red),
            PingResultType.Timeout => new Style(Color.Yellow),
            _ => new Style(Color.Default)
        };

        var text = new Text(
            $"{result.HttpStatusCode}: {result.Url} (Elapsed: {result.Elapsed.TotalSeconds.ToString("##0.00")}s)" +
            $"{(result.Result == PingResultType.Failure ? " [Failed]" : null)}" +
            $"{(result.Result == PingResultType.Timeout ? " [Timeout]" : null)}" +
            Environment.NewLine,
            style);

        AnsiConsole.Write(text);
    }

    public static void WriteResults(PingResult[] results, PingContext context)
    {
        var table = new Table();

        table.AddColumn("Tests");
        table.AddColumn(new TableColumn(new Text("Success", new Style(Color.Green))));
        table.AddColumn(new TableColumn(new Text("Failed", new Style(Color.Red))));
        table.AddColumn(new TableColumn(new Text("Timeouts", new Style(Color.Yellow))));

        var successResults = results.Where(x => x.Result == PingResultType.Success).ToArray();
        var failureResults = results.Where(x => x.Result == PingResultType.Failure).ToArray();
        var timeoutResults = results.Where(x => x.Result == PingResultType.Timeout).ToArray();

        table.AddRow(
            $"{results.Length}",
            $"[green]{successResults.Length}[/]",
            $"[red]{failureResults.Length}[/]",
            $"[yellow]{timeoutResults.Length}[/]");

        AnsiConsole.Write(table);

        if (failureResults.Any() || context.UseDebug)
        {
            WriteResultsTable("Failed", Color.Red, failureResults, includeExpected: true);
        }

        if (timeoutResults.Any() || context.UseDebug)
        {
            WriteResultsTable("Timeouts", Color.Yellow, timeoutResults);
        }

        if ((successResults.Any() && !context.UseMinimal) || context.UseDebug)
        {
            WriteResultsTable("Success", Color.Green, successResults);
        }
    }

    private static void WriteResultsTable(string title, Color keyColor, PingResult[] results, bool includeExpected = false)
    {
        var header = new Rule($"{title} ({results.Length})")
        {
            Border = BoxBorder.Double,
            Style = new Style(decoration: Decoration.Bold)
        };

        AnsiConsole.Write(header);

        var table = new Table();
        table.BorderColor(keyColor);

        table.AddColumn("Method");
        table.AddColumn("Url", x => x.NoWrap = true);
        table.AddColumn("Status");
        table.AddColumn("Elapsed");
        if (includeExpected)
        {
            table.AddColumn("Expected");
        }

        foreach (var result in results)
        {
            string[] values = [
                result.Method,
                result.Url,
                result.HttpStatusCode.ToString(),
                $"{result.Elapsed.TotalSeconds.ToString("# ##0.00")}s"
            ];

            var texts = values.Select(x => new Text(x)).ToArray();

            string[] additionValues = includeExpected ? [GetJson(result.ExpectedStatusCodes)] : [];

            var additionalTexts = additionValues.Select(x => new Text(x)).ToArray();

            IRenderable[] columns = [.. texts, .. additionalTexts];

            table.AddRow(columns);
        }

        AnsiConsole.Write(table);
    }

    private static string GetJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, s_jsonSerializerOptions);
    }
}
