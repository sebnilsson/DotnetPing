namespace DotnetPing.Configuration;

public interface IConfigReader
{
    event EventHandler<string>? OnError;

    Task<FileConfigJson> Read(string filePath, bool useMinimal);
}
