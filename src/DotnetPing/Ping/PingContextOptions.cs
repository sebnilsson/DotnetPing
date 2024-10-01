namespace DotnetPing.Ping;

public record PingContextOptions
{
    public bool UseMinimal { get; init; }
    public Action<string, Exception>? OnReaderError { get; init; }
    public Action<string>? OnReaderRead { get; init; }
}
