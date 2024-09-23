namespace DotnetPing;

internal static class AppCommandConfig
{
    public const string UrlsDescription = "The URLs to ping. If not specified, the URLs are read from the ping.json file.";

    public const string BackoffDescription = "The initial backoff time in milliseconds.";

    public const string BackoffMaxDescription = "Sets the max backoff time in milliseconds. Uses the backoff time as start value and this value as end value for a random number.";

    public const string ConfigDescription = "The path to the ping.json file";
}
