using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public abstract class Control
{
    protected Control(IContainer? parent = null)
    {
        Parent = parent;
        
        Input.Register(this);
    }
    
    public IContainer? Parent { get; set; }

    public bool IsMouseOver { get; protected set; }
    public bool IsFocused { get; protected set; }
    public State State { get; protected set; } = State.Default;

    public MouseEventMask MouseEventMask { get; protected set; } = MouseEventMask.All;
    public bool IsEnabled => State != State.Disabled;
    public bool UsesMouseEvents => MouseEventMask != MouseEventMask.None;
    public Layer Layer { get; set; } = Layer.Foreground;
    
    public bool AutoResize { get; init; } = true;
    
    public Coord Size;
    public Coord Position;
    public Coord Center
    {
        get => Position + Size / 2;
        set => Position = value - Size / 2;
    }

    public int Width
    {
        get => Size.X;
        set => Size.X = value;
    }

    public int Height
    {
        get => Size.Y;
        set => Size.Y = value;
    }
    
    public void SetEnabled(bool enabled)
    {
        if (State == State.Disabled && enabled) State = State.Default;
        else if (!enabled) State = State.Disabled;
    }
    
    public bool ContainsPoint(Coord pos)
    {
        return pos.X >= Position.X && pos.X < Position.X + Width &&
               pos.Y >= Position.Y && pos.Y < Position.Y + Height;
    }
    
    public virtual void Remove()
    {
        Input.Unregister(this);
    }

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);
    public event MouseEventHandler? MouseEnter;
    public event MouseEventHandler? MouseLeave;
    public event MouseEventHandler? MouseDown;
    public event MouseEventHandler? MouseLeftDown;
    public event MouseEventHandler? MouseRightDown;
    public event MouseEventHandler? MouseMove;

    public virtual void OnMouseEnter(MouseEventArgs e)
    {
        IsMouseOver = true;

        MouseEnter?.Invoke(this, e);
    }

    public virtual void OnMouseExit(MouseEventArgs e)
    {
        IsMouseOver = false;
        MouseLeave?.Invoke(this, e);
    }

    public virtual void OnMouseMove(MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }

    public virtual void OnMouseLeftDown(MouseEventArgs e)
    {
        MouseLeftDown?.Invoke(this, e);
        MouseDown?.Invoke(this, e);
    }

    public virtual void OnMouseLeftUp() { }

    public virtual void OnMouseRightDown(MouseEventArgs e)
    {
        MouseRightDown?.Invoke(this, e);
        MouseDown?.Invoke(this, e);
    }

    public virtual void OnMouseRightUp() { }

    public virtual void OnGotFocus(MouseEventArgs e)
    {
        IsFocused = true;
    }

    public virtual void OnLostFocus(MouseEventArgs e)
    {
        IsFocused = false;
    }
}

public enum State
{
    Default,
    Highlighted,
    Pressed,
    Disabled
}

[Flags]
public enum MouseEventMask
{
    None = 0,
    MouseMove = 1,
    MouseClick = 1 << 1,
    MouseDoubleClick = 1 << 2,
    MouseWheel = 1 << 3,
    All = MouseMove | MouseClick | MouseDoubleClick | MouseWheel
}
