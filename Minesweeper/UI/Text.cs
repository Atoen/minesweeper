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
        String = text;
        Foreground = foreground;
        Background = Color.Transparent;

        ZIndexUpdateMode = ZIndexUpdateMode.SameAsParent;
    }

    public Text(string text, Color foreground, Color background) : this(text, foreground)
    {
        Background = background;
    }

    private VisualComponent _parent = null!;
    public new VisualComponent Parent
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
    
    private string _text = null!;

    public string String
    {
        get => _text;
        set
        {
            _text = value;
            Size = new Coord(Length, 1);
        }
    }

    public int Length => _text.Length;
    
    public override void Render()
    {
        var background = Background == Color.Transparent ? Parent.Color : Background;
        var position = Parent.Center;
        
        Display.Print(position.X, position.Y, _text, Foreground, background, Alignment, TextMode);
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

    public override string ToString() => _text;
}
