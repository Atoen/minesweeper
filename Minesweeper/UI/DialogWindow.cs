using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public class DialogWindow : Control
{
    public DialogWindow()
    {
        Layer = Layer.Top;
        
        Position = new Coord(4, 5);
        Size = new Coord(16, 5);
        
        DefaultColor = Color.Red;
    }

    public override void OnMouseLeftDown(MouseEventArgs e)
    {
        Display.RemoveFromRenderList(this);
        
        Remove();
        
        base.OnMouseLeftDown(e);
    }
}