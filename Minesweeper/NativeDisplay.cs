using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Minesweeper.UI;

namespace Minesweeper;

internal sealed class NativeDisplay : IDisplayProvider
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }

    private readonly SafeFileHandle _fileHandle;
    
    private CharInfo[] _buffer;
    private DisplayRect _screenRect;
    private Coord _screenSize;
    private readonly Coord _startPos = new() {X = 0, Y = 0};

    private bool _modified;

    private readonly List<IRenderable> _renderables = new();
    private readonly List<IRenderable> _addedRenderables = new();

    public NativeDisplay(int width, int height)
    {
        Width = (short) width;
        Height = (short) height;
        
        _fileHandle = CreateFile("CONOUT$",
            0x40000000,
            2,
            IntPtr.Zero,
            FileMode.Open,
            0,
            IntPtr.Zero);
        
        if (_fileHandle.IsInvalid) throw new IOException("Console buffer file is invalid");
        
        _buffer = new CharInfo[Width * Height];
        _screenSize = new Coord(Width, Height);
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = Width, Bottom = Height};   
    }
    
    public void Draw()
    {
        foreach (var renderable in _renderables.Where(renderable => renderable.ShouldRemove))
        {
            renderable.Clear();
        }
        
        _renderables.RemoveAll(r => r.ShouldRemove);

        if (_renderables.Any(r => r.ShouldRemove)) throw new Exception();
        
        foreach (var renderable in _renderables)
        {
            renderable.Render();
        }
        
        if (_modified)
        {
            WriteConsoleOutput(_fileHandle, _buffer, _screenSize, _startPos, ref _screenRect);
            _modified = false;
        }
        
        if (_addedRenderables.Count == 0) return;

        _renderables.AddRange(_addedRenderables);
        _addedRenderables.Clear();
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        var bufferIndex = posY * Width + posX;
        
        var cfg = FromColor(fg);
        var cbg = FromColor(bg);

        var color = (short) ((int) cfg | (int) cbg << 4);
        var symbolInfo = _buffer[bufferIndex];

        if (symbolInfo.Symbol == symbol && symbolInfo.Color == color) return;

        _buffer[bufferIndex].Symbol = (byte) symbol;
        _buffer[bufferIndex].Color = color;

        _modified = true;
    }

    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        var bufferIndex = posY * Width + posX;
        
        if (_buffer[bufferIndex].Symbol == (byte) ' ' && _buffer[bufferIndex].Color == 15) return;

        _buffer[bufferIndex].Symbol = (byte) ' ';
        _buffer[bufferIndex].Color = 0;

        _modified = true;
    }

    public void Print(int posX, int posY, string text, Color fg,
        Color bg, Alignment alignment = Alignment.Center)
    {
        var startX = alignment switch
        {
            Alignment.Left => posX - text.Length,
            Alignment.Right => posX,
            _ => posX - text.Length / 2
        };
            
        var endX = startX + text.Length;

        for (int x = startX - posX, i = 0; x < endX - posX; x++, i++)
        {
            Draw(posX + x, posY, text[i], fg, bg);
        }
    }

    public void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            Draw(pos.X + x, pos.Y + y, symbol, color, color);
        }
    }

    public void ClearRect(Coord pos, Coord size)
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            ClearAt(pos.X + x, pos.Y + y);
        }
    }

    private static ConsoleColor FromColor(Color c) {
        var index = c.R > 128 | c.G > 128 | c.B > 128 ? 8 : 0; // Bright bit
        index |= c.R > 64 ? 4 : 0; // Red bit
        index |= c.G > 64 ? 2 : 0; // Green bit
        index |= c.B > 64 ? 1 : 0; // Blue bit
        return (ConsoleColor) index;
    }
    
    public void AddToRenderList(IRenderable renderable) => _addedRenderables.Add(renderable);

    // [SupportedOSPlatform("windows")]
    // private void CheckBufferSize()
    // {
    //     var windowHeight = Console.WindowHeight;
    //
    //     if (windowHeight != Console.BufferHeight && windowHeight > 0)
    //     {
    //         Console.BufferHeight = windowHeight;
    //     }
    // }
    //
    // [SupportedOSPlatform("windows")]
    // internal void SetSize(int width, int height)
    // {
    //     if (width > Console.LargestWindowWidth) width = Console.LargestWindowWidth;
    //     if (height > Console.LargestWindowHeight) height = Console.LargestWindowHeight;
    //
    //     Width = (short) width;
    //     Height = (short) height;
    //     
    //     Console.SetWindowSize(width, height);
    //     Console.SetBufferSize(width, height);
    //         
    //     ResizeBuffer();
    //     
    // }
    //
    // private void ResizeBuffer() => ResizeBuffer(new Coord(Width, Height));
    //
    // private void ResizeBuffer(Coord size)
    // {
    //     if (size.X < 15) size.X = 15;
    //     else if (size.X > Console.LargestWindowWidth) size.X = (short) Console.LargestWindowWidth;
    //     
    //     if (size.Y < 1) size.Y = 1;
    //     else if (size.Y > Console.LargestWindowWidth) size.Y = (short) Console.LargestWindowHeight;
    //
    //     Width = size.X;
    //     Height = size.Y;
    //     
    //     _buffer = new CharInfo[Width * Height];
    //     _screenSize = new Coord {X = Width, Y = Height};
    //     _screenRect = new DisplayRect {Left = 0, Top = 0, Right = Width, Bottom = Height};
    //     
    //     OnResize?.Invoke();
    // }
    //
    // public void Draw(Coord pos, char symbol, Color foreground, Color background) =>
    //     Draw(pos.X, pos.Y, symbol, foreground, background);
    //
    // public void Draw(Coord pos, TileDisplay tileDisplay) =>
    //     Draw(pos.X, pos.Y, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);
    //
    // public void Draw(int posX, int posY, TileDisplay tileDisplay) =>
    //     Draw(posX, posY, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

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
