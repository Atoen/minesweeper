using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class TString : IText
{
    public string Text { get; set; }
    public bool Enabled { get; set; }
    
    public bool Animating { get; set; }
    
    public Color Foreground { get; set; }
    public Color? Background { get; set; }
    
    public TextMode Mode { get; set; }
    
    private readonly Timer _timer;

    public TString(Color foreground, Color? background = null)
        : this(false, TimeSpan.Zero, foreground, background) {}

    public TString(bool countingDown, TimeSpan timeSpan, Color foreground, Color? background = null)
    {
        Foreground = foreground;
        Background = background;

        _timer = new Timer(timeSpan, countingDown);

        Text = _timer.String;

        Enabled = Animating = true;
        
        _timer.Elapsed += delegate { Enabled = false; };
    }

    public TString(Timer timer, Color foreground, Color? background = null)
    {
        Foreground = foreground;
        Background = background;

        _timer = timer;
        
        Text = _timer.String;

        Enabled = Animating = true;
        
        _timer.Elapsed += delegate { Enabled = false; };
    }
    
    public void Cycle()
    {
        if (Enabled) Text = _timer.String;
    }

    public void RestartTimer()
    {
        _timer.StartNow();
    }
}