namespace DotnetPing.Ping;

public class Url(string? url, Url? baseUrl = null)
{
    public bool HasValue => !string.IsNullOrWhiteSpace(Value);

    public string Value { get; } = EnsureUrl(url, baseUrl);

    public override string ToString() => Value.ToString();

    public static readonly Url Empty = new(string.Empty);

    private static string EnsureUrl(string? url, Url? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var isUrlAbsolute = Uri.IsWellFormedUriString(url, UriKind.Absolute);
        if (isUrlAbsolute)
        {
            return url;
        }

        if (baseUrl?.HasValue ?? false)
        {
            return $"{baseUrl.Value.TrimEnd('/')}/{url.TrimStart('/')}";
        }

        var urlResult = $"https://{url}";

        return Uri.IsWellFormedUriString(urlResult, UriKind.Absolute) ? urlResult : string.Empty;
    }
}
