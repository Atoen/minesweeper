namespace Minesweeper.UI;

public abstract class Widget : IRenderable
{
    protected readonly Frame Parent;
    protected Color Color;

    public Coord Size;
    public Coord Anchor;
    public Coord Offset;
    
    public bool AutoResize = true;
    public Coord InnerPadding = new(1, 1);
    public Coord OuterPadding = Coord.Zero;

    public Color DefaultColor = Color.Aqua;
    public Color HighlightedColor = Color.Blue;
    public Color PressedColor = Color.White;

    public WidgetState State { get; protected set; }

    public Coord Center => Anchor + Offset + Size / 2;
    
    protected Widget(Frame parent)
    {
        Parent = parent;
    }

    public Widget Grid(int row, int column, int rowSpan = 1, int columnSpawn = 1, GridAlignment alignment = GridAlignment.Center)
    {
        Color = DefaultColor;
        
        if (AutoResize) Resize();
        
        Parent.Grid(this, row, column, rowSpan, columnSpawn, alignment);
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

    public void Clear()
    {
        Display.Display.ClearRect(Anchor + Offset, Size);
    }
    
    protected bool IsCursorOver(Coord pos)
    {
        var c = Anchor + Offset;
        
        return pos.X >= c.X && pos.X < c.X + Size.X &&
               pos.Y >= c.Y && pos.Y < c.Y + Size.Y;
    }

    protected abstract void Resize();

    public bool ShouldRemove { get; private set; }
}