using System.Diagnostics;
using DotnetPing.Http;
using DotnetPing.Output;

namespace DotnetPing.Ping;

public class PingService(PingContextBuilder pingContextBuilder, IHttpRequester httpRequester)
{
    public async Task<PingResults> Run(AppSettings settings)
    {
        PingContext context = await GetContext(pingContextBuilder, settings);

        PingResults results = await GetResults(context);

        return results;
    }

    private async Task<PingContext> GetContext(PingContextBuilder pingContextBuilder, AppSettings settings)
    {
        ConsoleContextWriter.Start(settings);

        var contextTimer = Stopwatch.StartNew();

        var contextOptions = new PingContextOptions
        {
            UseMinimal = settings.Minimal,
            OnReaderError = OnReaderError,
            OnReaderRead = OnReaderRead
        };

        var context = await pingContextBuilder.Build(settings, contextOptions);

        contextTimer.Stop();

        ConsoleContextWriter.Done(context, contextTimer);

        return context;

        void OnReaderError(string filePath, Exception ex) => ConsoleConfigWriter.WriteOnReaderError(filePath, ex, settings);

        void OnReaderRead(string filePath) => ConsoleConfigWriter.WriteOnReaderRead(filePath, settings);
    }

    private async Task<PingResults> GetResults(PingContext context)
    {
        ConsoleResultsWriter.Start(context);

        var resultsTimer = Stopwatch.StartNew();

        var results = await GetPingResults(context);

        resultsTimer.Stop();

        ConsoleResultsWriter.Done(results, context, resultsTimer);

        return results;
    }

    private async Task<PingResults> GetPingResults(PingContext context)
    {
        if (context.Urls.Length == 0)
        {
            return PingResults.Empty;
        }

        var tasks = new List<Task<PingResult>>(context.Urls.Length);

        var lastUrl = context.Urls.LastOrDefault();

        foreach (var url in context.Urls)
        {
            var task = GetPingResult(url, context);
            tasks.Add(task);

            if (url != lastUrl)
            {
                await Task.Delay((int)url.Config.Sleep);
            }
        }

        await Task.WhenAll(tasks);

        var results = tasks.Select(x => x.Result).ToArray();

        return new PingResults(results);
    }

    private async Task<PingResult> GetPingResult(UrlConfig url, PingContext context)
    {
        ConsoleResultWriter.WriteOnStarted(url, context);

        Stopwatch stopwatch = Stopwatch.StartNew();

        var result = await httpRequester.Get(url, context);

        stopwatch.Stop();

        var isSuccess = result.HttpStatusCode > 0 && url.Config.ExpectedStatusCodes.Contains(result.HttpStatusCode);

        var pingResult = new PingResult(isSuccess, stopwatch.Elapsed, result, url);

        ConsoleResultWriter.WriteOnCompleted(pingResult, context);

        return pingResult;
    }
}
