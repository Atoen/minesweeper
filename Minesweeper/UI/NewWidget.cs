using Minesweeper.Display;

namespace Minesweeper.UI;

public abstract class NewWidget
{
    protected NewFrame Parent;

    public Coord Size;
    public Coord DrawOffset;
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
        
        return this;
    }

    public virtual NewWidget Place(int posX, int posY)
    {
        Parent.Place(this, posX, posY);
        Render();
        
        return this;
    }

    public void Render()
    {
        // for (var x = 0; x < Size.X; x++)
        // for (var y = 0; y < Size.Y; y++)
        // {
        //     Display.Display.Draw();
        // }
        
        Display.Display.DrawRect(DrawOffset, Size, DefaultColor);
    }

    public void Clear()
    {
        Display.Display.ClearRect(DrawOffset, Size);
    }
}