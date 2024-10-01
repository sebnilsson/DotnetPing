using System.Text.Json;
using DotnetPing.Ping;

namespace DotnetPing.Configuration;

public class FileConfigReader : IConfigReader
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public async Task<FileConfigJson> Read(string filePath, PingContextOptions? options = null)
    {
        try
        {
            return await ReadInternal(filePath, options);
        }
        catch (Exception ex)
        {
            if (options?.UseMinimal == false)
            {
                options?.OnReaderError?.Invoke(filePath, ex);
            }
            return FileConfigJson.Empty;
        }
    }

    private static async Task<FileConfigJson> ReadInternal(string filePath, PingContextOptions? options)
    {
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

        options?.OnReaderRead?.Invoke(filePath);

        return
            await JsonSerializer.DeserializeAsync<FileConfigJson>(fileStream, s_jsonSerializerOptions)
            ?? FileConfigJson.Empty;

    }
}
