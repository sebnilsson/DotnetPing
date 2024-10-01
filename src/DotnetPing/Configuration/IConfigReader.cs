using DotnetPing.Ping;

namespace DotnetPing.Configuration;

public interface IConfigReader
{
    Task<FileConfigJson> Read(string filePath, PingContextOptions? options = null);
}
