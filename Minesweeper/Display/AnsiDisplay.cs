using System.Runtime.CompilerServices;
using System.Text;
using Minesweeper.Game;

namespace Minesweeper.Display;

public class AnsiDisplay : IRenderer
{
    public bool Modified { get; set; }
    public int ChunkSize { get; set; }

    private readonly Coord _displaySize;
    private readonly Pixel[,] _pixels;
    private bool[,] _modifiedChunks;
    
    private readonly StringBuilder _stringBuilder = new();
    
    public AnsiDisplay(int width, int height, int chunkSize = 5)
    {
        _displaySize.X = (short) width;
        _displaySize.Y = (short) height;
        
        _pixels = new Pixel[width, height];
        
        // for (var i = 0; i < width; i++)
        // for (var j = 0; j < height; j++)
        // {
        //     _pixels[i, j] = Pixel.Empty;
        // }

        ChunkSize = chunkSize;
        var chunksX = (width + chunkSize - 1) / chunkSize;
        var chunksY = (height + chunkSize - 1) / chunkSize;

        _modifiedChunks = new bool[chunksX, chunksY];
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

        SetChunk(posX, posY);
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
        
        SetChunk(posX, posY);
        Modified = true;
    }
    
    public void Draw()
    {
        Console.Write(GenerateDisplayString());
        _stringBuilder.Clear();
    }

    private void SetChunk(int posX, int posY)
    {
        var x = posX / ChunkSize;
        var y = posY / ChunkSize;

        _modifiedChunks[x, y] = true;
    }

    private bool GetChunk(int posX, int posY)
    {
        var x = posX / ChunkSize;
        var y = posY / ChunkSize;

        return _modifiedChunks[x, y];
    }

    private void CleatChunks()
    {
        for (var i = 0; i < _modifiedChunks.GetLength(0); i++)
        for (var j = 0; j < _modifiedChunks.GetLength(1); j++)
        {
            _modifiedChunks[i, j] = false;
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
        
        for (var y = 0; y < _pixels.GetLength(1); y++)
        for (var x = 0; x < _pixels.GetLength(0); x++)
        {
            var pixel = _pixels[x, y];

            // Printing the already gathered pixels if next one has different colors
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || previousIsCleared && pixel.IsEmpty)
            {
                if (symbolsBuilder.Length != 0)
                {
                    // need to specify new coords for printing
                    if (oldStreakPos.Y != y || oldStreakPos.X + oldStreakLen != streakStartPos.X)
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
                streakStartPos.Y = (short) y;
                streakStartPos.X = (short) x;
            }
        
            // Collecting the pixels with same colors together
            if (!pixel.IsEmpty) symbolsBuilder.Append(pixel.Symbol);
        
            previousIsCleared = pixel.IsCleared;
        
            // Marking the pixel as empty to not draw it again unnecessarily
            if (pixel.IsCleared) _pixels[x, y] = Pixel.Empty;
        }

        // If all of the pixels are the same, they are printed all at once
        if (symbolsBuilder.Length > 0)
        {
            var lastPixel = _pixels[_displaySize.X - 1, _displaySize.Y - 1];
            
            _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");
            _stringBuilder.Append($"\x1b[38;2;{lastPixel.Fg.AnsiString()}m\x1b[48;2;{lastPixel.Bg.AnsiString()}m");
            _stringBuilder.Append(symbolsBuilder);

            symbolsBuilder.Clear();
        }

        // Resetting the console style after full draw
        _stringBuilder.Append("\x1b[0m");
        CleatChunks();
        
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