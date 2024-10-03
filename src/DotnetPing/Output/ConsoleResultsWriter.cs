using System.Diagnostics;
using DotnetPing.Ping;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DotnetPing.Output;

public static class ConsoleResultsWriter
{
    public static void Start(PingContext context)
    {
        if (!context.UseDebug)
        {
            return;
        }

        var header = ConsoleWriter.GetHeader($"Ping ({context.Urls.Length})");

        AnsiConsole.Write(header);
    }

    public static void Done(PingResults results, PingContext context, Stopwatch resultsTimer)
    {
        WriteResultsTables(results, context);

        WriteResultsSummary(results, context, resultsTimer);
    }

    private static void WriteResultsTables(PingResults results, PingContext context)
    {
        if (results.All.Length == 0)
        {
            return;
        }

        if ((results.Successes.Length > 0 && !context.UseMinimal) || context.UseDebug)
        {
            WriteResultsTable("Success", ConsoleColor.Success, results.Successes);
        }

        if ((results.Failures.Length > 0 && !context.UseMinimal) || context.UseDebug)
        {
            WriteResultsTable("Failed", ConsoleColor.Error, results.Failures);
        }

        if ((results.Timeouts.Length > 0 && !context.UseMinimal) || context.UseDebug)
        {
            WriteResultsTable("Timeouts", ConsoleColor.Timeout, results.Timeouts, includeExpected: false);
        }
    }

    private static void WriteResultsSummary(PingResults results, PingContext context, Stopwatch resultsTimer)
    {
        if (context.UseDebug)
        {
            var header = ConsoleWriter.GetHeader($"Summary");
            AnsiConsole.Write(header);
        }

        var resultTypeText = results.ResultType switch
        {
            PingResultType.Success => "succeeded",
            PingResultType.Failure => "failed",
            PingResultType.Timeout => "timeout",
            _ => "done"
        };

        var resultColor = ConsoleColor.GetFromResultType(results.ResultType);

        var summaryText = new Text(
            $"Ping(s) {resultTypeText} in {resultsTimer.Elapsed.TotalSeconds:##0.00}s",
            new Style(resultColor, decoration: Decoration.Bold));

        if (!context.UseDebug && !context.UseMinimal)
        {
            AnsiConsole.WriteLine();
        }

        AnsiConsole.Write(summaryText);
        AnsiConsole.WriteLine();

        var prefix = !context.UseMinimal ? "    " : string.Empty;
        var separator = !context.UseMinimal ? string.Empty : ", ";

        Text[] resultDetails = [
            new Text($"{prefix}Total: {results.All.Length}{separator}"),
            new Text($"{prefix}Success: {results.Successes.Length}{separator}", new Style(ConsoleColor.Success)),
            new Text($"{prefix}Failed: {results.Failures.Length}{separator}", new Style(ConsoleColor.Error)),
            new Text($"{prefix}Timeouts: {results.Timeouts.Length}", new Style(ConsoleColor.Timeout)),
        ];

        foreach (var details in resultDetails)
        {
            AnsiConsole.Write(details);

            if (!context.UseMinimal)
            {
                AnsiConsole.WriteLine();
            }
        }

        //if (context.UseMinimal)
        //{
        //    return;
        //}

        //var table = new Table();
        //table.RoundedBorder();

        //table.AddColumn("Tests");
        //table.AddColumn(new TableColumn(new Text("Success", new Style(ConsoleColor.Success))));
        //table.AddColumn(new TableColumn(new Text("Failed", new Style(ConsoleColor.Error))));
        //table.AddColumn(new TableColumn(new Text("Timeouts", new Style(ConsoleColor.Timeout))));

        //table.AddRow(
        //    $"{results.All.Length}",
        //    $"[green]{results.Successes.Length}[/]",
        //    $"[red]{results.Failures.Length}[/]",
        //    $"[yellow]{results.Timeouts.Length}[/]");

        //AnsiConsole.Write(table);
    }

    private static void WriteResultsTable(string title, Color keyColor, PingResult[] results, bool includeExpected = true)
    {
        var header = ConsoleWriter.GetHeader($"{title} ({results.Length})");

        AnsiConsole.Write(header);

        var table = new Table();
        table.BorderColor(keyColor);
        table.RoundedBorder();

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
                $"{result.Elapsed.TotalSeconds:##0.00}s"
            ];

            var texts = values.Select(x => new Text(x)).ToArray();

            string[] additionValues =
                includeExpected ? [JsonUtility.SerializeCompact(result.ExpectedStatusCodes)] : [];

            var additionalTexts = additionValues.Select(x => new Text(x)).ToArray();

            IRenderable[] columns = [.. texts, .. additionalTexts];

            table.AddRow(columns);
        }

        AnsiConsole.Write(table);
    }
}
