namespace DotnetPing.Http;

public record HttpResult(
    string Url,
    uint HttpStatusCode,
    bool IsTimeout = false,
    Exception? Exception = null);
