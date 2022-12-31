using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class DialogWindow : Control, IRenderable
{
    public DialogWindow(IContainer parent) : base(parent)
    {
        MouseEventMask = MouseEventMask.MouseClick | MouseEventMask.MouseMove;
        Layer = Layer.Top;
        
        Position = new Coord(4, 5);
        Size = new Coord(16, 5);
    }
    
    public void Render()
    {
        Display.DrawRect(Position, Size, Color.Red);
    }

    public void Clear()
    {
        Display.ClearRect(Position, Size);
    }

    protected override void OnMouseLeftDown()
    {
        Display.RemoveFromRenderList(this);
        
        Remove();
    }
}