using System.Runtime.CompilerServices;

namespace Minesweeper.UI;

public abstract class Widget : IRenderable
{
    protected readonly Frame Parent;
    protected Color Color;

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

    public Widget Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        Color = DefaultColor;
        
        if (AutoResize) Resize();
        
        Parent.Grid(this, row, column, rowSpan, columnSpan, alignment);
        Render();
        
        Display.Display.AddToRenderList(this);

        return this;
    }

    public Widget Place(int posX, int posY)
    {
        Color = DefaultColor;
        
        if (AutoResize) Resize();

        Parent.Place(this, posX, posY);
        Render();
        
        Display.Display.AddToRenderList(this);
        
        return this;
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
    
    protected bool IsCursorOver(Coord pos)
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