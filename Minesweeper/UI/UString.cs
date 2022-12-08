using System.Collections;

namespace Minesweeper.UI;

public class UString
{
    public static readonly UString Empty = new("", Color.Black)
    {
        Enabled = false
    };

    public string Text
    {
        get => _displayingCaret ? $"{_text}{Caret}" : _text;
        set => _text = value;
    }

    public int Length => _text.Length;
    public char Caret { get; set; } = '_';
    
    public int CaretCycleSpeed { get; set; } = 10;

    public Color Foreground { get; set; }
    public Color? Background { get; set; }
    
    private string _text;
    private bool _displayingCaret;
    private readonly IEnumerator _cycler;
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

    public bool Enabled { get; set; } = true;

    public UString(string text, Color foreground, Color? background = null)
    {
        _text = text;
        
        Foreground = foreground;
        Background = background;

        _cycler = CycleText();
    }

    public void Cycle() => _cycler.MoveNext();

    public void Append(string newText) => _text += newText;
    public void Append(char symbol) => _text += symbol;

    public void RemoveLast(int n) => _text = _text[..^n];

    public static implicit operator UString(string s) => new(s, Color.Black);

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
}