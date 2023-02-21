using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI.Widgets;

public class Canvas : ContentControl
{
    public Canvas() => Focusable = false;

    public required IDrawable Drawable
    {
        get => _drawable;
        set
        {
            _buffer = new Pixel[value.Width, value.Height];
            value.SetBuffer(_buffer);

            _drawable = value;
        }
    }

    private Pixel[,] _buffer = null!;
    private IDrawable _drawable = null!;

    public override void Render()
    {
        base.Render();

        Display.DrawBuffer(GlobalPosition + InnerPadding, _buffer);
    }

    public override void Resize()
    {
        MinSize = InnerPadding * 2 + (Drawable.Width, Drawable.Height);

        ApplyResizing();

        base.Resize();
    }


    // private readonly Pixel[,] _buffer;
    //
    // private readonly IDrawable _drawable;
    //
    // public Canvas(IDrawable drawable)
    // {
    //     _drawable = drawable;
    //     _buffer = new Pixel[drawable.Width, drawable.Height];
    //     
    //     _drawable.SetBuffer(_buffer);
    //     _drawable.Draw();
    // }
    //
    // public override void Render()
    // {
    //     var drawStart = Position + (Size - new Coord(_drawable.Width, _drawable.Height)) / 2;
    //     _drawable.Offset = drawStart;
    //
    //     Display.DrawBuffer(drawStart, _buffer);
    // }
    //
    // public override void Clear()
    // {
    //     Display.ClearRect(Position, Size);
    // }
    //
    // // protected override void Resize()
    // // {
    // //     var minSize = new Coord(_drawable.Width, _drawable.Height);
    // //
    // //     Size = Size.ExpandTo(minSize);
    // // }
}

public interface IDrawable
{
    int Width { get; }
    int Height { get; }

    Vector Offset { get; set; }

    void Draw();

    void SetBuffer(Pixel[,] buffer);
}
