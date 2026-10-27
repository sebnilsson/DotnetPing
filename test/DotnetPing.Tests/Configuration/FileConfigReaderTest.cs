using DotnetPing.Configuration;
using DotnetPing.Ping;

namespace DotnetPing.Tests.Configuration;

public class FileConfigReaderTest : IDisposable
{
    private readonly string _tempDir;

    public FileConfigReaderTest()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "DotnetPingTests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }
        catch
        {
            // best-effort cleanup
        }
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Read_MissingFile_ReturnsEmptyConfig()
    {
        // Arrange
        var reader = new FileConfigReader();

        // Act
        var result = await reader.Read(Path.Combine(_tempDir, "does-not-exist.json"));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Urls);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public async Task Read_ValidFile_DeserialisesUrlsAndGroups()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "ping.json");
        await File.WriteAllTextAsync(path, """
        {
            "urls": [
                { "url": "https://example.com", "method": "POST", "timeout": 1500, "sleep": 100, "expect": [200, 201] }
            ],
            "groups": [
                { "baseUrl": "https://learn.microsoft.com/", "urls": ["/dotnet"], "expect": [200] }
            ]
        }
        """, TestContext.Current.CancellationToken);

        var reader = new FileConfigReader();

        // Act
        var result = await reader.Read(path);

        // Assert
        var url = Assert.Single(result.Urls);
        Assert.Equal("https://example.com", url.Url);
        Assert.Equal("POST", url.Method);
        Assert.Equal(1_500u, url.Timeout);
        Assert.Equal(100u, url.Sleep);
        Assert.Equal(new uint[] { 200, 201 }, url.Expect);

        var group = Assert.Single(result.Groups);
        Assert.Equal("https://learn.microsoft.com/", group.BaseUrl);
        Assert.Equal(["/dotnet"], group.Urls);
    }

    [Fact]
    public async Task Read_FileWithCommentsAndTrailingCommas_IsTolerated()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "ping.jsonc");
        await File.WriteAllTextAsync(path, """
        {
            // top-level
            "urls": [
                { "url": "https://example.com", },
            ],
        }
        """, TestContext.Current.CancellationToken);

        var reader = new FileConfigReader();

        // Act
        var result = await reader.Read(path);

        // Assert
        Assert.Single(result.Urls);
    }

    [Fact]
    public async Task Read_NullJson_ReturnsEmptyConfig()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "null.json");
        await File.WriteAllTextAsync(path, "null", TestContext.Current.CancellationToken);

        var reader = new FileConfigReader();

        // Act
        var result = await reader.Read(path);

        // Assert
        Assert.Empty(result.Urls);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public async Task Read_InvalidJson_InvokesOnReaderErrorAndReturnsEmpty()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "broken.json");
        await File.WriteAllTextAsync(path, "{ this is not valid json", TestContext.Current.CancellationToken);

        string? errorPath = null;
        Exception? errorException = null;

        var options = new PingContextOptions
        {
            UseMinimal = false,
            OnReaderError = (filePath, ex) =>
            {
                errorPath = filePath;
                errorException = ex;
            }
        };

        var reader = new FileConfigReader();

        // Act
        var result = await reader.Read(path, options);

        // Assert
        Assert.Empty(result.Urls);
        Assert.Empty(result.Groups);
        Assert.Equal(path, errorPath);
        Assert.NotNull(errorException);
    }

    [Fact]
    public async Task Read_InvalidJson_WithMinimal_DoesNotInvokeOnReaderError()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "broken.json");
        await File.WriteAllTextAsync(path, "{ broken", TestContext.Current.CancellationToken);

        var invoked = false;
        var options = new PingContextOptions
        {
            UseMinimal = true,
            OnReaderError = (_, _) => invoked = true
        };

        var reader = new FileConfigReader();

        // Act
        var result = await reader.Read(path, options);

        // Assert
        Assert.Empty(result.Urls);
        Assert.False(invoked);
    }

    [Fact]
    public async Task Read_ValidFile_InvokesOnReaderRead()
    {
        // Arrange
        var path = Path.Combine(_tempDir, "ping.json");
        await File.WriteAllTextAsync(path, "{}", TestContext.Current.CancellationToken);

        string? readPath = null;
        var options = new PingContextOptions
        {
            OnReaderRead = filePath => readPath = filePath
        };

        var reader = new FileConfigReader();

        // Act
        await reader.Read(path, options);

        // Assert
        Assert.Equal(path, readPath);
    }

    [Fact]
    public async Task Read_MissingFile_DoesNotInvokeOnReaderRead()
    {
        // Arrange
        var invoked = false;
        var options = new PingContextOptions
        {
            OnReaderRead = _ => invoked = true
        };

        var reader = new FileConfigReader();

        // Act
        await reader.Read(Path.Combine(_tempDir, "missing.json"), options);

        // Assert
        Assert.False(invoked);
    }
}
