namespace DotnetPing.Ping;

public readonly struct PingContext(IReadOnlyList<UrlConfig> urls, bool useMinimal, bool useDebug)
{
    public UrlConfig[] Urls { get; } =
        urls.Where(x => x.IsValidUrl).DistinctBy(x => x.Url, StringComparer.InvariantCultureIgnoreCase).ToArray();

    public bool UseMinimal { get; } = useMinimal;

    public bool UseDebug { get; } = useDebug;
}
