using System.Text;

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
        var previousIsCleared = false;
        
        var symbolsBuilder = new StringBuilder();
        var debugBuilder = new StringBuilder();

        for (var x = 0; x < _pixels.GetLength(1); x++)
        for (var y = 0; y < _pixels.GetLength(0); y++)
        {
            ref var pixel = ref _pixels[y, x]; // swapped indexes

            // Printing the already gathered pixels if next one has different colors
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || (previousIsCleared && pixel.IsEmpty))
            {
                if (symbolsBuilder.Length != 0)
                {
                    _stringBuilder.Append($"\x1b[{streakStartPos.X + 1};{streakStartPos.Y + 1}f");
                    debugBuilder.Append($"{{[{streakStartPos.X + 1}, {streakStartPos.Y + 1}] ");

                    // Resetting the colors to clear the pixel
                    if (previousIsCleared)
                    {
                        _stringBuilder.Append("\x1b[0m");
                        debugBuilder.Append("\x1b[0m '");
                    }
                    else
                    {
                        _stringBuilder.Append($"\x1b[38;2;{lastFg.AnsiString()}m\x1b[48;2;{lastBg.AnsiString()}m");
                        debugBuilder.Append($"{lastFg}, {lastBg} '");
                    }

                    _stringBuilder.Append(symbolsBuilder);

                    debugBuilder.Append(symbolsBuilder);

                    debugBuilder.Append("' }\n");
                }

                symbolsBuilder.Clear();

                lastFg = pixel.Fg;
                lastBg = pixel.Bg;
            }

            // Setting the start pos of the collected pixel symbols when collecting the first one
            if (symbolsBuilder.Length == 0)
            {
                streakStartPos.X = (short) x;
                streakStartPos.Y = (short) y;
            }

            // Collecting the pixels with same colors together
            if (!pixel.IsEmpty) symbolsBuilder.Append(pixel.Symbol);

            previousIsCleared = pixel.IsCleared;

            // Marking the pixel as cleared to not draw it again unnecessarily
            if (pixel.IsCleared) pixel = Pixel.Empty;
        }
        
        // Resetting the console style in case of an app interrupt
        _stringBuilder.Append("\x1b[0m");

        var _ = debugBuilder.ToString();
        debugBuilder.Clear();

        Console.Title = _stringBuilder.ToString().Length.ToString();
        
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
    public static string AnsiString(this Color color)
    {
        return $"{color.R};{color.G};{color.B}";
    }
}