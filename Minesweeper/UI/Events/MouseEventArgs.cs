namespace Minesweeper.UI.Events;

public class MouseEventArgs : InputEventArgs
{
    public MouseEventArgs(Control source) : base(Device.MouseDevice, source)
    {
    }
    
    public MouseButtonState LeftButton { get; init; }
    public MouseButtonState RightButton { get; init; }
    public MouseButtonState MiddleButton { get; init; }
    
    public Coord CursorPosition { get; init; }
    public Coord RelativeCursorPosition { get; init; }
}

public enum MouseButtonState
{
    Released,
    Pressed
}
