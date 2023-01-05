namespace Minesweeper.UI;

public abstract class Component
{
    public event EventHandler<PositionChangedEventArgs>? PositionChanged;
    public event EventHandler<SizeChangedEventArgs>? SizeChanged; 
    
    public State State { get; protected set; } = State.Default;
    public bool Enabled { get; protected set; } = true;
    
    public ResizeMode ResizeMode { get; set; } = ResizeMode.Stretch;
    public ZIndexUpdateMode ZIndexUpdateMode { get; set; } = ZIndexUpdateMode.OneHigherThanParent;
    
    private int _zIndex;
    public int ZIndex
    {
        get => GetZIndex();
        set
        {
            if (ZIndexUpdateMode == ZIndexUpdateMode.Manual) _zIndex = value;
        }
    }

    private Component? _parent;
    public Component? Parent
    {
        get => _parent;
        set => SetParent(value);
    }

    private Coord _parentOffset;

    public Coord GlobalPosition;

    public Coord Position
    {
        get => _parent == null ? GlobalPosition : _parent.Position + _parentOffset;
        set => SetPosition(value);
    }
    
    public Coord Center
    {
        get => Position + Size / 2;
        set => Position = value - Size / 2;
    }
    
    private Coord _size;
    public Coord Size
    {
        get => _size;
        set
        {
            if (value != _size)
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs(_size, value));
            }

            _size = value;
        }
    }

    public int Width
    {
        get => _size.X;
        set
        {
            if (value != _size.X)
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs(_size, _size with {X = value}));
            }
            
            _size.X = value;
        }
    }

    public int Height
    {
        get => _size.Y;
        set
        {
            if (value != _size.Y)
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs(_size, _size with {Y = value}));
            }
            
            _size.Y = value;
        }
    }

    public virtual void Resize()
    {
    }

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
    
    private int GetZIndex()
    {
        if (ZIndexUpdateMode == ZIndexUpdateMode.Manual || _parent == null) return _zIndex;

        return ZIndexUpdateMode switch
        {
            ZIndexUpdateMode.SameAsParent => _parent.ZIndex,
            ZIndexUpdateMode.OneHigherThanParent => _parent.ZIndex + 1,
            _ => _zIndex
        };
    }

    private void SetParent(Component? value)
    {
        if (value == this)
        {
            throw new InvalidOperationException("Component cannot be its own parent.");
        }

        if (value != _parent && _parent != null)
        {
            _parent.PositionChanged -= OnPositionChanged;
            SizeChanged -= _parent.OnSizeChanged;
            
            GlobalPosition = Position;
        }

        if (value is null)
        {
            _parent = null;
            _parentOffset = Coord.Zero;
            return;
        }

        _parent = value;
        
        _parent.PositionChanged += OnPositionChanged;
        SizeChanged += _parent.OnSizeChanged;
    }

    private void SetPosition(Coord value)
    {
        if (value != Position)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(value - Position));
        }

        if (_parent is null)
        {
            GlobalPosition = value;
            return;
        }

        _parentOffset = value;
        GlobalPosition = _parent.Position + _parentOffset;
    }

    private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(sender, e);
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SizeChanged?.Invoke(sender, e);

        if (sender != this && ResizeMode != ResizeMode.Manual)
        {
            Resize();
        }
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

public enum ResizeMode
{
    Grow,
    Stretch,
    Manual
}

public class PositionChangedEventArgs : EventArgs
{
    public PositionChangedEventArgs(Coord delta)
    {
        Delta = delta;
    }

    public Coord Delta { get; }
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
