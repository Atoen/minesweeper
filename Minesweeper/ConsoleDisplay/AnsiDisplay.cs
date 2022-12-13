using System.Text;
using Minesweeper.Game;

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
        lock (_threadLock)
        {
            if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

            _currentPixels[posX, posY].Symbol = symbol;
            _currentPixels[posX, posY].Fg = fg;
            _currentPixels[posX, posY].Bg = bg;
        }
    }

    public void Draw(int posX, int posY, TileDisplay tile)
    {
        Draw(posX, posY, tile.Utf8Symbol, tile.Foreground, tile.Background);
    }

    public void ClearAt(int posX, int posY)
    {
        lock (_threadLock)
        {
            if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

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

    private string GenerateDisplayString()
    {
        var lastFg = Color.Transparent;
        var lastBg = Color.Transparent;

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
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || (previousIsCleared && pixel.IsEmpty))
            {
                if (symbolsBuilder.Length != 0)
                {
                    // Need to specify new coords for printing
                    if (oldStreakPos.Y != y || oldStreakPos.X + oldStreakLen != streakStartPos.X)
                        _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");

                    // Resetting the colors to clear the pixels
                    if (previousIsCleared)
                        _stringBuilder.Append("\x1b[0m");

                    // Applying the colors for gathered pixels
                    else
                        _stringBuilder.Append($"\x1b[38;2;{lastFg.AnsiString()}m\x1b[48;2;{lastBg.AnsiString()}m");

                    // Starting new streak of pixels
                    oldStreakLen = symbolsBuilder.Length;
                    oldStreakPos = streakStartPos;

                    _stringBuilder.Append(symbolsBuilder);
                    symbolsBuilder.Clear();
                }

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
            _stringBuilder.Append($"\x1b[38;2;{lastPixel.Fg.AnsiString()}m\x1b[48;2;{lastPixel.Bg.AnsiString()}m");
            _stringBuilder.Append(symbolsBuilder);

            symbolsBuilder.Clear();
        }

        // Resetting the console style after full draw
        _stringBuilder.Append("\x1b[0m");

        Console.Title = $"{_stringBuilder.Length}";

        return _stringBuilder.ToString();
    }

    private struct Pixel : IEquatable<Pixel>
    {
        private const char ClearedSymbol = ' ';

        internal static readonly Pixel Empty = new('\0', Color.Empty, Color.Empty);
        internal static readonly Pixel Cleared = new(ClearedSymbol, Color.Empty, Color.Empty);

        private Pixel(char symbol, Color fg, Color bg)
        {
            Symbol = symbol;
            Fg = fg;
            Bg = bg;
        }

        public char Symbol = '\0';
        public Color Fg = Color.Empty;
        public Color Bg = Color.Empty;

        public override string ToString()
        {
            return $"\x1b[38;2;{Fg.R};{Fg.G};{Fg.B}m\x1b[48;2;{Bg.R};{Bg.G};{Bg.B}m{Symbol}";
        }

        public bool IsEmpty => Symbol == '\0' && Fg == Color.Empty && Bg == Color.Empty;
        public bool IsCleared => Symbol == ClearedSymbol && Fg == Color.Empty && Bg == Color.Empty;

        public static bool operator ==(Pixel a, Pixel b)
        {
            return a.Fg == b.Fg && a.Bg == b.Bg && a.Symbol == b.Symbol;
        }

        public static bool operator !=(Pixel a, Pixel b)
        {
            return !(a == b);
        }

        public bool Equals(Pixel other)
        {
            return Symbol == other.Symbol && Fg.Equals(other.Fg) && Bg.Equals(other.Bg);
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