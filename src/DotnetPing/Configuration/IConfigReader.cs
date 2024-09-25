namespace DotnetPing.Configuration;

public interface IConfigReader
{
    Task<FileConfigJson> Read(string filePath, bool useMinimal);
}
