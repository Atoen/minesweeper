using Minesweeper.UI;
using Minesweeper.UI.Events;

using static Minesweeper.NativeConsole;

namespace Minesweeper;

public static class Input
{
    internal static event EventHandler<SCoord>? WindowEvent; 

    private static volatile bool _running;
    private static MouseButton _lastMouseButton = MouseButton.None;
    
    private static readonly MouseState MouseState = new();
    private static readonly KeyboardState KeyboardState = new();
    
    private static readonly List<Control> Controls = new();
    private static readonly ReaderWriterLockSlim LockSlim = new();

    public static void Init()
    {
        if (_running) return;
        _running = true;

        var inHandle = GetStdHandle(unchecked((uint) -10));
        
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
        LockSlim.EnterWriteLock();
        Controls.Add(control);
        LockSlim.ExitWriteLock();
    }

    public static void Unregister(Control control)
    {
        LockSlim.EnterWriteLock();
        Controls.Remove(control);
        LockSlim.ExitWriteLock();
    }

    private static void HandleInput()
    {
        var handleIn = GetStdHandle(unchecked((uint) -10));
        var recordArray = new[] {new InputRecord()};
        
        while (_running)
        {
            uint numRead = 0;
            ReadConsoleInput(handleIn, recordArray, 1, ref numRead);

            var record = recordArray[0];

            switch (record.EventType)
            {
                case MouseEventCode:
                    MouseState.Assign(ref record.MouseEventRecord);
                    HandleMouse();
                    break;

                case KeyEventCode:
                    KeyboardState.Assign(ref record.KeyEventRecord);
                    HandleKeyboard();
                    break;

                case WindowBufferSizeEvent:
                    WindowEvent?.Invoke(null, record.WindowBufferSizeEventRecord.Size);
                    break;
            }
        }
    }

    private static void HandleMouse()
    {
        // if (MouseState.Position.Y > Display.Height - 1) MouseState.Position.Y = Display.Height - 1;
        
        var zIndex = int.MinValue;
        Control? hit = null;

        var pos = MouseState.Position;

        LockSlim.EnterWriteLock();
        
        foreach (var control in Controls)
        {
            if (control.ContainsPoint(pos) && control.ZIndex > zIndex)
            {
                // Previously marked as hit - now detected something on higher layer blocking the cursor 
                if (hit != null) Miss(hit, MouseState.Buttons);

                zIndex = control.ZIndex;
                hit = control;
            }
            else
            {
                Miss(control, MouseState.Buttons);
            }
        }
            
        LockSlim.ExitWriteLock();

        if (hit is not {Enabled: true}) return;

        var args = CreateMouseArgs(hit);
        
        SendMouseEvents(hit, args);

        _lastMouseButton = MouseState.Buttons;
    }
    
    private static void Miss(Control control, MouseButton button)
    {
        MouseEventArgs? args = null;

        if (control is {IsMouseOver: true, Enabled: true})
        {
            args = CreateMouseArgs(control);
            control.SendMouseEvent(MouseEventType.MouseExit, args);
        }

        if (control is {Focusable: true, Focused: true} && (button & MouseButton.Left) != 0)
        {
            args ??= CreateMouseArgs(control);
            control.SendFocusEvent(FocusEventType.LostFocus, args);
        }
    }

    private static void SendMouseEvents(Control control, MouseScrollEventArgs args)
    {
        if (args.ScrollDirection != ScrollDirection.None)
        {
            control.SendMouseEvent(MouseEventType.MouseScroll, args);
        }
        
        if (_lastMouseButton == MouseButton.None)
        {
            if (args.LeftButton == MouseButtonState.Pressed)
            {
                control.SendMouseEvent(MouseEventType.MouseLeftDown, args);
                
                if (control.Focusable) control.SendFocusEvent(FocusEventType.GotFocus, args);
            }

            if (args.RightButton == MouseButtonState.Pressed)
            {
                control.SendMouseEvent(MouseEventType.MouseRightDown, args);
            }
        }
        
        if (!control.IsMouseOver) control.SendMouseEvent(MouseEventType.MouseEnter, args);

        control.SendMouseEvent(MouseEventType.MouseMove, args);
    }

    private static MouseScrollEventArgs CreateMouseArgs(Control source) => new(source)
    {
        CursorPosition = MouseState.Position,
        RelativeCursorPosition = MouseState.Position - source.GlobalPosition,

        LeftButton = (MouseState.Buttons & MouseButton.Left) != 0
            ? MouseButtonState.Pressed
            : MouseButtonState.Released,

        RightButton = (MouseState.Buttons & MouseButton.Right) != 0
            ? MouseButtonState.Pressed
            : MouseButtonState.Released,

        MiddleButton = (MouseState.Buttons & MouseButton.Middle) != 0
            ? MouseButtonState.Pressed
            : MouseButtonState.Released,
        
        ScrollDirection = MouseState.Wheel switch
        {
            MouseWheelState.Down or MouseWheelState.AnsiDown => ScrollDirection.Down,
            MouseWheelState.Up or MouseWheelState.AnsiUp => ScrollDirection.Up,
            _ => ScrollDirection.None
        }
    };

    private static void HandleKeyboard()
    {
        LockSlim.EnterWriteLock();

        foreach (var control in Controls)
        {
            if (!control.Focused) continue;
            
            SendKeyboardEvent(control);
            break;
        }
            
        LockSlim.ExitWriteLock();
    }

    private static void SendKeyboardEvent(Control control)
    {
        var args = new KeyboardEventArgs(control)
        {
            Key = KeyboardState.Key,
            Char = KeyboardState.Char,
            IsPressed = KeyboardState.Pressed,
            IsReleased = !KeyboardState.Pressed
        };

        if (KeyboardState.Pressed)
        {
            control.SendKeyboardEvent(KeyboardEventType.KeyDown, args);
            return;
        }
        
        control.SendKeyboardEvent(KeyboardEventType.KeyUp, args);
    }

    internal static void Stop() => _running = false;
}

internal sealed class KeyboardState
{
    public ConsoleKey Key;
    public char Char;
    public bool Pressed;

    public void Assign(ref KeyEventRecord record)
    {
        Key = (ConsoleKey) record.VirtualKeyCode;
        Pressed = record.KeyDown;
        Char = record.UnicodeChar;
    }
}

internal sealed class MouseState
{
    public Coord Position;
    public MouseButton Buttons;
    public MouseEventFlags Flags;
    public MouseWheelState Wheel;

    public void Assign(ref MouseEventRecord record)
    {
        Position = new Coord(record.MousePosition.X, record.MousePosition.Y);
        Buttons = (MouseButton) record.ButtonState;
        Wheel = (MouseWheelState) record.ButtonState;
        Flags = (MouseEventFlags) record.EventFlags;
    }
}

[Flags]
internal enum MouseButton : byte
{
    None = 0,
    Left = 1,
    Right = 1 << 1,
    Middle = 1 << 2
}

[Flags]
internal enum MouseEventFlags : byte
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
