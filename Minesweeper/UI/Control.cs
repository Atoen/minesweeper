using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public abstract class Control
{
    protected Control(IContainer? parent = null)
    {
        Parent = parent;
        
        Input.Register(this);
    }
    
    public IContainer? Parent { get; }

    public bool IsMouseOver { get; protected set; }
    public bool IsFocused { get; protected set; }
    public State State { get; protected set; } = State.Default;

    public MouseEventMask MouseEventMask { get; protected set; } = MouseEventMask.None;
    public bool IsEnabled => State != State.Disabled;
    public bool UsesMouseEvents => MouseEventMask != MouseEventMask.None;
    public Layer Layer { get; set; } = Layer.Foreground;
    
    public bool AutoResize { get; init; } = true;
    
    public Coord Size;
    public Coord Position;
    public Coord Center => Position + Size / 2;
    
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

    public void MouseLeftDown() => OnMouseLeftDown();

    public void MouseExit() 
    {
        if (!IsMouseOver) return;

        IsMouseOver = false;
        OnMouseExit();
    }

    public void MouseMove(MouseState state)
    {
        if (!IsMouseOver)
        {
            IsMouseOver = true;
            OnMouseEnter();
        }
        
        OnMouseMove(state);
    }

    public void LostFocus()
    {
        IsFocused = false;
        OnLostFocus();
    }

    public void GotFocus()
    {
        IsFocused = true;
        OnGotFocus();
    }

    protected virtual void OnMouseLeftDown() { }
    
    protected virtual void OnMouseEnter()
    {
        State = State.Highlighted;
    }

    protected virtual void OnMouseExit()
    {
        State = State.Default;
    }

    protected virtual void OnMouseMove(MouseState state)
    {
        if (state.Button == MouseButton.None) State = State.Highlighted;
    }

    protected virtual void OnGotFocus() { }

    protected virtual void OnLostFocus() { }
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
    MouseWheel = 1 << 3
}

public static class EnumExtensions
{
    public static bool HasValue<T>(this T value, T flag) where T : Enum
    {
        var f = Convert.ToInt32(flag);
        return (Convert.ToInt32(value) & f) == f;
    }
}
