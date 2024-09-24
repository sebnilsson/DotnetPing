using System.Text.Json;
using Spectre.Console;

namespace DotnetPing.Config;

public class FileConfigReader : IConfigReader
{
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
                AnsiConsole.WriteLine($"Error reading config file at path '{filePath}'.");
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
