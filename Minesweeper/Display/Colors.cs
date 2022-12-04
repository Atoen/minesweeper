namespace Minesweeper.Display;

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
}