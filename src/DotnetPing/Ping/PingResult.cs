namespace DotnetPing.Ping;

public record PingResult(bool IsSuccess, uint HttpStatusCode, string Url, Exception? Exception);
