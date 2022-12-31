using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public abstract class Widget : Control, IRenderable
{
    public Color DefaultColor { get; set; } = Color.Aqua;
    public Color HighlightedColor { get; set; } = Color.Blue;
    public Color PressedColor { get; set; } = Color.White;
    
    public Color Color => State switch
    {
        State.Pressed => PressedColor,
        State.Highlighted => HighlightedColor,
        State.Disabled => DefaultColor.Dimmer(),
        _ => DefaultColor
    };
    
    public FillMode Fill { get; init; } = FillMode.None;
    
    public Coord InnerPadding = new(1, 1);
    public Coord OuterPadding = Coord.Zero;
    
    public Coord PaddedSize => Size + OuterPadding * 2;
    
    // public abstract Widget Grid(int row, int column, int rowSpan = 1, int columnSpan = 1,
    //     GridAlignment alignment = GridAlignment.Center);

    // protected T Grid<T>(int row, int column, int rowSpan = 1, int columnSpan = 1,
    //     GridAlignment alignment = GridAlignment.Center)
    //     where T : Widget
    // {
    //     // if (AutoResize) Resize();
    //     //
    //     // Parent.Grid(this, row, column, rowSpan, columnSpan, alignment);
    //     //
    //     // Display.AddToRenderList(this);
    //     //
    //     // return this as T ?? throw new InvalidOperationException();
    // }

    // public abstract Widget Place(int posX, int posY);

    // protected T Place<T>(int posX, int posY) where T : Widget
    // {
    //     // if (AutoResize) Resize();
    //     //
    //     // Parent.Place(this, posX, posY);
    //     //
    //     // Display.AddToRenderList(this);
    //     //
    //     // return this as T ?? throw new InvalidOperationException();
    // }

    public virtual void Render()
    {
        Display.DrawRect(Position, Size, Color);
    }
    
    public virtual void Clear()
    {
        Display.ClearRect(Position, Size);
    }

    protected abstract void Resize();

    public override void Remove()
    {
        Display.RemoveFromRenderList(this);
        
        base.Remove();
    }
}

public enum Alignment
{
    Left,
    Right,
    Center
}


