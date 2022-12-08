using Minesweeper.Display;

namespace Minesweeper.UI;

public abstract class Widget : IRenderable
{
    protected readonly Frame Parent;
    public Color Color { get; protected set; }

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

    public WidgetState State { get; protected set; }

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
        Color = DefaultColor;
        
        if (AutoResize) Resize();
        
        Parent.Grid(this, row, column, rowSpan, columnSpan, alignment);
        Render();
        
        Display.Display.AddToRenderList(this);

        return this as T ?? throw new InvalidOperationException();
    }

    public abstract Widget Place(int posX, int posY);

    protected T Place<T>(int posX, int posY) where T : Widget
    {
        Color = DefaultColor;
        
        if (AutoResize) Resize();

        Parent.Place(this, posX, posY);
        Render();
        
        Display.Display.AddToRenderList(this);
        
        return this as T ?? throw new InvalidOperationException();
    }

    public virtual void Render()
    {
        Display.Display.DrawRect(Anchor + Offset, Size, Color, layer: Layer);
    }

    public virtual void Remove() => ShouldRemove = true;

    public virtual void Clear()
    {
        Display.Display.ClearRect(Anchor + Offset, Size, Layer);
    }
    
    protected bool IsInside(Coord pos)
    {
        var c = Anchor + Offset;
        
        return pos.X >= c.X && pos.X < c.X + Width &&
               pos.Y >= c.Y && pos.Y < c.Y + Height;
    }

    protected abstract void Resize();

    public bool ShouldRemove { get; private set; }
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
