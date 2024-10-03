using DotnetPing.Ping;
using Spectre.Console;

namespace DotnetPing.Output;

public static class ConsoleColor
{
    public static readonly Color Error = Color.Red;

    public static readonly Color Success = Color.Green;

    public static readonly Color Timeout = Color.Yellow;

    public static readonly Color Debug = Color.Blue;

    public static readonly Color Default = Color.Default;

    public static Color GetFromResultType(PingResultType type)
    {
        return type switch
        {
            PingResultType.Success => ConsoleColor.Success,
            PingResultType.Failure => ConsoleColor.Error,
            PingResultType.Timeout => ConsoleColor.Timeout,
            _ => ConsoleColor.Default
        };
    }
}
