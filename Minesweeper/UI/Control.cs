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
