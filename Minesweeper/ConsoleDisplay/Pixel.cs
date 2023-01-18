using Minesweeper.Game;

namespace Minesweeper.ConsoleDisplay;

public struct Pixel : IEquatable<Pixel>
{
    private Pixel(char symbol, TextMode mode, Color fg, Color bg)
    {
        Symbol = symbol;
        Mode = mode;
        Fg = fg;
        Bg = bg;
    }

    private const char ClearedSymbol = ' ';

    public static readonly Pixel Empty = new('\0', TextMode.Default, Color.Empty, Color.Empty);
    public static readonly Pixel Cleared = new(ClearedSymbol, TextMode.Default, Color.Empty, Color.Empty);

    public TextMode Mode;
    public char Symbol;
    
    public Color Fg = Color.Empty;
    public Color Bg = Color.Empty;

    public override string ToString()
    {
        return $"\x1b[38;2;{Fg.R};{Fg.G};{Fg.B}m\x1b[48;2;{Bg.R};{Bg.G};{Bg.B}m{Symbol}";
    }

    public bool IsEmpty => Symbol == '\0' && Fg == Color.Empty && Bg == Color.Empty;
    public bool IsCleared => Symbol == ClearedSymbol && Fg == Color.Empty && Bg == Color.Empty;

    public static implicit operator Pixel(TileDisplay tileDisplay) =>
        new(tileDisplay.Symbol, TextMode.Default, tileDisplay.Foreground, tileDisplay.Background);

    public static bool operator ==(Pixel a, Pixel b)
    {
        return a.Fg == b.Fg && a.Bg == b.Bg && a.Symbol == b.Symbol && a.Mode == b.Mode;
    }

    public static bool operator !=(Pixel a, Pixel b) => !(a == b);

    public override int GetHashCode() => HashCode.Combine(Symbol, Fg, Bg);

    public bool Equals(Pixel other)
    {
        return Mode == other.Mode && Symbol == other.Symbol && Fg.Equals(other.Fg) && Bg.Equals(other.Bg);
    }

    public override bool Equals(object? obj) => obj is Pixel other && Equals(other);
}

[Flags]
public enum TextMode
{
    Default = 0,
    Bold = 1,
    Underline = 1 << 1,
    Italic = 1 << 2,
    Strikethrough = 1 << 3,
    DoubleUnderline = 1 << 4,
    Overline = 1 << 5
}


