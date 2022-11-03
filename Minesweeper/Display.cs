using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Minesweeper.UI;

namespace Minesweeper;

public static class Display
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }
    
    public static int RefreshRate { get; set; }
    
    private static SafeFileHandle _fileHandle = null!;
    
    private static CharInfo[] _buffer = null!;
    private static DisplayRect _screenRect;
    private static Coord _screenSize;
    private static readonly Coord StartPos = new() {X = 0, Y = 0};

    private static bool _modified;
    private static bool _refreshing;

    private static readonly List<IRenderable> Renderables = new();
    private static readonly List<IRenderable> AddedRenderables = new();

    [SupportedOSPlatform("windows")]
    internal static void Init(short width, short height, int refreshRate = 20)
    {
        if (width < 40) width = 40;
        if (height < 20) height = 20;

        Width = width;
        Height = height;
        RefreshRate = refreshRate;

        // Displaying stuff on the screen
        _buffer = new CharInfo[Width * Height];
        _screenSize = new Coord {X = Width, Y = Height};
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = Width, Bottom = Height};
        
        Console.Title = "Minesweeper";
        Console.SetWindowSize(Width, Height);
        Console.SetBufferSize(Width, Height);
        Console.CursorVisible = false;

        // file handle for faster console printing
        _fileHandle = CreateFile("CONOUT$",
            0x40000000,
            2,
            IntPtr.Zero,
            FileMode.Open,
            0,
            IntPtr.Zero);

        if (_fileHandle.IsInvalid) throw new IOException("Console buffer file is invalid");
        
        new Thread(Start).Start();
    }

    [SupportedOSPlatform("windows")]
    internal static void Start()
    {
        Input.WindowEvent += delegate(Input.WindowState state) { ResizeBuffer(state.Size); };

        var tickLenght = 1000 / RefreshRate; // ms
        var stopwatch = new Stopwatch();

        _refreshing = true;

        while (_refreshing)
        {
            stopwatch.Start();

            Renderables.RemoveAll(r => r.ShouldRemove);
            
            foreach (var renderable in Renderables)
            {
                renderable.Render();
            }
            
            if (_modified)
            {
                WriteConsoleOutput(_fileHandle, _buffer, _screenSize, StartPos, ref _screenRect);
                _modified = false;
            }
            
            Renderables.AddRange(AddedRenderables);
            AddedRenderables.Clear();
            
            CheckBufferSize();

            stopwatch.Stop();
            var sleepTime = tickLenght - (int) stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            
            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }

    internal static void Stop() => _refreshing = false;

    [SupportedOSPlatform("windows")]
    private static void CheckBufferSize()
    {
        var windowHeight = Console.WindowHeight;

        if (windowHeight != Console.BufferHeight && windowHeight > 0)
        {
            Console.BufferHeight = windowHeight;
        }
    }

    [SupportedOSPlatform("windows")]
    internal static void SetSize(int width, int height)
    {
        if (width > Console.LargestWindowWidth) width = Console.LargestWindowWidth;
        if (height > Console.LargestWindowHeight) height = Console.LargestWindowHeight;
        
        Console.SetWindowSize(width, height);
    }

    private static void ResizeBuffer(Coord size)
    {
        if (size.X < 15) size.X = 15;
        else if (size.X > Console.LargestWindowWidth) size.X = (short) Console.LargestWindowWidth;
        
        if (size.Y < 1) size.Y = 1;
        else if (size.Y > Console.LargestWindowWidth) size.Y = (short) Console.LargestWindowHeight;

        Width = size.X;
        Height = size.Y;
        
        _buffer = new CharInfo[Width * Height];
        _screenSize = new Coord {X = Width, Y = Height};
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = Width, Bottom = Height};
    }

    internal static void AddToRenderList(IRenderable renderable) => AddedRenderables.Add(renderable);

    internal static void Update()
    {
        if (!_modified) return;
    
        WriteConsoleOutput(_fileHandle, _buffer, _screenSize, StartPos, ref _screenRect);
        _modified = false;
    }

    public static void Draw(Coord pos, char symbol, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black) =>
        Draw(pos.X, pos.Y, symbol, foreground, background);

    public static void Draw(Coord pos, TileDisplay tileDisplay) =>
        Draw(pos.X, pos.Y, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

    public static void Draw(int posX, int posY, TileDisplay tileDisplay) =>
        Draw(posX, posY, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

    public static void Draw(int posX, int posY, char symbol, ConsoleColor foreground, ConsoleColor background)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        var bufferIndex = posY * Width + posX;

        var color = (short) ((int) foreground | (int) background << 4);
        var symbolInfo = _buffer[bufferIndex];

        if (symbolInfo.Symbol == symbol && symbolInfo.Color == color) return;

        _buffer[bufferIndex].Symbol = (byte) symbol;
        _buffer[bufferIndex].Color = color;

        _modified = true;
    }

    public static void ClearAt(Coord pos) => ClearAt(pos.X, pos.Y);
    
    public static void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        var bufferIndex = posY * Width + posX;

        _buffer[bufferIndex].Symbol = (byte) ' ';
        _buffer[bufferIndex].Color = 15;

        _modified = true;
    }

    public static void Print(int posX, int posY, string text, ConsoleColor foreground = ConsoleColor.White,
        ConsoleColor background = ConsoleColor.Black, Alignment alignment = Alignment.Center)
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
            Draw(posX + x, posY, text[i], foreground, background);
        }
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
