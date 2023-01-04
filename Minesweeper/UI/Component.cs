namespace Minesweeper.UI;

public abstract class Component
{
    public State State { get; protected set; } = State.Default;
    public bool Enabled { get; protected set; } = true;
    public bool AutoResize { get; set; } = true;

    private Component? _parent;
    public Component? Parent
    {
        get => _parent;
        set
        {
            if (value is null)
            {
                if (_parent != null) _parent.PositionChanged -= OnPositionChanged;
                _parent = value;
                
                return;
            }

            _parent = value;

            _parent.PositionChanged += OnPositionChanged;
        }
    }

    public Coord Size;

    public event EventHandler<PositionChangedEventArgs>? PositionChanged; 

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

public class PositionChangedEventArgs : EventArgs
{
    public PositionChangedEventArgs(Coord oldPosition, Coord newPosition)
    {
        OldPosition = oldPosition;
        NewPosition = newPosition;
    }

    public readonly Coord OldPosition;
    public readonly Coord NewPosition;
}
