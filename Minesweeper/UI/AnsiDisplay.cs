using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace Minesweeper.UI;

internal sealed class AnsiDisplay : IDisplayProvider
{
    private readonly StringBuilder _stringBuilder = new();
    private bool _modified;
    
    private Pixel[,] _pixels;
    
    public static short Width { get; private set; }
    public static short Height { get; private set; }
    
    private readonly List<IRenderable> _renderables = new();
    private readonly List<IRenderable> _addedRenderables = new();

    public AnsiDisplay(int width, int height)
    {
        Width = (short) width;
        Height = (short) height;
        
        _pixels = new Pixel[Width, Height];

        for (var i = 0; i < _pixels.GetLength(0); i++)
        for (var j = 0; j < _pixels.GetLength(1); j++)
        {
            _pixels[i, j] = Pixel.Empty;
        }
    }

    public void Draw()
    {
        foreach (var renderable in _renderables)
        {
            if (renderable.ShouldRemove) renderable.Clear();
        }

        _renderables.RemoveAll(r => r.ShouldRemove);
        
        foreach (var renderable in _renderables)
        {
            renderable.Render();
        }
        
        if (_modified)
        {
            Console.Write(GenerateDisplayString());
            _modified = false;
        }
        
        _stringBuilder.Clear();

        if (_addedRenderables.Count == 0) return;
        
        _renderables.AddRange(_addedRenderables);
        _addedRenderables.Clear();
    }

    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        if (_pixels[posX, posY].Symbol == symbol && _pixels[posX, posY].Fg == fg &&
            _pixels[posX, posY].Bg == bg)
        {
            return;
        }

        _pixels[posX, posY].Symbol = symbol;
        _pixels[posX, posY].Fg = fg;
        _pixels[posX, posY].Bg = bg;

        _modified = true;
    }
    
    public void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            Draw(pos.X + x, pos.Y + y, symbol, color, color);
        }
    }

    public void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment)
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
            Draw(posX + x, posY, text[i], fg, bg);
        }
    }
    
    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;
        
        if (_pixels[posX, posY].IsEmpty) return;

        // _pixels[posX, posY].Symbol = ' ';
        // _pixels[posX, posY].Bg = Color.Empty;
        // _pixels[posX, posY].Fg = Color.Empty;

        _pixels[posX, posY] = Pixel.Cleared;
        
        _modified = true;
    }

    public void ClearRect(Coord pos, Coord size)
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            ClearAt(pos.X + x, pos.Y + y);
        }
    }
    
    public void AddToRenderList(IRenderable renderable) => _addedRenderables.Add(renderable);

    private string GenerateDisplayString()
    {
        var lastFg = Color.Transparent;
        var lastBg = Color.Transparent;
        var firstStreakPos = new Coord();
        var symbolsBuilder = new StringBuilder();
        var debugBuilder = new StringBuilder();
        
        for (var x = 0; x < _pixels.GetLength(1); x++)
        for (var y = 0; y < _pixels.GetLength(0); y++)
        {
            ref var pixel = ref _pixels[y, x]; // swapped indexes

            if (pixel.IsCleared)
            {
                // pixel = Pixel.Empty;
            }

            // Printing the already gathered pixels if next one has different colors
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || pixel.IsCleared)
            {
                _stringBuilder.Append($"\x1b[{firstStreakPos.X + 1};{firstStreakPos.Y + 1}f");
                if (pixel.IsCleared)
                {
                    _stringBuilder.Append("\x1b[0m");
                }
                else
                {
                    _stringBuilder.Append($"\x1b[38;2;{lastFg.AnsiString()}m\x1b[48;2;{lastBg.AnsiString()}m");
                }
                
                _stringBuilder.Append(symbolsBuilder);

                debugBuilder.Append($"{{[{firstStreakPos.X + 1}, {firstStreakPos.Y + 1}] {lastFg}, {lastBg} '");
                debugBuilder.Append(symbolsBuilder);

                debugBuilder.Append("' }\n");
                
                symbolsBuilder.Clear();
                
                lastFg = pixel.Fg;
                lastBg = pixel.Bg;

                firstStreakPos.X = (short) x;
                firstStreakPos.Y = (short) y;
            }

            // Collecting the pixels with same colors together
            if (!pixel.IsEmpty) symbolsBuilder.Append(pixel.Symbol);
            if (pixel.IsCleared) pixel = Pixel.Empty; 
        }
        
        // Resetting the console style in case of an app interrupt
        _stringBuilder.Append("\x1b[0m");

        var _ = debugBuilder.ToString();
        debugBuilder.Clear();

        return _stringBuilder.ToString();
    }
    
    private struct Pixel
    {
        private const char ClearedSymbol = ' ';
        
        internal static readonly Pixel Empty = new('\0', Color.Empty, Color.Empty);
        internal static readonly Pixel Cleared = new(ClearedSymbol, Color.Empty, Color.Empty);

        // public bool IsCleared = false;
        
        public Pixel(char symbol, Color fg, Color bg)
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
