namespace Minesweeper.UI.Events;

public class MouseEventArgs : InputEventArgs
{
    public MouseEventArgs(Control source) : base(Device.MouseDevice, source)
    {
    }
    
    public required MouseButtonState LeftButton { get; init; }
    public required MouseButtonState RightButton { get; init; }
    public required MouseButtonState MiddleButton { get; init; }
    
    public required Vector CursorPosition { get; init; }
    public required Vector RelativeCursorPosition { get; init; }
}

public enum MouseButtonState
{
    Released,
    Pressed
}
