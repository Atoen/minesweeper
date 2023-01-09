using System.Diagnostics.Contracts;

namespace Minesweeper;

public record struct Coord(int X, int Y)
{
    public static Coord Zero => new(0, 0);
    public static Coord Up => new(0, -1);
    public static Coord Down => new(0, 1);
    public static Coord Left => new(-1, 0);
    public static Coord Right => new(1, 0);

    [Pure]
    public Coord ExpandTo(Coord other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y));
    
    public static implicit operator Coord((int x, int y) v) => new(v.x, v.y);

    public static Coord operator +(Coord a, Coord b) => new(a.X + b.X, a.Y + b.Y);

    public static Coord operator -(Coord a, Coord b) => new(a.X - b.X, a.Y - b.Y);

    public static Coord operator /(Coord c, int i) => new(c.X / i, c.Y / i);
    
    public static Coord operator *(Coord c, int i) => new(c.X * i, c.Y * i);

    public static Coord operator -(Coord a) => new(-a.X, -a.Y);

    public static Coord operator +(Coord a) => a;

    public override string ToString() => $"({X} {Y})";
}
