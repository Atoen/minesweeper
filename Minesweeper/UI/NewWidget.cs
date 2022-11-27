using Minesweeper.Display;

namespace Minesweeper.UI;

public abstract class NewWidget : IRenderable
{
    protected NewFrame Parent;
    protected Coord DrawOffset;
    protected Coord _center;

    public Coord Size;
    public Coord Anchor;

    public Coord Center
    {
        get => _center;
        set
        {
            _center = value;
            Anchor = value;
            DrawOffset = _center - new Coord(Size.X / 2, Size.Y / 2);
        }
    }

    public bool AutoResize;

    public Color DefaultColor = Color.Aqua;
    public Color HighlightedColor = Color.Blue;
    public Color PressedColor = Color.White;
    
    protected NewWidget(NewFrame parent)
    {
        Parent = parent;
    }

    public virtual NewWidget Grid(int row, int column)
    {
        Parent.Grid(this, row, column);
        Render();
        
        Display.Display.AddToRenderList(this);
        
        return this;
    }

    public virtual NewWidget Place(int posX, int posY)
    {
        Parent.Place(this, posX, posY);
        Render();
        
        Display.Display.AddToRenderList(this);
        
        return this;
    }

    public virtual void Render()
    {
        Display.Display.DrawRect(DrawOffset, Size, DefaultColor);
    }

    public void Clear()
    {
        Display.Display.ClearRect(DrawOffset, Size);
    }

    public bool ShouldRemove { get; }
}