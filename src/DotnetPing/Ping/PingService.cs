using DotnetPing.Http;

namespace DotnetPing.Ping;

public class PingService(IHttpRequester httpRequester)
{
    public async Task<PingResult[]> Run(PingContext context)
    {
        var tasks = new List<Task<PingResult>>(context.Urls.Length);

        var lastUrl = context.Urls.LastOrDefault();

        foreach (var url in context.Urls)
        {
            var task = GetResult(url, context);
            tasks.Add(task);

            if (url != lastUrl)
            {
                await Task.Delay((int)url.Sleep);
            }
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToArray();
    }

    private async Task<PingResult> GetResult(UrlConfig url, PingContext context)
    {
        var result = await httpRequester.Get(url, context);

        var isSuccess = url.ExpectedStatusCodes.Contains(result.HttpStatusCode);

        return new PingResult(isSuccess, result.HttpStatusCode, url.Url, result.Exception);
    }
}
