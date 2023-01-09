using Minesweeper.UI.Events;
using Serilog;

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

    public void SendMouseEvent(MouseEventType mouseEventType, MouseEventArgs e)
    {
        switch (mouseEventType)
        {
            case MouseEventType.MouseMove:
                OnMouseMove(e);
                break;
            
            case MouseEventType.MouseEnter:
                OnMouseEnter(e);
                break;
            
            case MouseEventType.MouseExit:
                OnMouseExit(e);
                break;
            
            case MouseEventType.MouseLeftDown:
                OnMouseLeftDown(e);
                break;
            
            case MouseEventType.MouseLeftUp:
                OnMouseLeftUp();
                break;
            
            case MouseEventType.MouseRightDown:
                OnMouseRightDown(e);
                break;
            
            case MouseEventType.MouseRightUp:
                OnMouseRightUp();
                break;
            
            case MouseEventType.GotFocus:
                OnGotFocus(e);
                break;
            
            case MouseEventType.LostFocus:
                OnLostFocus(e);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mouseEventType), mouseEventType, null);
        }
        
        if (e.Handled) return;

        e.Source = this;
        (Parent as Control)?.SendMouseEvent(mouseEventType, e);
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

public enum MouseEventType
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
}
