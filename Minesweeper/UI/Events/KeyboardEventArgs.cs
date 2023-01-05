namespace Minesweeper.UI.Events;

public class KeyboardEventArgs : InputEventArgs
{
    public KeyboardEventArgs(Control source) : base(Device.KeyboardDevice, source)
    {
    }

    public ConsoleKey Key { get; init; }
    public bool IsReleased { get; init; }
    public bool IsPressed { get; init; }
}