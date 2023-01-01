#nullable enable
using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public class Button : Widget
{
    public Action? OnClick { get; init; }
    public required IAnimatedText AnimatedText { get; init; }
    public Coord TextOffset = Coord.Zero;

    public Button()
    {
        MouseEventMask = MouseEventMask.MouseMove | MouseEventMask.MouseClick;
    }

    public override void Render()
    {

    }
    
    protected override void Resize()
    {
        var minSize = new Coord(AnimatedText.Length + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }
    
    public override void OnMouseLeftDown(MouseEventArgs e)
    {
        State = State.Pressed;
        OnClick?.Invoke();
        
        base.OnMouseLeftDown(e);
    }
}
