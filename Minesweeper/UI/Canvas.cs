using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Canvas : Widget
{
    private readonly AnsiDisplay.Pixel[,] _buffer;

    private readonly IDrawable _drawable;

    public Canvas(Frame parent, IDrawable drawable) : base(parent)
    {
        _drawable = drawable;
        _buffer = new AnsiDisplay.Pixel[drawable.Width, drawable.Height];
        
        _drawable.SetBuffer(_buffer);
        _drawable.Draw();
    }

    public override Canvas Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        return base.Grid<Canvas>(row, column, rowSpan, columnSpan, alignment);
    }

    public override Canvas Place(int posX, int posY)
    {
        return base.Grid<Canvas>(posX, posY);
    }

    public override void Render()
    {
        var start = Anchor + Offset + (Size - new Coord(_drawable.Width, _drawable.Height)) / 2;
        _drawable.Offset = start;

        for (var x = 0; x < _drawable.Width; x++)
        for (var y = 0; y < _drawable.Height; y++)
        {
            var pixel = _buffer[x, y];
            Display.Draw(start.X + x, start.Y + y, pixel.Symbol, pixel.Fg, pixel.Bg);
        }
    }

    public override void Clear()
    {
        var start = Anchor + Offset;
        
        Display.ClearRect(start, (_drawable.Width, _drawable.Height));
    }

    protected override void Resize()
    {
        var minSize = new Coord(_drawable.Width, _drawable.Height);
    
        Size = Size.ExpandTo(minSize);
    }
}

public interface IDrawable
{
    int Width { get; }
    int Height { get; }
    
    Coord Offset { get; set; }

    void Draw();

    void SetBuffer(AnsiDisplay.Pixel[,] buffer);
}
