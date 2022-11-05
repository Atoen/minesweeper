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
        
        _pixels = new Pixel[Height, Width];
    }
    
    public void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        for (var y = 0; y < size.Y; y++)
        for (var x = 0; x < size.X; x++)
        {
            Draw(pos.X + x, pos.Y + y, symbol, color, color);
        }
    }
    
    public void Draw()
    {
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
        
        _renderables.AddRange(_addedRenderables);
        _addedRenderables.Clear();

        _renderables.RemoveAll(r => r.ShouldRemove);
    }

    public void Draw(int posX, int posY, char symbol, Color foreground, Color background)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;

        if (_pixels[posY, posX].Symbol == symbol && _pixels[posY, posX].Fg == foreground &&
            _pixels[posY, posX].Bg == background)
        {
            return;
        }

        _pixels[posY, posX].Symbol = symbol;
        _pixels[posY, posX].Fg = foreground;
        _pixels[posY, posX].Bg = background;

        _modified = true;
    }
    
    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return;
        
        if (_pixels[posY, posX].IsEmpty) return;

        _pixels[posY, posX] = Pixel.Empty;

        _modified = true;
    }
    public void AddToRenderList(IRenderable renderable) => _addedRenderables.Add(renderable);

    private string GenerateDisplayString()
    {
        for (var y = 0; y < _pixels.GetLength(1); y++)
        for (var x = 0; x < _pixels.GetLength(0); x++)
        {
            var pixel = _pixels[x, y];
            
            if (pixel.IsEmpty) continue;
            
            _stringBuilder.Append($"\x1b[{x + 1};{y + 1}f{pixel}");
            _stringBuilder.Append("\x1b[0m");
        }

        return _stringBuilder.ToString();
    }
    
    private struct Pixel
    {
        internal static readonly Pixel Empty = new(' ', Color.Empty, Color.Empty);
        
        public Pixel(char symbol, Color fg, Color bg)
        {
            Symbol = symbol;
            Fg = fg;
            Bg = bg;
        }

        public char Symbol;
        public Color Fg = Color.Black;
        public Color Bg = Color.Black;

        public override string ToString()
        {
            return $"\x1b[38;2;{Fg.R};{Fg.G};{Fg.B}m\x1b[48;2;{Bg.R};{Bg.G};{Bg.B}m{Symbol}";
        }

        public bool IsEmpty => Bg.IsEmpty || Bg == Color.Black && Symbol == ' ';
    }
}