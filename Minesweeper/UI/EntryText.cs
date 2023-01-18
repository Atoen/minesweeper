using System.Collections;
using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class EntryText : Content, IAnimatedText
{
    public string Text
    {
        get => _displayingCaret ? $"{_text}{Caret}" : _text;
        set
        {
            _text = value;
            Size = new Coord(Length + 1, 1);
        }
    }

    public char Caret { get; set; } = '_';
    
    public int CaretCycleSpeed { get; set; } = 10;

    public TextMode Mode { get; set; }
    
    private string _text;
    private bool _displayingCaret;
    private readonly IEnumerator _enumerator;
    private bool _animating;

    public bool Animating
    {
        get => _animating;
        set
        {
            if (value == false) _displayingCaret = false;
            _animating = value;
        }
    }
    
    public int Length => _text.Length;

    public EntryText(string text, Color foreground, Color? background = null)
    {
        _text = text;
        
        Foreground = foreground;
        Background = background;

        _enumerator = CycleText();

        Size = new Coord(Length + 1, 1);
    }

    public void Cycle() => _enumerator.MoveNext();

    public void Append(string newText) => _text += newText;
    public void Append(char symbol) => _text += symbol;

    public void RemoveLast(int n) => _text = _text[..^n];

    public static implicit operator EntryText(string s) => new(s, Color.Black);

    private IEnumerator CycleText()
    {
        var i = 0;

        while (Enabled)
        {
            i++;
            
            if (i >= CaretCycleSpeed)
            {
                _displayingCaret = !_displayingCaret;
                i = 0;
            }

            yield return null;
        }
    }

    public override string ToString() => _text;
    public override void Render()
    {
        if (Animating) Cycle();
        
        Display.Print(Position.X, Position.Y, Text, Foreground, Background ?? Parent.CurrentColor);
    }

    public override void Clear()
    {
        Display.ClearRect(Position, Size);
    }
}

public interface IAnimatedText
{
    public void Cycle();
    
    string Text { get; set; }
    TextMode Mode { get; set; }
    bool Animating { get; set; }
    int Length => Text.Length;
}