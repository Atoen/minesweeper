namespace Minesweeper.UI.Events;

public class MouseScrollEventArgs : MouseEventArgs
{
    public MouseScrollEventArgs(Control source) : base(source)
    {
    }
    
    public required ScrollDirection ScrollDirection { get; init; }
}

public enum ScrollDirection
{
    Up,
    Down,
    None
}