using System.Drawing;

namespace Minesweeper.UI;

public static class ColorExtensions
{
    private const string StringFormat = "\x1b[{0};2;{1};{2};{3}m{4}\x1b[0m";

    private static string ColorFormat(string input, Color fg, Color bg)
    {
        var s = string.Format(StringFormat, "38", fg.R, fg.G, fg.B, input);
        return string.Format(StringFormat, "48", bg.R, bg.G, bg.B, s);
    }

    public static string Color(this string input, Color fg, Color bg)
    {
        return ColorFormat(input, fg, bg);
    }
}