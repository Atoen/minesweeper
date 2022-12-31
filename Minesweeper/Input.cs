﻿using System.Runtime.InteropServices;
using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;

namespace Minesweeper;

public static class Input
{
    internal static event Action<KeyboardState>? KeyEvent;
    internal static event Action<WindowState>? WindowEvent;

    private static bool _running;
    private static uint _lastMouseButton = unchecked((uint) -1);
    
    private static readonly MouseState MouseState = new();
    private static KeyboardState _keyboardState;
    
    private static readonly object LockObject = new();

    internal static void Init()
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

    private static readonly List<Control> Controls = new();
    public static void Register(Control control)
    {
        lock (LockObject)
        {
            Controls.Add(control);
        }
    }

    public static void Unregister(Control control)
    {
        lock (LockObject)
        {
            Controls.Remove(control);
        }
    }

    private static void HandleMouse(MouseEventRecord mouseRecord)
    {
        MouseState.Assign(ref mouseRecord);

        var layer = (Layer) (-1);
        Control? hit = null;

        var pos = MouseState.Position;

        void Miss(Control control, MouseButton button)
        {
            if (control.IsEnabled && control.MouseEventMask.HasValue(MouseEventMask.MouseMove)) control.MouseExit();

            if (control.IsFocused && button.HasValue(MouseButton.Left)) control.LostFocus();
        }

        lock (LockObject)
        {
            foreach (var control in Controls)
            {
                if (control.ContainsPoint(pos) && control.Layer > layer)
                {
                    // Previously marked as hit - now detected something on higher layer blocking the cursor 
                    if (hit is not null) Miss(hit, MouseState.Button);

                    layer = control.Layer;
                    hit = control;
                }
                else
                {
                    Miss(control, MouseState.Button);
                }
            }
        }

        if (hit is null || !hit.UsesMouseEvents || !hit.IsEnabled) return;

        if (hit.MouseEventMask.HasValue(MouseEventMask.MouseClick) && _lastMouseButton == 0)
        {
            if (MouseState.Button.HasValue(MouseButton.Left))
            {
                hit.MouseLeftDown();
                hit.GotFocus();
            }

            if (MouseState.Button.HasValue(MouseButton.Right))
            {
                
            }
        }

        _lastMouseButton = mouseRecord.ButtonState;

        if (hit.MouseEventMask.HasValue(MouseEventMask.MouseMove))
        {
            hit.MouseMove(MouseState);
        }
    }

    internal static void Stop() => _running = false;

    #region NativeMethods

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

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(uint nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleInput, uint dwMode);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] InputRecord[] lpBuffer, uint nLength,
        ref uint lpNumberOfEventsRead);

    #endregion
    
    [StructLayout(LayoutKind.Sequential)]
    public struct SCoord
    {
        public short X;
        public short Y;
        public override string ToString() => $"({X} {Y})";
    }
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

public class MouseState
{
    public Coord Position;
    public MouseButton Button;
    public MouseEventFlags Flags;
    public MouseWheelState Wheel;

    public void Assign(ref Input.MouseEventRecord record)
    {
        Position = new Coord(record.MousePosition.X, record.MousePosition.Y);
        Button = (MouseButton) record.ButtonState;
        Wheel = (MouseWheelState) record.ButtonState;
        Flags = (MouseEventFlags) record.EventFlags;
    }
}

[Flags]
public enum MouseButton
{
    None = 0,
    Left = 1,
    Right = 1 << 1,
    Middle = 1 << 2
}

[Flags]
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
    AnsiDown = 0xff800000,
    Up = 0x780000,
    AnsiUp = 0x800000
}
