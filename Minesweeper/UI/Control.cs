using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public abstract class Control : VisualComponent
{
    protected Control()
    {
        Input.Register(this);
    }

    public bool IsMouseOver { get; protected set; }
    public bool IsFocused { get; protected set; }

    public bool IsFocusable { get; set; } = true;
    
    public override void Remove()
    {
        Input.Unregister(this);
        base.Remove();
    }

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);
    public event MouseEventHandler? MouseEnter;
    public event MouseEventHandler? MouseLeave;
    public event MouseEventHandler? MouseDown;
    public event MouseEventHandler? MouseLeftDown;
    public event MouseEventHandler? MouseRightDown;
    public event MouseEventHandler? MouseMove;

    public void SendMouseEvent(EventType eventType, MouseEventArgs e)
    {
        switch (eventType)
        {
            case EventType.MouseMove:
                OnMouseMove(e);
                break;
            case EventType.MouseEnter:
                OnMouseEnter(e);
                break;
            
            case EventType.MouseExit:
                OnMouseExit(e);
                break;
            
            case EventType.MouseLeftDown:
                OnMouseLeftDown(e);
                break;
            
            case EventType.MouseLeftUp:
                OnMouseLeftUp();
                break;
            
            case EventType.MouseRightDown:
                OnMouseRightDown(e);
                break;
            
            case EventType.MouseRightUp:
                OnMouseRightUp();
                break;
            
            case EventType.GotFocus:
                OnGotFocus(e);
                break;
            
            case EventType.LostFocus:
                OnLostFocus(e);
                break;

            case EventType.KeyDown:
                break;

            case EventType.KeyUp:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
        
        if (e.Handled) return;

        e.Source = this;
        (Parent as Control)?.SendMouseEvent(eventType, e);
    }

    protected virtual void OnMouseEnter(MouseEventArgs e)
    {
        IsMouseOver = true;

        MouseEnter?.Invoke(this, e);
    }

    protected virtual void OnMouseExit(MouseEventArgs e)
    {
        IsMouseOver = false;
        MouseLeave?.Invoke(this, e);
    }

    protected virtual void OnMouseMove(MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }

    protected virtual void OnMouseLeftDown(MouseEventArgs e)
    {
        MouseLeftDown?.Invoke(this, e);
        MouseDown?.Invoke(this, e);
    }

    protected virtual void OnMouseLeftUp() { }

    protected virtual void OnMouseRightDown(MouseEventArgs e)
    {
        MouseRightDown?.Invoke(this, e);
        MouseDown?.Invoke(this, e);
    }

    protected virtual void OnMouseRightUp() { }

    protected virtual void OnGotFocus(MouseEventArgs e)
    {
        IsFocused = true;
    }

    protected virtual void OnLostFocus(MouseEventArgs e)
    {
        IsFocused = false;
    }
}

public enum EventType
{
    MouseMove,
    MouseEnter,
    MouseExit,
    MouseLeftDown,
    MouseLeftUp,
    MouseRightDown,
    MouseRightUp,

    GotFocus,
    LostFocus,
    
    KeyDown,
    KeyUp
}
