namespace DotnetPing.Ping;

using System.Linq;

public readonly struct PingContext(IReadOnlyList<UrlConfig> urls, bool useMinimal, bool useDebug)
{
    public void Abc()
    {
    }
    public UrlConfig[] Urls { get; } =
        urls
        .Where(x => x.Url.HasValue)
        .DistinctBy(x => new { x.Url.Value, x.Method.Method })
        .ToArray();

    public bool UseMinimal { get; } = useMinimal;

    public bool UseDebug { get; } = useDebug;
}
