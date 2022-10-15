using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace Minesweeper;

public static class Display
{
    public const int WIDTH = 120;
    public const int HEIGHT = 40;
    
    private static SafeFileHandle _fileHandle = null!;

    private static readonly CharInfo[] Buffer = new CharInfo[WIDTH * HEIGHT];
    private static readonly Coord ScreenSize = new() {X = WIDTH, Y = HEIGHT};
    private static readonly Coord StartPos = new() {X = 0, Y = 0};

    private static DisplayRect _screenRect = new() {Left = 0, Top = 0, Right = WIDTH, Bottom = HEIGHT};

    private static bool _modified;

    [SupportedOSPlatform("windows")]
    internal static void Init()
    {
        Console.Title = "Minesweeper";
        Console.WindowWidth = WIDTH;
        Console.WindowHeight = HEIGHT;
        Console.SetBufferSize(WIDTH, HEIGHT);
        Console.CursorVisible = false;
        
        _fileHandle = CreateFile("CONOUT$",
            0x40000000,
            2,
            IntPtr.Zero,
            FileMode.Open,
            0,
            IntPtr.Zero);

        if (_fileHandle.IsInvalid) throw new IOException("Console buffer file is invalid");
    }

    internal static void Update()
    {
        if (!_modified) return;

        WriteConsoleOutput(_fileHandle, Buffer, ScreenSize, StartPos, ref _screenRect);
        _modified = false;
    }

    public static void Print(int posX, int posY, char symbol, ConsoleColor color = ConsoleColor.White)
    {
        if (posX is < 0 or > WIDTH || posY is < 0 or > HEIGHT) return;

        var bufferIndex = posY * WIDTH + posX;

        Buffer[bufferIndex].Symbol = (byte) symbol;
        Buffer[bufferIndex].Color = (short) color;

        _modified = true;
    }

    public static void ClearAt(int posX, int posY)
    {
        if (posX is < 0 or > WIDTH || posY is < 0 or > HEIGHT) return;
        
        var bufferIndex = posY * WIDTH + posX;
        
        Buffer[bufferIndex].Symbol = (byte) ' ';
        Buffer[bufferIndex].Color = (short) ConsoleColor.White;

        _modified = true;
    }
    
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
    
    [StructLayout(LayoutKind.Sequential)]
    private struct Coord
    {
        public short X;
        public short Y;
    }
    
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
}