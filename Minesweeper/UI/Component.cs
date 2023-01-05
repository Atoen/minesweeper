namespace Minesweeper.UI;

public abstract class Component
{
    public event EventHandler<PositionChangedEventArgs>? PositionChanged;
    public event EventHandler<SizeChangedEventArgs>? SizeChanged; 
    public State State { get; protected set; } = State.Default;
    public bool Enabled { get; protected set; } = true;
    public bool AutoResize { get; set; } = true;
    
    private int _zIndex;
    public int ZIndex
    {
        get
        {
            if (ZIndexUpdateMode == ZIndexUpdateMode.Manual || Parent == null) return _zIndex;
            
            return ZIndexUpdateMode switch
            {
                ZIndexUpdateMode.SameAsParent => Parent.ZIndex,
                ZIndexUpdateMode.OneHigherThanParent => Parent.ZIndex + 1,
                _ => _zIndex
            };
        }
        set
        {
            if (ZIndexUpdateMode == ZIndexUpdateMode.Manual) _zIndex = value;
        }
    }

    public ZIndexUpdateMode ZIndexUpdateMode { get; set; } = ZIndexUpdateMode.OneHigherThanParent;

    private Component? _parent;
    public Component? Parent
    {
        get => _parent;
        set => SetParent(value);
    }

    private void SetParent(Component? value)
    {
        if (value == this)
        {
            throw new InvalidOperationException("Component cannot be its own parent.");
        }

        if (value is null)
        {
            if (_parent != null) _parent.PositionChanged -= OnPositionChanged;
            GlobalPosition = Position;
            _parent = null;

            return;
        }

        _parent = value;
        _parent.PositionChanged += OnPositionChanged;
    }

    private Coord _parentOffset;

    public Coord Position
    {
        get => Parent is null ? GlobalPosition :  _parentOffset + Parent.Position;
        set
        {
            if (value != Position)
            {
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(Position, value));
            }
            
            if (Parent is null)
            {
                GlobalPosition = value;
                _parentOffset = Coord.Zero;
                return;
            }

            _parentOffset = value;
            GlobalPosition = Parent.Position + _parentOffset;
        }
    }

    public Coord GlobalPosition;

    public Coord Center
    {
        get => Position + Size / 2;
        set => Position = value - Size / 2;
    }

    public Coord Size;
    
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
    
    public virtual void Resize() {}

    public void SetEnabled(bool enabled)
    {
        if (!Enabled && enabled) State = State.Default;
        else if (!enabled) State = State.Default;

        Enabled = enabled;
    }
    
    public bool ContainsPoint(Coord pos)
    {
        return pos.X >= Position.X && pos.X < Position.X + Width &&
               pos.Y >= Position.Y && pos.Y < Position.Y + Height;
    }

    private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(sender, e);
    }
}

public enum State
{
    Default,
    Highlighted,
    Pressed
}

public enum ZIndexUpdateMode
{
    SameAsParent,
    OneHigherThanParent,
    Manual
}

public class PositionChangedEventArgs : EventArgs
{
    public PositionChangedEventArgs(Coord oldPosition, Coord newPosition)
    {
        OldPosition = oldPosition;
        NewPosition = newPosition;
    }

    public Coord OldPosition { get; }
    public Coord NewPosition { get; }
    public Coord Delta => NewPosition - OldPosition;
}

public class SizeChangedEventArgs : EventArgs
{
    public SizeChangedEventArgs(Coord oldSize, Coord newSize)
    {
        OldSize = oldSize;
        NewSize = newSize;
    }

    public Coord OldSize { get; }
    public Coord NewSize { get; }
}
