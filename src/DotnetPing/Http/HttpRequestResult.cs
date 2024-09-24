namespace DotnetPing.Http;

public record HttpRequestResult(uint HttpStatusCode, Exception? Exception = null);
