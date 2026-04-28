using DotnetPing.Configuration;
using DotnetPing.Ping;

namespace DotnetPing.Tests.Ping;

public class PingContextBuilderTest
{
    private sealed class StubConfigReader(FileConfigJson result) : IConfigReader
    {
        public string? LastPath { get; private set; }

        public PingContextOptions? LastOptions { get; private set; }

        public Task<FileConfigJson> Read(string filePath, PingContextOptions? options = null)
        {
            LastPath = filePath;
            LastOptions = options;
            return Task.FromResult(result);
        }
    }

    [Fact]
    public async Task Build_UsesSettingsUrls_WhenProvided_AndDoesNotInvokeReader()
    {
        var reader = new StubConfigReader(FileConfigJson.Empty);
        var builder = new PingContextBuilder(reader);

        var settings = new AppSettings
        {
            Urls = ["https://example.com", "example.org"],
            Method = "POST",
            BaseUrl = string.Empty
        };

        var context = await builder.Build(settings);

        Assert.Equal(2, context.Urls.Length);
        Assert.All(context.Urls, u => Assert.Equal(HttpMethod.Post, u.Method));
        Assert.Null(reader.LastPath);
    }

    [Fact]
    public async Task Build_AppliesBaseUrl_FromSettings()
    {
        var reader = new StubConfigReader(FileConfigJson.Empty);
        var builder = new PingContextBuilder(reader);

        var settings = new AppSettings
        {
            Urls = ["/dotnet"],
            BaseUrl = "https://learn.microsoft.com/"
        };

        var context = await builder.Build(settings);

        var only = Assert.Single(context.Urls);
        Assert.Equal("https://learn.microsoft.com/dotnet", only.Url.Value);
    }

    [Fact]
    public async Task Build_FallsBackToConfigReader_WhenNoUrlsProvided()
    {
        var reader = new StubConfigReader(new FileConfigJson
        {
            Urls = [new FileConfigJson.UrlJson { Url = "https://example.com" }]
        });
        var builder = new PingContextBuilder(reader);

        var settings = new AppSettings { Urls = [] };

        var context = await builder.Build(settings);

        var only = Assert.Single(context.Urls);
        Assert.Equal("https://example.com", only.Url.Value);
        Assert.NotNull(reader.LastPath);
    }

    [Fact]
    public async Task Build_UsesAbsoluteConfigPath_WhenProvided()
    {
        var reader = new StubConfigReader(FileConfigJson.Empty);
        var builder = new PingContextBuilder(reader);

        var absolute = Path.Combine(Path.GetTempPath(), "config.json");
        var settings = new AppSettings { Urls = [], Config = absolute };

        await builder.Build(settings);

        Assert.Equal(absolute, reader.LastPath);
    }

    [Fact]
    public async Task Build_ResolvesRelativeConfigPath_AgainstCurrentDirectory()
    {
        var reader = new StubConfigReader(FileConfigJson.Empty);
        var builder = new PingContextBuilder(reader);

        var settings = new AppSettings { Urls = [], Config = "custom.json" };

        await builder.Build(settings);

        Assert.Equal(Path.Combine(Environment.CurrentDirectory, "custom.json"), reader.LastPath);
    }

    [Fact]
    public async Task Build_DefaultsToPingJson_WhenConfigOmitted()
    {
        var reader = new StubConfigReader(FileConfigJson.Empty);
        var builder = new PingContextBuilder(reader);

        var settings = new AppSettings { Urls = [], Config = string.Empty };

        await builder.Build(settings);

        Assert.Equal(Path.Combine(Environment.CurrentDirectory, AppSettings.DefaultConfig), reader.LastPath);
    }

    [Fact]
    public async Task Build_PassesContextOptions_ThroughToReader()
    {
        var reader = new StubConfigReader(FileConfigJson.Empty);
        var builder = new PingContextBuilder(reader);

        var options = new PingContextOptions { UseMinimal = true };

        await builder.Build(new AppSettings { Urls = [] }, options);

        Assert.Same(options, reader.LastOptions);
    }
}
