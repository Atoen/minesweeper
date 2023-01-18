using System.Runtime.InteropServices;
using Minesweeper.UI;
using Minesweeper.UI.Events;
using Minesweeper.Utils;

namespace Minesweeper;

public static partial class Input
{
    internal static event Action<KeyboardState>? KeyEvent;
    internal static event Action<WindowState>? WindowEvent;

    private static bool _running;
    private static uint _lastMouseButton = unchecked((uint) -1);
    
    private static readonly MouseState MouseState = new();
    private static KeyboardState _keyboardState;
    
    private static readonly List<Control> Controls = new();
    private static readonly object LockObject = new();

    public static void Init()
    {
        if (_running) return;
        _running = true;

        var inHandle = GetStdHandle(StdInputHandle);

        var mode = 0u;
        GetConsoleMode(inHandle, ref mode);

        mode &= ~EnableQuickEditMode;
        mode |= EnableWindowInput;
        mode |= EnableMouseInput;
        SetConsoleMode(inHandle, mode);

        new Thread(HandleInput)
        {
            Name = "Inupt Thread"
        }.Start();
    }
    
    public static void Register(Control control)
    {
        lock (LockObject) Controls.Add(control);
    }

    public static void Unregister(Control control)
    {
        lock (LockObject) Controls.Remove(control);
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
                case MouseEventCode:
                    HandleMouse(record.MouseEventRecord);
                    break;

                case KeyEventCode:
                    _keyboardState.Assign(ref record.KeyEventRecord);
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
        MouseState.Assign(ref mouseRecord);

        var zIndex = int.MinValue;
        Control? hit = null;

        var pos = MouseState.Position;

        lock (LockObject)
        {
            foreach (var control in Controls)
            {
                if (control.ContainsPoint(pos) && control.ZIndex > zIndex)
                {
                    // Previously marked as hit - now detected something on higher layer blocking the cursor 
                    if (hit is not null) Miss(hit, MouseState.Buttons);

                    zIndex = control.ZIndex;
                    hit = control;
                }
                else
                {
                    Miss(control, MouseState.Buttons);
                }
            }
        }

        if (hit is null || !hit.Enabled) return;

        var args = CreateMouseArgs(MouseState, hit);
        
        SendMouseEvents(hit, args);

        _lastMouseButton = mouseRecord.ButtonState;
    }
    
    private static void Miss(Control control, MouseButton button)
    {
        MouseEventArgs? args = null;

        if (control is {IsMouseOver: true, Enabled: true})
        {
            args = CreateMouseArgs(MouseState, control);
            control.SendMouseEvent(MouseEventType.MouseExit, args);
        }

        if (control is {IsFocusable: true, IsFocused: true} && button.HasValue(MouseButton.Left))
        {
            args ??= CreateMouseArgs(MouseState, control);
            control.SendMouseEvent(MouseEventType.LostFocus, args);
        }
    }

    private static void SendMouseEvents(Control control, MouseEventArgs args)
    {
        if (_lastMouseButton == 0)
        {
            if (MouseState.Buttons.HasValue(MouseButton.Left))
            {
                control.SendMouseEvent(MouseEventType.MouseLeftDown, args);
                
                if (control.IsFocusable) control.SendMouseEvent(MouseEventType.GotFocus, args);
            }

            if (MouseState.Buttons.HasValue(MouseButton.Right))
            {
                control.SendMouseEvent(MouseEventType.MouseRightDown, args);
            }
        }
        
        if (!control.IsMouseOver) control.SendMouseEvent(MouseEventType.MouseEnter, args);

        control.SendMouseEvent(MouseEventType.MouseMove, args);
    }
    
    private static MouseEventArgs CreateMouseArgs(MouseState state, Control source) => new(source)
    {
        CursorPosition = state.Position,
        RelativeCursorPosition = state.Position - source.GlobalPosition,

        LeftButton = state.Buttons.HasValue(MouseButton.Left)
            ? MouseButtonState.Pressed
            : MouseButtonState.Released,

        RightButton = state.Buttons.HasValue(MouseButton.Right)
            ? MouseButtonState.Pressed
            : MouseButtonState.Released,

        MiddleButton = state.Buttons.HasValue(MouseButton.Middle)
            ? MouseButtonState.Pressed
            : MouseButtonState.Released
    };

    internal static void Stop() => _running = false;

    #region Native Methods

    private const uint StdInputHandle = unchecked((uint) -10);

    private const uint EnableMouseInput = 0x0010,
        EnableQuickEditMode = 0x0040,
        EnableWindowInput = 0x0008;

    private const ushort KeyEventCode = 0x0001,
        MouseEventCode = 0x0002,
        WindowBufferSizeEvent = 0x0004;

    [StructLayout(LayoutKind.Explicit)]
    private struct InputRecord
    {
        [FieldOffset(0)] public ushort EventType;
        [FieldOffset(4)] public KeyEventRecord KeyEventRecord;
        [FieldOffset(4)] public MouseEventRecord MouseEventRecord;
        [FieldOffset(4)] public WindowState WindowBufferSizeEventRecord;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MouseEventRecord
    {
        [FieldOffset(0)] public SCoord MousePosition;
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
        public SCoord Size;
    }

    [LibraryImport("kernel32.dll")]
    private static partial nint GetStdHandle(uint nStdHandle);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void GetConsoleMode(nint hConsoleInput, ref uint lpMode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void SetConsoleMode(nint hConsoleInput, uint dwMode);

    [DllImport("kernel32.dll")]
    private static extern bool ReadConsoleInput(nint hConsoleInput, [Out] InputRecord[] lpBuffer, uint nLength,
        ref uint lpNumberOfEventsRead);

    
    [StructLayout(LayoutKind.Sequential)]
    public struct SCoord
    {
        public short X;
        public short Y;
        public override string ToString() => $"({X} {Y})";
    }
    
    #endregion
}

public struct KeyboardState
{
    public ConsoleKey Key;
    public char Char;
    public bool Pressed;

    public void Assign(ref Input.KeyEventRecord record)
    {
        Key = (ConsoleKey) record.VirtualKeyCode;
        Pressed = record.KeyDown;
        Char = record.UnicodeChar;
    }
}

internal class MouseState
{
    public Coord Position;
    public MouseButton Buttons;
    public MouseEventFlags Flags;
    public MouseWheelState Wheel;

    public void Assign(ref Input.MouseEventRecord record)
    {
        Position = new Coord(record.MousePosition.X, record.MousePosition.Y);
        Buttons = (MouseButton) record.ButtonState;
        Wheel = (MouseWheelState) record.ButtonState;
        Flags = (MouseEventFlags) record.EventFlags;
    }
}

[Flags]
internal enum MouseButton
{
    None = 0,
    Left = 1,
    Right = 1 << 1,
    Middle = 1 << 2
}

[Flags]
internal enum MouseEventFlags
{
    Moved = 1,
    DoubleClicked = 1 << 1,
    Wheeled = 1 << 2,
    HorizontalWheeled = 1 << 3
}

internal enum MouseWheelState : ulong
{
    Down = 0xff880000,
    AnsiDown = 0xff800000,
    Up = 0x780000,
    AnsiUp = 0x800000
}
