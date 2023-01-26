namespace Minesweeper.UI;

public abstract class Component
{
    public State State { get; protected set; } = State.Default;
    public bool Enabled { get; protected set; } = true;
    
    public ResizeMode ResizeMode { get; set; } = ResizeMode.Stretch;
    public ZIndexUpdateMode ZIndexUpdateMode { get; set; } = ZIndexUpdateMode.OneHigherThanParent;
    
    private int _zIndex;
    public virtual int ZIndex
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
    
    public delegate void PositionChangeEventHandler(object sender, PositionChangedEventArgs e);
    public delegate void SizeChangeEventHandler(object sender, SizeChangedEventArgs e);
    
    public event PositionChangeEventHandler? PositionChanged;
    public event SizeChangeEventHandler? SizeChanged;

    private Coord _localPosition;
    private Coord _globalPosition;
    
    public Coord Position
    {
        get => _localPosition;
        set => SetPositionInternal(value, true);
    }

    public Coord GlobalPosition
    {
        get => _parent == null ? _localPosition : _parent.GlobalPosition + _localPosition;
        set => SetPositionInternal(value, false);
    }

    private void SetPositionInternal(Coord value, bool isLocal)
    {
        var positionBefore = isLocal ? _localPosition : _globalPosition;

        if (_parent == null)
        {
            _globalPosition = value;
            _localPosition = value;
        }
        else
        {
            if (isLocal)
            {
                _localPosition = value;
                _globalPosition = _localPosition + _parent.GlobalPosition;
            }
            else
            {
                _globalPosition = value;
                _localPosition = value - _parent.GlobalPosition;
            }
        }

        if (value != positionBefore)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(value - positionBefore));
        }
    }

    public Coord Center
    {
        get => GlobalPosition + Size / 2;
        set
        {
            GlobalPosition = value - Size / 2;

            if (_parent != null)
            {
                Position = GlobalPosition - _parent.GlobalPosition;
            }
        }
    }

    private Coord _size;

    public Coord Size
    {
        get => _size;
        set
        {
            var sizeBefore = _size;
            _size = value;

            if (value != sizeBefore)
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs(sizeBefore, value));
            }
        }
    }

    public int Width
    {
        get => _size.X;
        set
        {
            var widthBefore = _size.X;
            _size.X = value;
            
            if (value != widthBefore)
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs(_size with {X = widthBefore}, _size));
            }
        }
    }

    public int Height
    {
        get => _size.Y;
        set
        {
            var heightBefore = _size.Y;
            _size.Y = value;
            
            if (value != heightBefore)
            {
                SizeChanged?.Invoke(this, new SizeChangedEventArgs(_size with {Y = heightBefore}, _size));
            }
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
        return pos.X >= GlobalPosition.X && pos.X < GlobalPosition.X + Width &&
               pos.Y >= GlobalPosition.Y && pos.Y < GlobalPosition.Y + Height;
    }

    private int GetZIndex()
    {
        if (ZIndexUpdateMode == ZIndexUpdateMode.Manual || _parent == null) return _zIndex;

        return ZIndexUpdateMode switch
        {
            ZIndexUpdateMode.SameAsParent => _parent.ZIndex,
            ZIndexUpdateMode.OneHigherThanParent => _parent.ZIndex + 1,
            ZIndexUpdateMode.TwoHigherThatParent => _parent.ZIndex + 2,
            _ => _zIndex
        };
    }

    private void SetParent(Component? value)
    {
        if (value == this)
        {
            throw new InvalidOperationException($"Component {value} cannot be its own parent.");
        }

        if (value != _parent && _parent != null)
        {
            _parent.PositionChanged -= OnPositionChanged;
            SizeChanged -= _parent.OnSizeChanged;
        }

        if (value == null)
        {
            Position = GlobalPosition;
            _parent = null;
            
            return;
        }

        _parent = value;
        _localPosition = _globalPosition - _parent.GlobalPosition;

        _parent.PositionChanged += OnPositionChanged;
        SizeChanged += _parent.OnSizeChanged;
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        PositionChanged?.Invoke(sender, e);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
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
    TwoHigherThatParent,
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
    public PositionChangedEventArgs(Coord delta) => Delta = delta;

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
