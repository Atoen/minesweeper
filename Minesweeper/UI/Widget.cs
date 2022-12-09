using Minesweeper.Display;

namespace Minesweeper.UI;

public abstract class Widget : IRenderable
{
    protected readonly Frame Parent;

    public Color Color => State switch
    {
        WidgetState.Pressed => PressedColor,
        WidgetState.Highlighted => HighlightedColor,
        _ => DefaultColor
    };

    public Coord Size;
    public Coord Anchor;
    public Coord Offset;

    public bool AutoResize { get; init; } = true;
    public FillMode Fill { get; init; } = FillMode.None;
    public Coord InnerPadding = new(1, 1);
    public Coord OuterPadding = Coord.Zero;

    public Color DefaultColor { get; set; } = Color.Aqua;
    public Color HighlightedColor { get; set; } = Color.Blue;
    public Color PressedColor { get; set; } = Color.White;

    public Layer Layer { get; set; } = Layer.Foreground;

    public WidgetState State { get; protected set; } = WidgetState.Default;

    public bool ShouldRemove { get; protected set; }

    public Coord Center => Anchor + Offset + Size / 2;
    public Coord PaddedSize => Size + OuterPadding * 2;
    
    public int Width
    {
        get => Size.X;
        set => Size.X = value;
    }
    public int Height
    {
        get => Size.Y;
        set => Size.Y = value;
    }

    protected Widget(Frame parent)
    {
        Parent = parent;
    }

    public abstract Widget Grid(int row, int column, int rowSpan = 1, int columnSpan = 1,
        GridAlignment alignment = GridAlignment.Center);

    protected T Grid<T>(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
        where T : Widget
    {
        if (AutoResize) Resize();
        
        Parent.Grid(this, row, column, rowSpan, columnSpan, alignment);

        Display.Display.AddToRenderList(this);

        return this as T ?? throw new InvalidOperationException();
    }

    public abstract Widget Place(int posX, int posY);

    protected T Place<T>(int posX, int posY) where T : Widget
    {
        if (AutoResize) Resize();

        Parent.Place(this, posX, posY);

        Display.Display.AddToRenderList(this);
        
        return this as T ?? throw new InvalidOperationException();
    }

    public virtual void Render()
    {
        Display.Display.DrawRect(Anchor + Offset, Size, Color);
    }

    public virtual void Remove() => ShouldRemove = true;

    public virtual void Clear()
    {
        Display.Display.ClearRect(Anchor + Offset, Size);
    }
    
    protected bool IsInside(Coord pos)
    {
        var c = Anchor + Offset;
        
        return pos.X >= c.X && pos.X < c.X + Width &&
               pos.Y >= c.Y && pos.Y < c.Y + Height;
    }

    protected abstract void Resize();
}

public enum WidgetState
{
    Default,
    Highlighted,
    Pressed
}

public enum Alignment
{
    Left,
    Right,
    Center
}

public enum FillMode
{
    None,
    Vertical,
    Horizontal,
    Both
}
