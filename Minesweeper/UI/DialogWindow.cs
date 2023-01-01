using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

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

    public override void OnMouseLeftDown(MouseEventArgs e)
    {
        Display.RemoveFromRenderList(this);
        
        Remove();
        
        base.OnMouseLeftDown(e);
    }
}