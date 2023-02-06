namespace Minesweeper.UI.Events;

public class KeyboardEventArgs : InputEventArgs
{
    public KeyboardEventArgs(Control source) : base(Device.KeyboardDevice, source)
    {
    }

    public required ConsoleKey Key { get; init; }
    public required char Char { get; init; }
    public required bool IsReleased { get; init; }
    public required bool IsPressed { get; init; }
}
