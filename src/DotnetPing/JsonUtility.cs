using System.Text.Json;

namespace DotnetPing;

public static class JsonUtility
{
    private static readonly JsonSerializerOptions s_compactJsonSerializerOptions = new()
    {
        WriteIndented = false
    };

    public static string SerializeCompact<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, s_compactJsonSerializerOptions);
    }

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}
