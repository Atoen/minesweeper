using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public class DialogWindow : Control
{
    public DialogWindow()
    {
        ZIndex = 10;
        
        Position = new Coord(4, 5);
        Size = new Coord(16, 5);
        
        DefaultColor = Color.Red;
    }

    protected override void OnMouseLeftDown(MouseEventArgs e)
    {
        Display.RemoveFromRenderList(this);
        
        Remove();
        
        base.OnMouseLeftDown(e);
    }
}