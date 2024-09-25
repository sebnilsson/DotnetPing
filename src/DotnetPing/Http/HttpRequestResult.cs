namespace DotnetPing.Http;

public record HttpRequestResult(uint HttpStatusCode, bool IsTimeout = false, Exception? Exception = null);
