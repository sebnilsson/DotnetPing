using DotnetPing.Http;

namespace DotnetPing.Ping;

public record PingResult
{
    public PingResult(bool isSuccess, TimeSpan elapsed, HttpResult result, UrlConfig url)
    {
        Elapsed = elapsed;
        Exception = result.Exception;
        ExpectedStatusCodes = url.Config.ExpectedStatusCodes;
        HttpStatusCode = result.HttpStatusCode;
        Method = url.Method;
        Url = url.Url;

        Result = isSuccess ? PingResultType.Success : result.IsTimeout ? PingResultType.Timeout : PingResultType.Failure;
    }

    public TimeSpan Elapsed { get; }

    public Exception? Exception { get; }

    public uint[] ExpectedStatusCodes { get; }

    public uint HttpStatusCode { get; }

    public PingResultType Result { get; }

    public string Method { get; }

    public string Url { get; }
}

public enum PingResultType
{
    Success,
    Failure,
    Timeout
}
