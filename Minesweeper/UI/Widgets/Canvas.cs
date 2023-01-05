using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI.Widgets;

public class Canvas : Widget
{
    private readonly AnsiDisplay.Pixel[,] _buffer;

    private readonly IDrawable _drawable;

    public Canvas(IDrawable drawable)
    {
        _drawable = drawable;
        _buffer = new AnsiDisplay.Pixel[drawable.Width, drawable.Height];
        
        _drawable.SetBuffer(_buffer);
        _drawable.Draw();
    }

    // public override Canvas Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    // {
    //     return base.Grid<Canvas>(row, column, rowSpan, columnSpan, alignment);
    // }
    //
    // public override Canvas Place(int posX, int posY)
    // {
    //     return base.Grid<Canvas>(posX, posY);
    // }

    public override void Render()
    {
        var drawStart = Position + (Size - new Coord(_drawable.Width, _drawable.Height)) / 2;
        _drawable.Offset = drawStart;

        Display.DrawBuffer(drawStart, _buffer);
    }

    public override void Clear()
    {
        Display.ClearRect(Position, Size);
    }

    // protected override void Resize()
    // {
    //     var minSize = new Coord(_drawable.Width, _drawable.Height);
    //
    //     Size = Size.ExpandTo(minSize);
    // }
}

public interface IDrawable
{
    int Width { get; }
    int Height { get; }
    
    Coord Offset { get; set; }

    void Draw();

    void SetBuffer(AnsiDisplay.Pixel[,] buffer);
}
