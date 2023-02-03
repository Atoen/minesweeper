using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Widgets;

namespace Minesweeper.UI;

public class Text : VisualComponent
{
    public Text(string text) : this(text, Color.Black)
    {
        Background = Color.Transparent;
    }
    
    public Text(string text, Color foreground)
    {
        TextInternal = text;
        Size = new Coord(Length, 1);

        Foreground = foreground;
        Background = Color.Transparent;

        ZIndexUpdateMode = ZIndexUpdateMode.OneHigherThanParent;
    }

    public Text(string text, Color foreground, Color background) : this(text, foreground)
    {
        Background = background;
    }

    private VisualComponent? _parent;
    public new VisualComponent? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            base.Parent = value;
        }
    }

    public Color Foreground { get; set; }
    public Color Background
    {
        get => DefaultColor;
        set => DefaultColor = value;
    }

    public TextMode TextMode { get; set; } = TextMode.Default;
    public Alignment Alignment { get; set; } = Alignment.Center;

    protected string TextInternal;

    public virtual string String
    {
        get => TextInternal;
        set
        {
            TextInternal = value;
            Size = new Coord(Length, 1);
        }
    }

    public int Length => TextInternal.Length;
    
    public override void Render()
    {
        if (_parent == null) return;
        
        var background = Background == Color.Transparent ? _parent.CurrentColor : Background;
        var position = _parent.Center;
        
        Display.Print(position.X, position.Y, TextInternal, Foreground, background, Alignment, TextMode);
    }
    
    public override void Clear()
    {
        var startPos = Alignment switch
        {
            Alignment.Left => GlobalPosition,
            Alignment.Right => new Coord(GlobalPosition.X - Length, GlobalPosition.Y),
            _ => new Coord(GlobalPosition.X - Length / 2, GlobalPosition.Y)
        };
        
        Display.ClearRect(startPos, Size);
    }

    public override string ToString() => TextInternal;
}
