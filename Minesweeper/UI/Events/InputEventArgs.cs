namespace Minesweeper.UI.Events;

public class InputEventArgs : RoutedEventArgs
{
    public InputEventArgs(Device device, Control source) : base(source) => Device = device;

    public Device Device { get; }
}

public enum Device
{
    MouseDevice,
    KeyboardDevice
}
