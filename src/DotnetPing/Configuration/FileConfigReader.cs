using System.Text.Json;

namespace DotnetPing.Configuration;

public class FileConfigReader : IConfigReader
{
    public event EventHandler<string>? OnError;

    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public async Task<FileConfigJson> Read(string filePath, bool useMinimal)
    {
        try
        {
            return await ReadInternal(filePath);
        }
        catch (Exception)
        {
            if (!useMinimal)
            {
                OnError?.Invoke(this, filePath);
            }
            return FileConfigJson.Empty;
        }
    }

    private static async Task<FileConfigJson> ReadInternal(string filePath)
    {
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

        return
            await JsonSerializer.DeserializeAsync<FileConfigJson>(fileStream, s_jsonSerializerOptions)
            ?? FileConfigJson.Empty;

    }
}
