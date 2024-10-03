using Spectre.Console;
using Spectre.Console.Rendering;

namespace DotnetPing.Output;

public static class ConsoleWriter
{
    public static IRenderable GetHeader(string text, Color? foreground = null)
    {
        return new Rule(text)
        {
            Style = new Style(foreground, decoration: Decoration.Bold),
            Border = BoxBorder.Double,
        };
    }

}
