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
    
    public Text(string text, Color foreground, Color background) : this(text, foreground)
    {
        Background = background;
    }

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
        Display.Print(Position.X, Position.Y, String, Foreground, Background ?? Parent.Color);
    }

    public override void Clear()
    {
        Display.ClearRect(Position, Size);
    }
}