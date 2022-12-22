using System.Text;

namespace Minesweeper.ConsoleDisplay;

public static class Colors
{
    public static IEnumerable<Color> Gradient(Color start, Color end, int steps)
    {
        var stepA = (end.A - start.A) / (steps - 1);
        var stepR = (end.R - start.R) / (steps - 1);
        var stepG = (end.G - start.G) / (steps - 1);
        var stepB = (end.B - start.B) / (steps - 1);

        for (var i = 0; i < steps; i++)
        {
            yield return Color.FromArgb(start.A + stepA * i,
                start.R + stepR * i,
                start.G + stepG * i,
                start.B + stepB * i);
        }
    }

    public static Color Brighter(this Color color, int brighteningPercent = 20)
    {
        var factor = 100 + brighteningPercent;

        var a = Math.Min(color.A * factor / 100, 255);
        var r = Math.Min(color.R * factor / 100, 255);
        var g = Math.Min(color.G * factor / 100, 255);
        var b = Math.Min(color.B * factor / 100, 255);
        
        return Color.FromArgb(a, r, g, b);
    }

    public static Color Dimmer(this Color color, int dimmingPercent = 20) => Brighter(color, -dimmingPercent);

    public static void AppendToBuilder(this Color color, StringBuilder builder) => 
        builder.Append($"\x1b[38;2;{color.R};{color.G};{color.B}m");
    
    public static void AppendToBuilderBg(this Color color, StringBuilder builder) => 
        builder.Append($"\x1b[48;2;{color.R};{color.G};{color.B}m");

    public static ConsoleColor ConsoleColor(this Color color)
    {
        var index = color.R > 128 | color.G > 128 | color.B > 128 ? 8 : 0; // Bright bit
        index |= color.R > 64 ? 4 : 0; // Red bit
        index |= color.G > 64 ? 2 : 0; // Green bit
        index |= color.B > 64 ? 1 : 0; // Blue bit
        return (ConsoleColor) index;
    }

}