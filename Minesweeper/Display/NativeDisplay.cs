﻿using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Minesweeper.Display;

public sealed class NativeDisplay : IRenderer
{
    public bool Modified { get; set; }
    
    private readonly SafeFileHandle _fileHandle;
    
    private readonly CharInfo[] _buffer;
    private DisplayRect _screenRect;
    private readonly Coord _displaySize;
    private readonly Coord _startPos = new() {X = 0, Y = 0};
    
    public NativeDisplay(int width, int height)
    {
        _fileHandle = CreateFile("CONOUT$",
            0x40000000,
            2,
            IntPtr.Zero,
            FileMode.Open,
            0,
            IntPtr.Zero);
        
        if (_fileHandle.IsInvalid) throw new IOException("Console buffer file is invalid");
        
        _buffer = new CharInfo[width * height];
        _displaySize = new Coord(width, height);
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = (short) width, Bottom = (short) height};   
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        var bufferIndex = posY * _displaySize.X + posX;
        
        var cfg = FromColor(fg);
        var cbg = FromColor(bg);

        var color = (short) ((int) cfg | (int) cbg << 4);
        var symbolInfo = _buffer[bufferIndex];

        if (symbolInfo.Symbol == symbol && symbolInfo.Color == color) return;

        _buffer[bufferIndex].Symbol = (byte) symbol;
        _buffer[bufferIndex].Color = color;

        Modified = true;
    }

    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        var bufferIndex = posY * _displaySize.X + posX;
        
        if (_buffer[bufferIndex].Symbol == (byte) ' ' && _buffer[bufferIndex].Color == 15) return;

        _buffer[bufferIndex].Symbol = (byte) ' ';
        _buffer[bufferIndex].Color = 0;

        Modified = true;
    }
    
    public void Draw()
    {
        WriteConsoleOutput(_fileHandle, _buffer, _displaySize, _startPos, ref _screenRect);
    }
    
    private static ConsoleColor FromColor(Color c)
    {
        var index = c.R > 128 | c.G > 128 | c.B > 128 ? 8 : 0; // Bright bit
        index |= c.R > 64 ? 4 : 0; // Red bit
        index |= c.G > 64 ? 2 : 0; // Green bit
        index |= c.B > 64 ? 1 : 0; // Blue bit
        return (ConsoleColor) index;
    }
    
    #region NativeMetods
    
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeFileHandle CreateFile(string fileName,
        [MarshalAs(UnmanagedType.U4)] uint fileAccess,
        [MarshalAs(UnmanagedType.U4)] uint fileShare,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] int flags,
        IntPtr template);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteConsoleOutput(
        SafeFileHandle hConsoleOutput,
        CharInfo[] lpBuffer,
        Coord dwBufferSize,
        Coord dwBufferCoord,
        ref DisplayRect lpWriteRegion);

    [StructLayout(LayoutKind.Explicit)]
    private struct CharInfo
    {
        [FieldOffset(0)] public byte Symbol;
        [FieldOffset(2)] public short Color;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct DisplayRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    #endregion
}


[StructLayout(LayoutKind.Sequential)]
public struct Coord : IEquatable<Coord>
{
    public short X;
    public short Y;

    public Coord(short x, short y)
    {
        X = x;
        Y = y;
    }
    
    public Coord(int x, int y)
    {
        if (x > short.MaxValue || y > short.MaxValue)
        {
            throw new ArgumentException("Position argument is invalid (over the short max value)");
        }

        X = (short) x;
        Y = (short) y;
    }

    public override string ToString() => $"({X} {Y})";
    
    public static Coord operator +(Coord a, Coord b) => new((short) (a.X + b.X), (short) (a.Y + b.Y));
    
    public static Coord operator -(Coord a, Coord b) => new((short) (a.X - b.X), (short) (a.Y - b.Y));

    public bool Equals(Coord other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => obj is Coord other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Coord left, Coord right) => left.Equals(right);

    public static bool operator !=(Coord left, Coord right) => !(left == right);
}