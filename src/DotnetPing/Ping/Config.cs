﻿namespace DotnetPing.Ping;

public record Config
{
    public Config(
        string? baseUrl = null,
        uint? sleep = null,
        uint? timeout = null,
        uint[]? expectedStatusCodes = null)
    {
        Sleep = sleep > 0 ? sleep.Value : AppSettings.DefaultSleep;
        Timeout = timeout > 0 ? timeout.Value : AppSettings.DefaultTimeout;

        ExpectedStatusCodes = EnsureExpectedStatusCodes(expectedStatusCodes ?? []);
        BaseUrl = new Url(baseUrl);
    }

    public Url BaseUrl { get; }

    public uint[] ExpectedStatusCodes { get; }

    public uint Sleep { get; }

    public uint Timeout { get; }

    private static uint[] EnsureExpectedStatusCodes(uint[] expectedStatusCodes)
    {
        var statusCodes = expectedStatusCodes.Where(x => x > 100 && x < 600).ToArray();

        return statusCodes.Length > 0 ? statusCodes : AppSettings.DefaultExpect;
    }
}
