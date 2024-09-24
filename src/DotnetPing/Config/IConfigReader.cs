namespace DotnetPing.Config;

public interface IConfigReader
{
    Task<FileConfigJson> Read(string filePath, bool useMinimal);
}
