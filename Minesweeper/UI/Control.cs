using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public abstract class Control : VisualComponent
{
    protected Control() => Input.Register(this);

    public bool IsMouseOver { get; private set; }
    
    public bool Focused { get; private set; }
    public bool Focusable { get; set; } = true;
    public bool ShowFocusedBorder { get; set; } = true;
    public BorderStyle FocusBorderStyle { get; set; } = BorderStyle.Dotted;
    public Color FocusBorderColor { get; set; } = Color.Black;
    
    public int TabIndex { get; set; }

    public override void Render()
    {
        base.Render();
        
        // Focus border should not override normal border
        if (Focused && ShowFocusedBorder && !ShowBorder)
        {
            Display.DrawBorder(GlobalPosition, Size, FocusBorderColor, FocusBorderStyle);
        }
    }

    public override void Remove()
    {
        Input.Unregister(this);
        base.Remove();
    }

    public delegate void MouseEventHandler(Control sender, MouseEventArgs e);
    public delegate void KeyboardEventHandler(Control sender, KeyboardEventArgs e);
    public delegate void FocusEventHandler(Control sender, InputEventArgs e);
    
    public event MouseEventHandler? MouseEnter;
    public event MouseEventHandler? MouseLeave;
    public event MouseEventHandler? MouseDown;
    public event MouseEventHandler? MouseLeftDown;
    public event MouseEventHandler? MouseRightDown;
    public event MouseEventHandler? MouseMove;

    public event KeyboardEventHandler? KeyDown;
    public event KeyboardEventHandler? KeyUp;

    public event FocusEventHandler? GotFocus;
    public event FocusEventHandler? LostFocus;

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

            default:
                throw new ArgumentOutOfRangeException(nameof(mouseEventType), mouseEventType, null);
        }

        if (e.Handled) return;

        e.Source = this;
        (Parent as Control)?.SendMouseEvent(mouseEventType, e);
    }

    public void SendKeyboardEvent(KeyboardEventType keyboardEventType, KeyboardEventArgs e)
    {
        if (keyboardEventType == KeyboardEventType.KeyDown)
        {
            OnKeyDown(e);
            return;
        }
        
        OnKeyUp(e);
    }

    public void SendFocusEvent(FocusEventType focusEventType, InputEventArgs e)
    {
        if (e.OriginalSource != this) return;
        
        if (focusEventType == FocusEventType.GotFocus)
        {
            OnGotFocus(e);
            return;
        }
        
        OnLostFocus(e);
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

    protected virtual void OnGotFocus(InputEventArgs e)
    {
        Focused = true;
        GotFocus?.Invoke(this, e);
    }

    protected virtual void OnLostFocus(InputEventArgs e)
    {
        Focused = false;
        LostFocus?.Invoke(this, e);
    }

    protected virtual void OnKeyDown(KeyboardEventArgs e)
    {
        KeyDown?.Invoke(this, e);
    }

    protected virtual void OnKeyUp(KeyboardEventArgs e)
    {
        KeyUp?.Invoke(this, e);
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
    MouseRightUp
}

public enum KeyboardEventType
{
    KeyDown,
    KeyUp
}

public enum FocusEventType
{
    GotFocus,
    LostFocus
}
