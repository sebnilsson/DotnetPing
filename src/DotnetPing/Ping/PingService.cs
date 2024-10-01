using System.Diagnostics;
using DotnetPing.Http;

namespace DotnetPing.Ping;

public class PingService(PingContextBuilder pingContextBuilder, IHttpRequester httpRequester)
{
    public async Task<PingResult[]> Run(AppSettings settings)
    {
        Stopwatch contextTimer = Stopwatch.StartNew();

        var contextOptions = new PingContextOptions
        {
            UseMinimal = settings.Minimal,
            OnReaderError = OnReaderError,
            OnReaderRead = OnReaderRead
        };

        var context = await pingContextBuilder.Build(settings, contextOptions);

        contextTimer.Stop();

        ConsoleWriter.WriteContext(context, contextTimer);

        Stopwatch resultsTimer = Stopwatch.StartNew();

        var results = await GetResults(context);

        resultsTimer.Stop();

        ConsoleWriter.WriteResults(results, context, resultsTimer);

        return results;

        void OnReaderError(string filePath, Exception ex) => ConsoleWriter.WriteOnConfigReaderError(filePath, ex, settings);

        void OnReaderRead(string filePath) => ConsoleWriter.WriteOnConfigReaderRead(filePath, settings);
    }

    private async Task<PingResult[]> GetResults(PingContext context)
    {
        if (context.Urls.Length == 0)
        {
            return [];
        }

        var tasks = new List<Task<PingResult>>(context.Urls.Length);

        var lastUrl = context.Urls.LastOrDefault();

        foreach (var url in context.Urls)
        {
            var task = GetResult(url, context);
            tasks.Add(task);

            if (url != lastUrl)
            {
                await Task.Delay((int)url.Config.Sleep);
            }
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToArray();
    }

    private async Task<PingResult> GetResult(UrlConfig url, PingContext context)
    {
        ConsoleWriter.WriteOnResultStarted(url, context);

        Stopwatch stopwatch = Stopwatch.StartNew();

        var result = await httpRequester.Get(url, context);

        stopwatch.Stop();

        var isSuccess = result.HttpStatusCode > 0 && url.Config.ExpectedStatusCodes.Contains(result.HttpStatusCode);

        var pingResult = new PingResult(isSuccess, stopwatch.Elapsed, result, url);

        ConsoleWriter.WriteOnResultCompleted(pingResult, context);

        return pingResult;
    }
}
