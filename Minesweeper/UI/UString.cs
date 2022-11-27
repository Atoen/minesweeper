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
        get => _displayingPlaceholder ? _replacement : _text;
        private set => _text = value;
    }

    public char[] PlaceholderArray = {'_'};
    
    public int FramesPerSymbol = 10;

    public Color Foreground;
    public Color? Background;
    
    private string _replacement = "";
    private string _text;
    private bool _displayingPlaceholder;
    private readonly IEnumerator _cycler;

    public bool Animating { get; set; }
    public bool Enabled { get; set; } = true;

    public UString(string text, Color foreground, Color? background = null)
    {
        _text = text;

        Foreground = foreground;
        Background = background;

        _cycler = CycleText();
    }

    public void Cycle() => _cycler.MoveNext();

    private IEnumerator CycleText()
    {
        var i = 0;
        var j = 0;

        while (Enabled)
        {
            if (!Animating) yield return null;

            i++;

            Console.Title = i.ToString();
            
            if (i < FramesPerSymbol) continue;

            _displayingPlaceholder = !_displayingPlaceholder;

            i = 0;
            _replacement = new string(PlaceholderArray[j], Text.Length);
                
            j++;
            j %= PlaceholderArray.Length;

            yield return null;
        }
    }
}