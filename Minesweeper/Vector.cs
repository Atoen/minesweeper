﻿using System.Diagnostics.Contracts;

namespace Minesweeper;

public record struct Vector(int X, int Y)
{
    public static Vector Zero => new(0, 0);

    public static Vector Up => new(0, -1);
    public static Vector Down => new(0, 1);
    public static Vector Left => new(-1, 0);
    public static Vector Right => new(1, 0);

    [Pure]
    public Vector ExpandTo(Vector other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y));
    
    public static implicit operator Vector((int x, int y) v) => new(v.x, v.y);

    public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y);

    public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector operator /(Vector c, int i) => new(c.X / i, c.Y / i);
    
    public static Vector operator *(Vector c, int i) => new(c.X * i, c.Y * i);

    public static Vector operator -(Vector a) => new(-a.X, -a.Y);

    public static Vector operator +(Vector a) => a;

    public override string ToString() => $"({X} {Y})";
}
