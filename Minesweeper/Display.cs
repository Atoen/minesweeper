using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace Minesweeper;

public static class Display
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }
    
    private static SafeFileHandle _fileHandle = null!;
    
    private static CharInfo[] _buffer = null!;
    private static DisplayRect _screenRect;
    private static Coord _screenSize;
    private static readonly Coord StartPos = new() {X = 0, Y = 0};

    private static bool _modified;

    [SupportedOSPlatform("windows")]
    internal static void Init(short width, short height)
    {
        if (width < 40) width = 40;
        if (height < 20) height = 20;

        Width = width;
        Height = height;

        // Displaying stuff on the screen
        _buffer = new CharInfo[Width * Height];
        _screenSize = new Coord {X = Width, Y = Height};
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = Width, Bottom = Height};
        
        Console.Title = "Minesweeper";
        Console.SetWindowSize(Width - 2, height);
        Console.SetBufferSize(Width, Height);
        Console.CursorVisible = false;
        
        // Disabling window resizing
        var consoleHandle = GetConsoleWindow();
        var sysMenu = GetSystemMenu(consoleHandle, false);
        if (consoleHandle != IntPtr.Zero)
        {
            DeleteMenu(sysMenu, 0xF030, 0); // Maximize
            DeleteMenu(sysMenu, 0xF000, 0); // Resize
        }

        // Hiding scrollbars
        var windowHandle = GetStdHandle(-11);
        var bufferInfo = new ConsoleScreenBufferInfoEx();
        bufferInfo.cbSize = (uint) Marshal.SizeOf(bufferInfo);
        
        GetConsoleScreenBufferInfoEx(windowHandle, ref bufferInfo);
        bufferInfo.srWindow.Right++;
        bufferInfo.srWindow.Bottom++;
        SetConsoleScreenBufferInfoEx(windowHandle, ref bufferInfo);
        
        // file handle for faster console printing
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

        WriteConsoleOutput(_fileHandle, _buffer, _screenSize, StartPos, ref _screenRect);
        _modified = false;
    }

    public static void Print(int posX, int posY, char symbol, ConsoleColor color = ConsoleColor.White)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        var bufferIndex = posY * Width + posX;

        _buffer[bufferIndex].Symbol = (byte) symbol;
        _buffer[bufferIndex].Color = (short) color;

        _modified = true;
    }

    public static void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        var bufferIndex = posY * Width + posX;

        _buffer[bufferIndex].Symbol = (byte) ' ';
        _buffer[bufferIndex].Color = (short) ConsoleColor.White;

        _modified = true;
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

    [StructLayout(LayoutKind.Sequential)]
    private struct ConsoleScreenBufferInfoEx
    {
        public uint cbSize;
        public Coord dwSize;
        public Coord dwCursorPosition;
        public short wAttributes;
        public DisplayRect srWindow;
        public Coord dwMaximumWindowSize;
        public ushort wPopupAttributes;
        public bool bFullscreenSupported;

        public Colorref black,
            darkBlue,
            darkGreen,
            darkCyan,
            darkRed,
            darkMagenta,
            darkYellow,
            gray,
            darkGray,
            blue,
            green,
            cyan,
            red,
            magenta,
            yellow,
            white;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Colorref
    {
        public uint ColorDWORD;
    }
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleScreenBufferInfoEx(
        IntPtr hConsoleOutput,
        ref ConsoleScreenBufferInfoEx consoleScreenBufferInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleScreenBufferInfoEx(
        IntPtr hConsoleOutput,
        ref ConsoleScreenBufferInfoEx consoleScreenBufferInfoEx);

    [DllImport("user32.dll")]
    private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    
    #endregion
}
