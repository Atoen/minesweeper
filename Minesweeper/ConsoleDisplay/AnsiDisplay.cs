#nullable enable
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using Minesweeper.Game;
using Minesweeper.UI;

namespace Minesweeper.ConsoleDisplay;

public sealed class AnsiDisplay : IRenderer
{
    public bool Modified { get; set; }

    private readonly Coord _displaySize;
    private readonly StringBuilder _stringBuilder = new();

    private readonly Pixel[,] _currentPixels;
    private readonly Pixel[,] _lastPixels;

    private readonly object _threadLock = new();

    public AnsiDisplay(int width, int height)
    {
        _displaySize.X = (short) width;
        _displaySize.Y = (short) height;

        _currentPixels = new Pixel[width, height];
        _lastPixels = new Pixel[width, height];
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        lock (_threadLock)
        {
            _currentPixels[posX, posY].Symbol = symbol;
            _currentPixels[posX, posY].Fg = fg;
            _currentPixels[posX, posY].Bg = bg;
        }
    }

    public void Draw(int posX, int posY, TileDisplay tile)
    {
        Draw(posX, posY, tile.Utf8Symbol, tile.Foreground, tile.Background);
    }

    public void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode mode)
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
        
        lock (_threadLock)
        {
            for (int x = startX - posX, i = 0; x < endX - posX; x++, i++)
            {
                _currentPixels[posX + x, posY].Symbol = text[i];
                _currentPixels[posX + x, posY].Mode = mode;
                _currentPixels[posX + x, posY].Fg = fg;
                _currentPixels[posX + x, posY].Bg = bg;
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
                _currentPixels[x + pos.X, y + pos.Y] = buffer[x, y];
            }
        }
    }

    public void ClearAt(int posX, int posY)
    {
        
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;
        lock (_threadLock)
        {
            _currentPixels[posX, posY] = Pixel.Cleared;
        }
    }

    public void Draw()
    {
        lock (_threadLock)
        {
            CopyToBuffer();

            if (!Modified) return;

            Console.Write(GenerateDisplayString());
            _stringBuilder.Clear();

            Modified = false;
        }
    }

    public void ResetStyle() => Console.Write("\x1b[0m");

    private void CopyToBuffer()
    {
        for (var x = 0; x < _displaySize.X; x++)
        for (var y = 0; y < _displaySize.Y; y++)
        {
            if (_currentPixels[x, y] == _lastPixels[x, y]) continue;

            Modified = true;
            Array.Copy(_currentPixels, _lastPixels, _displaySize.X * _displaySize.Y);

            return;
        }
    }

    private readonly ReadOnlyDictionary<TextMode, string> _ansiTextModes = new(
        new Dictionary<TextMode, string>
        {
            {TextMode.Default, "\x1b[0m"},
            {TextMode.Bold, "\x1b[1m"},
            {TextMode.Italic, "\x1b[3m"},
            {TextMode.Underline, "\x1b[4m"},
            {TextMode.DoubleUnderline, "\x1b[21m"},
            {TextMode.Overline, "\x1b[53m"},
            {TextMode.Strikethrough, "\x1b[9m"}
        });

    private void AppendTextMode(TextMode mode, StringBuilder builder)
    {
        if (mode == TextMode.Default)
        {
            builder.Append(_ansiTextModes[TextMode.Default]);
            
            return;
        }

        foreach (var textMode in _ansiTextModes.Keys)
        {
            if ((mode & textMode) != 0)
            {
                builder.Append(_ansiTextModes[textMode]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private string GenerateDisplayString()
    {
        var lastFg = Color.Transparent;
        var lastBg = Color.Transparent;
        var lastMode = TextMode.Default;

        // starting position for printing the gathered pixel symbols
        var streakStartPos = new Coord();
        var oldStreakPos = new Coord();
        var oldStreakLen = 0;
        var previousIsCleared = false;

        var symbolsBuilder = new StringBuilder();

        _stringBuilder.Append("\x1b[1;1f");

        for (var y = 0; y < _displaySize.Y; y++)
        for (var x = 0; x < _displaySize.X; x++)
        {
            var pixel = _currentPixels[x, y];

            // Printing the already gathered pixels if next one has different colors
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || pixel.Mode != lastMode || (previousIsCleared && pixel.IsEmpty))
            {
                if (symbolsBuilder.Length != 0)
                {
                    // Need to specify new coords for printing
                    if (oldStreakPos.Y != y || oldStreakPos.X + oldStreakLen != streakStartPos.X)
                    {
                        _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");
                    }
                    
                    AppendTextMode(lastMode, _stringBuilder);

                    // Resetting the colors to clear the pixels
                    if (previousIsCleared)
                    {
                        _stringBuilder.Append("\x1b[0m");
                    }

                    // Applying the colors for gathered pixels
                    else
                    {
                        lastFg.AppendToBuilder(_stringBuilder);
                        lastBg.AppendToBuilderBg(_stringBuilder);
                    }
                    
                    // Starting new streak of pixels
                    oldStreakLen = symbolsBuilder.Length;
                    oldStreakPos = streakStartPos;
                    
                    _stringBuilder.Append(symbolsBuilder);
                    symbolsBuilder.Clear();
                }
                
                lastMode = pixel.Mode;
                lastFg = pixel.Fg;
                lastBg = pixel.Bg;
            }

            // Setting the start pos of the collected pixel symbols when collecting the first one
            if (symbolsBuilder.Length == 0)
            {
                streakStartPos.Y = y;
                streakStartPos.X = x;
            }

            // Collecting the pixels with same colors together
            if (!pixel.IsEmpty) symbolsBuilder.Append(pixel.Symbol);

            previousIsCleared = pixel.IsCleared;

            // Marking the pixel as empty to not draw it again unnecessarily
            if (pixel.IsCleared) _currentPixels[x, y] = Pixel.Empty;
        }

        // If all of the pixels are the same, they are printed all at once
        if (symbolsBuilder.Length > 0)
        {
            var lastPixel = _currentPixels[_displaySize.X - 1, _displaySize.Y - 1];

            _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");
            AppendTextMode(lastMode, _stringBuilder);

            lastPixel.Fg.AppendToBuilder(_stringBuilder);
            lastPixel.Bg.AppendToBuilderBg(_stringBuilder);
            
            _stringBuilder.Append(symbolsBuilder);

            symbolsBuilder.Clear();
        }

        // Resetting the console style after full draw
        _stringBuilder.Append("\x1b[0m");

        Console.Title = $"{_stringBuilder.Length}";

        return _stringBuilder.ToString();
    }

    public struct Pixel : IEquatable<Pixel>
    {
        private const char ClearedSymbol = ' ';

        internal static readonly Pixel Empty = new('\0', TextMode.Default, Color.Empty, Color.Empty);
        internal static readonly Pixel Cleared = new(ClearedSymbol, TextMode.Default, Color.Empty, Color.Empty);

        private uint _symbolData = 0;

        private Pixel(char symbol, TextMode mode, Color fg, Color bg)
        {
            _symbolData = (uint) (symbol & 0xfffffff) << 8;
            _symbolData |= (uint) ((int) mode & 0xf);
            
            Fg = fg;
            Bg = bg;
        }
        
        public Color Fg = Color.Empty;
        public Color Bg = Color.Empty;

        public override string ToString()
        {
            return $"\x1b[38;2;{Fg.R};{Fg.G};{Fg.B}m\x1b[48;2;{Bg.R};{Bg.G};{Bg.B}m{Symbol}";
        }

        public bool IsEmpty => Symbol == '\0' && Fg == Color.Empty && Bg == Color.Empty;
        public bool IsCleared => Symbol == ClearedSymbol && Fg == Color.Empty && Bg == Color.Empty;

        public char Symbol
        {
            get => (char) ((_symbolData >> 8) & 0xfffffff);
            set => _symbolData = (_symbolData & 0xff) | (uint) (value & 0xfffffff) << 8;
        }

        public TextMode Mode
        {
            get => (TextMode) (_symbolData & 0xff);
            set => _symbolData = (_symbolData & ~(uint) 0xff) | (uint) value & 0xff;
        }
        
        public static implicit operator Pixel(TileDisplay tileDisplay) =>
            new(tileDisplay.Symbol, TextMode.Default, tileDisplay.Foreground, tileDisplay.Background);

        public static bool operator ==(Pixel a, Pixel b)
        {
            return a.Fg == b.Fg && a.Bg == b.Bg && a._symbolData == b._symbolData;
        }

        public static bool operator !=(Pixel a, Pixel b)
        {
            return !(a == b);
        }

        public bool Equals(Pixel other)
        {
            return _symbolData == other._symbolData && Fg.Equals(other.Fg) && Bg.Equals(other.Bg);
        }

        public override bool Equals(object? obj)
        {
            return obj is Pixel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, Fg, Bg);
        }
    }
}
