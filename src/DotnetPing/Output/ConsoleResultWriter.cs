using DotnetPing.Ping;
using Spectre.Console;

namespace DotnetPing.Output;

public static class ConsoleResultWriter
{
    public static void WriteOnStarted(UrlConfig url, PingContext context)
    {
        if (context.UseMinimal)
        {
            return;
        }

        var text = new Text(
            $"{url.Method}: {url.Url} (" +
            (context.UseDebug ? $"Timeout: {url.Config.Timeout}, Sleep: {url.Config.Sleep}, " : string.Empty) +
            $"Expect: {JsonUtility.SerializeCompact(url.Config.ExpectedStatusCodes)})" +
            Environment.NewLine);

        AnsiConsole.Write(text);
    }

    public static void WriteOnCompleted(PingResult result, PingContext context)
    {
        if (context.UseMinimal)
        {
            return;
        }

        var style = result.Result switch
        {
            PingResultType.Success => new Style(ConsoleColor.Success),
            PingResultType.Failure => new Style(ConsoleColor.Error),
            PingResultType.Timeout => new Style(ConsoleColor.Timeout),
            _ => new Style(ConsoleColor.Default)
        };

        var text = new Text(
            $"{result.HttpStatusCode}: {result.Url} (Elapsed: {result.Elapsed.TotalSeconds:##0.00}s)" +
            $"{(result.Result == PingResultType.Failure ? " [Failed]" : null)}" +
            $"{(result.Result == PingResultType.Timeout ? " [Timeout]" : null)}" +
            Environment.NewLine,
            style);

        AnsiConsole.Write(text);
    }
}
