using System.Text;
using Minesweeper.Game;

namespace Minesweeper.Display;

public class AnsiDisplay : IRenderer
{
    public bool Modified { get; set; }
    
    private readonly Pixel[,] _pixels;
    private readonly Coord _displaySize;
    
    private readonly StringBuilder _stringBuilder = new();
    
    public AnsiDisplay(int width, int height)
    {
        _displaySize.X = (short) width;
        _displaySize.Y = (short) height;
        
        _pixels = new Pixel[width, height];

        for (var i = 0; i < _pixels.GetLength(0); i++)
        for (var j = 0; j < _pixels.GetLength(1); j++)
        {
            _pixels[i, j] = Pixel.Empty;
        }
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        if (_pixels[posX, posY].Symbol == symbol && _pixels[posX, posY].Fg == fg &&
            _pixels[posX, posY].Bg == bg)
        {
            return;
        }

        _pixels[posX, posY].Symbol = symbol;
        _pixels[posX, posY].Fg = fg;
        _pixels[posX, posY].Bg = bg;

        Modified = true;
    }

    public void Draw(int posX, int posY, TileDisplay tile)
    {
        Draw(posX, posY, tile.Utf8Symbol, tile.Foreground, tile.Background);
    }

    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;
        
        if (_pixels[posX, posY].IsEmpty) return;

        _pixels[posX, posY] = Pixel.Cleared;
        
        Modified = true;
    }
    
    public void Draw()
    {
        Console.Write(GenerateDisplayString());
        _stringBuilder.Clear();
    }
    
    private string GenerateDisplayString()
    {
        var lastFg = Color.Empty;
        var lastBg = Color.Empty;
        
        // starting position for printing the gathered pixel symbols
        var streakStartPos = new Coord();
        var oldStreakPos = new Coord();
        var previousIsCleared = false;
        
        var symbolsBuilder = new StringBuilder();

        _stringBuilder.Append($"\x1b[1;1f");
        
        for (var y = 0; y < _pixels.GetLength(1); y++)
        for (var x = 0; x < _pixels.GetLength(0); x++)
        {
            var pixel = _pixels[x, y]; // swapped indexes

            // Printing the already gathered pixels if next one has different colors
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || previousIsCleared && pixel.IsEmpty)
            {
                if (symbolsBuilder.Length != 0)
                {
                    if (oldStreakPos.Y != y || oldStreakPos.X + symbolsBuilder.Length != streakStartPos.X)
                    {
                        _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");
                    }

                    // Resetting the colors to clear the pixels
                    if (previousIsCleared)
                    {
                        _stringBuilder.Append("\x1b[0m");
                    }
                    
                    // Applying the colors for gathered pixels
                    else
                    {
                        _stringBuilder.Append($"\x1b[38;2;{lastFg.AnsiString()}m\x1b[48;2;{lastBg.AnsiString()}m");
                    }

                    // Starting new streak of pixels
                    _stringBuilder.Append(symbolsBuilder);
                    oldStreakPos = streakStartPos;
                }

                symbolsBuilder.Clear();

                lastFg = pixel.Fg;
                lastBg = pixel.Bg;
            }

            // Setting the start pos of the collected pixel symbols when collecting the first one
            if (symbolsBuilder.Length == 0)
            {
                streakStartPos.Y = (short) y;
                streakStartPos.X = (short) x;
            }

            // Collecting the pixels with same colors together
            if (!pixel.IsEmpty) symbolsBuilder.Append(pixel.Symbol);

            previousIsCleared = pixel.IsCleared;

            // Marking the pixel as empty to not draw it again unnecessarily
            if (pixel.IsCleared) _pixels[x, y] = Pixel.Empty;
        }
        
        // Resetting the console style after full draw
        _stringBuilder.Append("\x1b[0m");

        Console.Title = $"{_stringBuilder.Length}";
        
        return _stringBuilder.ToString();
    }

    private struct Pixel
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
    }
}

public static class ColorExtensions
{
    public static string AnsiString(this Color color) => $"{color.R};{color.G};{color.B}";
}