using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Minesweeper.Game;
using Minesweeper.UI;

namespace Minesweeper.ConsoleDisplay;

public sealed class NativeDisplay : IRenderer
{
    public bool Modified { get; set; }
    
    private readonly SafeFileHandle _fileHandle;
    
    private DisplayRect _screenRect;
    private readonly SCoord _displaySize;
    private readonly SCoord _startPos = new() {X = 0, Y = 0};

    private readonly CharInfo[] _currentBuffer;
    private readonly CharInfo[] _lastBuffer;

    private readonly object _threadLock = new();

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
        
        _displaySize = new SCoord {X = (short) width, Y = (short) height};
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = (short) width, Bottom = (short) height};

        _currentBuffer = new CharInfo[width * height];
        _lastBuffer = new CharInfo[width * height];
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        lock(_threadLock)
        {
            if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

            var bufferIndex = posY * _displaySize.X + posX;
            
            var cfg = fg.ConsoleColor();
            var cbg = bg.ConsoleColor();

            var color = (short) ((int) cfg | (int) cbg << 4);

            _currentBuffer[bufferIndex].Symbol = (byte) symbol;
            _currentBuffer[bufferIndex].Color = color;
        }
    }

    public void Draw(int posX, int posY, TileDisplay tile)
    {
        Draw(posX, posY, tile.Symbol, tile.Foreground, tile.Background);
    }

    public void ClearAt(int posX, int posY)
    {
        lock (_threadLock)
        {
            if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

            var bufferIndex = posY * _displaySize.X + posX;

            _currentBuffer[bufferIndex] = CharInfo.Cleared;
        }
    }
    
    public void Draw()
    {
        lock (_threadLock)
        {
            CopyToBuffer();

            if (!Modified) return;
            
            Debug.WriteLine("Modified");
            
            WriteConsoleOutput(_fileHandle, _currentBuffer, _displaySize, _startPos, ref _screenRect);
            Modified = false;
        }
    }

    public void ResetStyle() { }

    private unsafe void CopyToBuffer()
    {
        fixed (CharInfo* current = _currentBuffer, last = _lastBuffer)
        {
            for (var i = 0; i < _displaySize.X * _displaySize.Y; i++)
            {
                if (current[i].Symbol == last[i].Symbol && current[i].Color == last[i].Color) continue;
                
                Modified = true;
                Array.Copy(_currentBuffer, _lastBuffer, _displaySize.X * _displaySize.Y);
                
                return;
            }
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
        SCoord dwBufferSize,
        SCoord dwBufferSCoord,
        ref DisplayRect lpWriteRegion);

    [StructLayout(LayoutKind.Explicit)]
    private struct CharInfo
    {
        public static readonly CharInfo Empty = new() {Symbol = 0, Color = 0};
        public static readonly CharInfo Cleared = new() {Symbol = (byte) ' ', Color = 0};
        
        [FieldOffset(0)] public byte Symbol;
        [FieldOffset(2)] public short Color;

        public bool IsEmpty => Symbol == 0 && Color == 0;
        public bool IsCleared => Symbol == (byte) ' ' && Color == 0;
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
    
    [StructLayout(LayoutKind.Sequential)]
    private struct SCoord
    {
        public short X;
        public short Y;

        public override string ToString() => $"({X} {Y})";
    }
}
