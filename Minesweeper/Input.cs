using Microsoft.Extensions.ObjectPool;
using Minesweeper.UI;
using Minesweeper.UI.Events;
using static Minesweeper.NativeConsole;

namespace Minesweeper;

internal static class Input
{
    public static bool TreatControlCAsInput
    {
        get => Console.TreatControlCAsInput;
        set => Console.TreatControlCAsInput = value;
    }
    internal static event EventHandler<SCoord>? WindowEvent;

    private static volatile bool _running;
    private static MouseButton _lastMouseButton = MouseButton.None;

    private static readonly MouseState MouseState = new();
    private static readonly KeyboardState KeyboardState = new();

    private static readonly List<Control> Controls = new();
    private static readonly ReaderWriterLockSlim LockSlim = new();

    private static readonly EventArgsPool<MouseEventArgs> MouseArgsPool = new();
    private static readonly EventArgsPool<KeyboardEventArgs> KeyboardArgsPool = new();

    public static void Init()
    {
        if (_running) return;
        _running = true;

        TreatControlCAsInput = true;

        var inHandle = GetStdHandle(StdHandleIn);

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
        try
        {
            MainLoop();
        }
        catch (Exception exception)
        {
            Application.Exit(exception);
        }
    }

    private static void MainLoop()
    {
        var handleIn = GetStdHandle(StdHandleIn);
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

        var args = MouseArgsPool.Get(hit, MouseState);

        SendMouseEvents(hit, args);

        _lastMouseButton = MouseState.Buttons;

        MouseArgsPool.Return(args);
    }

    private static void Miss(Control control, MouseButton button)
    {
        MouseEventArgs? args = null;

        if (control is {IsMouseOver: true, Enabled: true})
        {
            args = MouseArgsPool.Get(control, MouseState);
            control.SendMouseEvent(MouseEventType.MouseExit, args);
        }

        if (control is {Focusable: true, Focused: true} && (button & MouseButton.Left) != 0)
        {
            args ??= MouseArgsPool.Get(control, MouseState);
            control.SendFocusEvent(FocusEventType.LostFocus, args);
        }

        if (args != null) MouseArgsPool.Return(args);
    }

    private static void SendMouseEvents(Control control, MouseEventArgs args)
    {
        if ((MouseState.Flags & MouseEventFlags.DoubleClicked) != 0)
        {
            control.SendMouseEvent(MouseEventType.DoubleClick, args);
        }

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

            if (args.MiddleButton == MouseButtonState.Pressed)
            {
                control.SendMouseEvent(MouseEventType.MouseMiddleDown, args);
            }
        }

        if (!control.IsMouseOver) control.SendMouseEvent(MouseEventType.MouseEnter, args);

        control.SendMouseEvent(MouseEventType.MouseMove, args);
    }

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
        var args = KeyboardArgsPool.Get(control, KeyboardState);
        args.Set(control, KeyboardState);

        control.SendKeyboardEvent(KeyboardState.Pressed ? KeyboardEventType.KeyDown : KeyboardEventType.KeyUp, args);

        KeyboardArgsPool.Return(args);
    }

    internal static void Stop() => _running = false;
}

public sealed class KeyboardState
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

public sealed class MouseState
{
    public Vector Position;
    public MouseButton Buttons;
    public MouseEventFlags Flags;
    public MouseWheelState Wheel;

    public void Assign(ref MouseEventRecord record)
    {
        Position = new Vector(record.MousePosition.X, record.MousePosition.Y);
        Buttons = (MouseButton) record.ButtonState;
        Wheel = (MouseWheelState) record.ButtonState;
        Flags = (MouseEventFlags) record.EventFlags;
    }
}

[Flags]
public enum MouseButton : byte
{
    None = 0,
    Left = 1,
    Right = 1 << 1,
    Middle = 1 << 2
}

[Flags]
public enum MouseEventFlags : byte
{
    Moved = 1,
    DoubleClicked = 1 << 1,
    Wheeled = 1 << 2,
    HorizontalWheeled = 1 << 3
}

public enum MouseWheelState : ulong
{
    Up = 0xff880000,
    AnsiUp = 0xff800000,
    Down = 0x780000,
    AnsiDown = 0x800000
}
