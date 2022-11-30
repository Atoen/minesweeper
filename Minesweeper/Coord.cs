using System.Diagnostics.Contracts;

namespace Minesweeper;

public struct Coord : IEquatable<Coord>
{
    public static readonly Coord Zero = new(0, 0);

    public static readonly Coord Up = new(0, -1);
    
    public static readonly Coord Down = new(0, 1);
    
    public static readonly Coord Left = new(-1, 0);
    
    public static readonly Coord Right = new(1, 0);
    
    public int X;
    public int Y;

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    [Pure]
    public Coord ExpandTo(Coord other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y));

    public static bool operator ==(Coord a, Coord b) => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(Coord a, Coord b) => !(a == b);
    
    public static Coord operator +(Coord a, Coord b) => new(a.X + b.X, a.Y + b.Y);

    public static Coord operator -(Coord a, Coord b) => new(a.X - b.X, a.Y - b.Y);

    public static Coord operator /(Coord c, int i) => new(c.X / i, c.Y / i);
    
    public static Coord operator *(Coord c, int i) => new(c.X * i, c.Y * i);

    public static Coord operator -(Coord a) => new(-a.X, -a.Y);

    public static Coord operator +(Coord a) => a;
    
    public bool Equals(Coord other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => obj is Coord other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"({X} {Y})";

}