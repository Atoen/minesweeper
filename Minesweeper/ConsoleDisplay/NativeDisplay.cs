using Microsoft.Win32.SafeHandles;
using Minesweeper.UI;
using Minesweeper.UI.Widgets;

using static Minesweeper.NativeConsole;

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
            nint.Zero,
            FileMode.Open,
            0,
            nint.Zero);
        
        if (_fileHandle.IsInvalid) throw new IOException("Console buffer file is invalid");
        
        _displaySize = new SCoord {X = (short) width, Y = (short) height};
        _screenRect = new DisplayRect {Left = 0, Top = 0, Right = (short) width, Bottom = (short) height};

        _currentBuffer = new CharInfo[width * height];
        _lastBuffer = new CharInfo[width * height];
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        var bufferIndex = posY * _displaySize.X + posX;
        
        var color = Colors.CombineToShort(fg, bg);

        lock(_threadLock)
        {
            _currentBuffer[bufferIndex].Symbol = symbol;
            _currentBuffer[bufferIndex].Color = color;
        }
    }

    public void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        if (pos.X >= _displaySize.X || pos.Y >= _displaySize.Y) return;
        
        var endX = Math.Min(size.X, _displaySize.X - pos.X);
        var endY = Math.Min(size.Y, _displaySize.Y - pos.Y);

        var consoleColor = (short) (color.ConsoleColorIndex() << 4);

        lock (_threadLock)
        {
            for (var x = 0; x < endX; x++)
            for (var y = 0; y < endY; y++)
            {
                var bufferIndex = (pos.Y + y) * _displaySize.X + pos.X + x;

                _currentBuffer[bufferIndex].Symbol = symbol;
                _currentBuffer[bufferIndex].Color = consoleColor;
            }
        }
    }

    public void DrawLine(Coord pos, Coord direction, int length, Color fg, Color bg, char symbol)
    {
    }

    public void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode _)
    {
        if (posY < 0 || posY >= _displaySize.Y) return;

        var startX = alignment switch
        {
            Alignment.Left => posX,
            Alignment.Right => posX - text.Length,
            _ => posX - text.Length / 2
        };
        
        if (startX < 0) startX = 0;
        if (startX >= _displaySize.X) startX = _displaySize.X - 1;
    
        var endX = startX + text.Length;
        
        if (endX >= _displaySize.X) endX = _displaySize.X - 1;

        var color = Colors.CombineToShort(fg, bg);
        
        var bufferIndex = posY * _displaySize.X + startX;

        lock (_threadLock)
        {
            for (var i = 0; i < endX - startX; i++)
            {
                _currentBuffer[bufferIndex + i].Color = color;
                _currentBuffer[bufferIndex + i].Symbol = text[i];
            }
        }
    }

    public void DrawBuffer(Coord pos, Coord size, Pixel[,] buffer)
    {
        if (pos.X >= _displaySize.X || pos.Y >= _displaySize.Y) return;

        var endX = Math.Min(size.X, _displaySize.X - pos.X);
        var endY = Math.Min(size.Y, _displaySize.Y - size.Y);
            
        lock (_threadLock)
        {
            for (var x = 0; x < endX; x++)
            for (var y = 0; y < endY; y++)
            {
                var index = (y + pos.Y) * _displaySize.X + x + pos.X;
                var pixel = buffer[x, y];
                
                _currentBuffer[index].Symbol = pixel.Symbol;

                var color = Colors.CombineToShort(buffer[x, y].Fg, buffer[x, y].Bg);
                _currentBuffer[index].Color = color;
            }
        }
    }

    public void DrawBorder(Coord pos, Coord size, Color color, BorderStyle style)
    {
    }


    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        var bufferIndex = posY * _displaySize.X + posX;

        lock (_threadLock)
        {
            _currentBuffer[bufferIndex] = CharInfo.Empty;
        }
    }

    public void ClearRect(Coord pos, Coord size)
    {
        if (pos.X >= _displaySize.X || pos.Y >= _displaySize.Y) return;
        
        var endX = Math.Min(size.X, _displaySize.X - pos.X);
        var endY = Math.Min(size.Y, _displaySize.Y - pos.Y);

        lock (_threadLock)
        {
            for (var x = 0; x < endX; x++)
            for (var y = 0; y < endY; y++)
            {
                var bufferIndex = (pos.Y + y) * _displaySize.X + pos.X + x;

                _currentBuffer[bufferIndex] = CharInfo.Empty;
            }
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

    public void ResetStyle() => Console.ResetColor();

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
    
    public void Clear()
    {
    }
}
