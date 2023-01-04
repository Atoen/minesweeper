using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Text : Content
{
    public Text(string text)
    {
        _text = text;
        Foreground = Color.Black;
        Size = new Coord(Length, 1);
    }

    public Text(string text, Color foreground)
    {
        _text = text;
        Foreground = foreground;
        Size = new Coord(Length, 1);
    }
    
    public Text(string text, Color foreground, Color background) : this(text, foreground) => Background = background;

    public TextMode Mode { get; set; } = TextMode.Default;
    public Alignment Alignment { get; set; } = Alignment.Center;

    private string _text;

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
        Display.Print(Position.X, Position.Y, String, Foreground, Background ?? Parent.Color, Alignment, Mode);
    }

    public override void Clear()
    {
        var startPos = Alignment switch
        {
            Alignment.Left => Position,
            Alignment.Right => new Coord(Position.X - Length, Position.Y),
            _ => new Coord(Position.X - Length / 2, Position.Y)
        };
        
        Display.ClearRect(startPos, Size);
    }
}