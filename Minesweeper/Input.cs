using System.Runtime.InteropServices;
using Minesweeper.UI;

namespace Minesweeper;

public static class Input
{
    internal static event Action<KeyboardState>? KeyEvent;
    internal static event Action<MouseState>? MouseEvent;
    internal static event Action<MouseState>? MouseClickEvent;
    internal static event Action<MouseWheelState>? MouseWheelEvent; 
    internal static event Action<WindowBufferSizeRecord>? WindowEvent;

    private static bool _running;
    private static uint _lastMouseButton = unchecked((uint) -1);
    
    private static MouseState _mouseState;
    private static KeyboardState _keyboardState;

    private static readonly List<IInteractable> MouseUi = new();
    private static readonly List<IInteractable> KeyboardUi = new();

    internal static void Init()
    {
        if (_running) return;
        _running = true;

        var inHandle = GetStdHandle(STD_INPUT_HANDLE);

        uint mode = 0;
        GetConsoleMode(inHandle, ref mode);

        mode &= ~ENABLE_QUICK_EDIT_MODE;
        mode |= ENABLE_WINDOW_INPUT;
        mode |= ENABLE_MOUSE_INPUT;
        SetConsoleMode(inHandle, mode);

        new Thread(HandleInput).Start();
    }

    internal static void RegisterMouseUiElement(IInteractable mouseUi) => MouseUi.Add(mouseUi);
    internal static void UnregisterMouseUiElement(IInteractable mouseUi) => MouseUi.Remove(mouseUi);
    internal static void RegisterKeyboardUiElement(IInteractable mouseUi) => KeyboardUi.Add(mouseUi);
    internal static void UnregisterKeyboardUiElement(IInteractable mouseUi) => KeyboardUi.Remove(mouseUi);

    private static void HandleInput()
    {
        var handleIn = GetStdHandle(STD_INPUT_HANDLE);
        var recordArray = new[] {new InputRecord()};
        
        
        while (_running)
        {
            foreach (var mouseUi in MouseUi)
            {
                mouseUi.Update();
            }
            
            uint numRead = 0;
            ReadConsoleInput(handleIn, recordArray, 1, ref numRead);

            var record = recordArray[0];

            switch (record.EventType)
            {
                case MOUSE_EVENT:
                    HandleMouse(record.MouseEventRecord);
                    break;

                case KEY_EVENT:
                    _keyboardState.Assign(record.KeyEventRecord);
                    KeyEvent?.Invoke(_keyboardState);
                    break;

                case WINDOW_BUFFER_SIZE_EVENT:
                    WindowEvent?.Invoke(record.WindowBufferSizeEventRecord);
                    break;
            }
        }

        uint numWritten = 0;
        WriteConsoleInput(handleIn, recordArray, 1, ref numWritten);
    }

    private static void HandleMouse(MouseEventRecord mouseRecord)
    {
        _mouseState.Assign(mouseRecord);
        if (_lastMouseButton == 0 && _mouseState.Buttons != 0)
        {
            MouseClickEvent?.Invoke(_mouseState);

            // foreach (var mouseUi in MouseUi)
            // {
            //     mouseUi.Click(_mouseState);
            // }
        }

        _lastMouseButton = mouseRecord.ButtonState;
        
        if (_lastMouseButton == (ulong) MouseWheelState.Down || _lastMouseButton == (ulong) MouseWheelState.Up)
            MouseWheelEvent?.Invoke((MouseWheelState) _lastMouseButton);
        
        MouseEvent?.Invoke(_mouseState);
        
        // foreach (var mouseUi in MouseUi)
        // {
        //     mouseUi.CursorMove(_mouseState);
        // }
    }

    internal static void Stop() => _running = false;

    #region NativeMethods

    private const uint STD_INPUT_HANDLE = unchecked((uint) -10),
        STD_OUTPUT_HANDLE = unchecked((uint) -11),
        STD_ERROR_HANDLE = unchecked((uint) -12);

    private const uint ENABLE_MOUSE_INPUT = 0x0010,
        ENABLE_QUICK_EDIT_MODE = 0x0040,
        ENABLE_EXTENDED_FLAGS = 0x0080,
        ENABLE_ECHO_INPUT = 0x0004,
        ENABLE_WINDOW_INPUT = 0x0008;

    private const ushort KEY_EVENT = 0x0001,
        MOUSE_EVENT = 0x0002,
        WINDOW_BUFFER_SIZE_EVENT = 0x0004;

    [StructLayout(LayoutKind.Explicit)]
    private struct InputRecord
    {
        [FieldOffset(0)] public readonly ushort EventType;
        [FieldOffset(4)] public readonly KeyEventRecord KeyEventRecord;
        [FieldOffset(4)] public readonly MouseEventRecord MouseEventRecord;
        [FieldOffset(4)] public readonly WindowBufferSizeRecord WindowBufferSizeEventRecord;
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

    public struct WindowBufferSizeRecord
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
    public ushort KeyCode;
    public char Char;
    public bool Pressed;

    public void Assign(Input.KeyEventRecord record)
    {
        KeyCode = record.VirtualKeyCode;
        Pressed = record.KeyDown;
        Char = record.UnicodeChar;
    }
}

public struct MouseState
{
    public Coord Position;
    public MouseButtonState Buttons;
    public MouseEventFlags Flags;

    public void Assign(Input.MouseEventRecord record)
    {
        Position = record.MousePosition;
        Buttons = (MouseButtonState) record.ButtonState;
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