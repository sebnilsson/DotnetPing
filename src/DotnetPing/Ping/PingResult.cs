using DotnetPing.Http;

namespace DotnetPing.Ping;

public record PingResult
{
    public PingResult(bool isSuccess, HttpRequestResult result, UrlConfig url)
    {
        IsSuccess = isSuccess;
        Exception = result.Exception;
        HttpStatusCode = result.HttpStatusCode;
        IsTimeout = result.IsTimeout;
        ExpectedStatusCodes = url.ExpectedStatusCodes;
        Url = url.Url;
    }

    public Exception? Exception { get; }

    public uint[] ExpectedStatusCodes { get; }

    public uint HttpStatusCode { get; }

    public bool IsSuccess { get; }

    public bool IsTimeout { get; }

    public string Url { get; }
}
