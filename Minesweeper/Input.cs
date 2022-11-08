using System.Runtime.InteropServices;

namespace Minesweeper;

public static class Input
{
    internal static event Action<KeyboardState>? KeyEvent;
    internal static event Action<MouseState>? MouseEvent;
    internal static event Action<MouseState>? MouseLeftClick;
    internal static event Action<MouseState>? MouseRightClick; 
    internal static event Action<MouseState>? DoubleClick;
    internal static event Action<WindowState>? WindowEvent;

    private static bool _running;
    private static uint _lastMouseButton = unchecked((uint) -1);
    
    private static MouseState _mouseState;
    private static KeyboardState _keyboardState;

    internal static void Init()
    {
        if (_running) return;
        _running = true;

        var inHandle = GetStdHandle(StdInputHandle);

        uint mode = 0;
        GetConsoleMode(inHandle, ref mode);

        mode &= ~EnableQuickEditMode;
        mode |= EnableWindowInput;
        mode |= EnableMouseInput;
        SetConsoleMode(inHandle, mode);

        new Thread(HandleInput).Start();
    }

    private static void HandleInput()
    {
        var handleIn = GetStdHandle(StdInputHandle);
        var recordArray = new[] {new InputRecord()};
        
        while (_running)
        {
            uint numRead = 0;
            ReadConsoleInput(handleIn, recordArray, 1, ref numRead);

            var record = recordArray[0];

            switch (record.EventType)
            {
                case _MouseEvent:
                    HandleMouse(record.MouseEventRecord);
                    break;

                case _KeyEvent:
                    _keyboardState.Assign(record.KeyEventRecord);
                    KeyEvent?.Invoke(_keyboardState);
                    break;

                case WindowBufferSizeEvent:
                    WindowEvent?.Invoke(record.WindowBufferSizeEventRecord);
                    break;
            }
        }
    }

    private static void HandleMouse(MouseEventRecord mouseRecord)
    {
        _mouseState.Assign(ref mouseRecord);
        
        if (_lastMouseButton == 0)
        {
            if ((_mouseState.Buttons & MouseButtonState.Left) != 0)
            {
                MouseLeftClick?.Invoke(_mouseState);
            }

            if ((_mouseState.Buttons & MouseButtonState.Right) != 0)
            {
                MouseRightClick?.Invoke(_mouseState);
            }
        }

        if (mouseRecord.EventFlags == (ulong) MouseEventFlags.DoubleClicked)
        {
            DoubleClick?.Invoke(_mouseState);
        }

        _lastMouseButton = mouseRecord.ButtonState;

        MouseEvent?.Invoke(_mouseState);
    }

    internal static void Stop() => _running = false;

    #region NativeMethods

    private const uint StdInputHandle = unchecked((uint) -10);

    private const uint EnableMouseInput = 0x0010,
        EnableQuickEditMode = 0x0040,
        EnableWindowInput = 0x0008;

    private const ushort _KeyEvent = 0x0001,
        _MouseEvent = 0x0002,
        WindowBufferSizeEvent = 0x0004;

    [StructLayout(LayoutKind.Explicit)]
    private struct InputRecord
    {
        [FieldOffset(0)] public readonly ushort EventType;
        [FieldOffset(4)] public readonly KeyEventRecord KeyEventRecord;
        [FieldOffset(4)] public readonly MouseEventRecord MouseEventRecord;
        [FieldOffset(4)] public readonly WindowState WindowBufferSizeEventRecord;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MouseEventRecord
    {
        [FieldOffset(0)] public Coord MousePosition;
        [FieldOffset(4)] public uint ButtonState;
        [FieldOffset(12)] public uint EventFlags;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KeyEventRecord
    {
        [FieldOffset(0)] public readonly bool KeyDown;
        [FieldOffset(4)] public readonly ushort RepeatCount;
        [FieldOffset(6)] public readonly ushort VirtualKeyCode;
        [FieldOffset(8)] public readonly ushort VirtualScanCode;
        [FieldOffset(10)] public readonly char UnicodeChar;
        [FieldOffset(10)] public readonly byte AsciiChar;
        [FieldOffset(12)] public readonly uint ControlKeyState;
    }

    public struct WindowState
    {
        public Coord Size;
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(uint nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleInput, uint dwMode);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] InputRecord[] lpBuffer, uint nLength,
        ref uint lpNumberOfEventsRead);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool WriteConsoleInput(IntPtr hConsoleInput, InputRecord[] lpBuffer, uint nLength,
        ref uint lpNumberOfEventsWritten);

    #endregion
}

internal struct KeyboardState
{
    public ConsoleKey Key;
    public char Char;
    public bool Pressed;

    public void Assign(Input.KeyEventRecord record)
    {
        Key = (ConsoleKey) record.VirtualKeyCode;
        Pressed = record.KeyDown;
        Char = record.UnicodeChar;
    }
}

public struct MouseState
{
    public Coord Position;
    public MouseButtonState Buttons;
    public MouseEventFlags Flags;
    public MouseWheelState Wheel;

    public void Assign(ref Input.MouseEventRecord record)
    {
        Position = record.MousePosition;
        Buttons = (MouseButtonState) record.ButtonState;

        Wheel = (MouseWheelState) record.ButtonState;
        
        Flags = (MouseEventFlags) record.EventFlags;
    }
}

[Flags]
public enum MouseButtonState
{
    None = 0,
    Left = 1,
    Right = 1 << 1,
    Middle = 1 << 2
}

public enum MouseEventFlags
{
    Moved = 1,
    DoubleClicked = 1 << 1,
    Wheeled = 1 << 2,
    HorizontalWheeled = 1 << 3
}

public enum MouseWheelState : ulong
{
    Down = 0xff880000,
    Up = 0x780000
}