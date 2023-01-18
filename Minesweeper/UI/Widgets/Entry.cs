using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI.Widgets;

public class Entry : Widget
{
    public EntryText Text { get; init; } = new("", Color.Black);
    public Coord TextOffset = Coord.Zero;
    public TextMode EntryTextMode { get; set; } = TextMode.Italic;

    public Color TextBackground { get; set; } = Color.Gray;
    public int MaxTextLenght { get; set; }

    public EntryMode InputMode { get; init; } = EntryMode.All;

    private bool _inEntryMode;

    public override void Render()
    {
        if (_inEntryMode && Enabled) Text.Cycle();

        Display.DrawRect(Position, Size, CurrentColor);
        
        var textStart = Center + TextOffset + Coord.Left * (MaxTextLenght / 2);

        Display.DrawRect(textStart, (MaxTextLenght + 1, 1), TextBackground);

        Display.Print(
            Center.X + TextOffset.X - MaxTextLenght / 2,
            Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            Text.Background ?? TextBackground, Alignment.Left,
            Text.Mode);
    }

    private void KeyEvent(KeyboardState obj)
    {
        if (!_inEntryMode || !obj.Pressed || !Enabled) return;

        if (obj.Key == ConsoleKey.Enter)
        {
            ExitEntryMode();
            return;
        }

        var symbol = obj.Char;

        if (CheckIfAllowed(symbol) && Text.Length < MaxTextLenght)
        {
            Text.Append(symbol);
        }
        
        if (obj.Key == ConsoleKey.Backspace && Text.Length > 0)
        {
            Text.RemoveLast(1);
        }
    }

    private void ExitEntryMode()
    {
        if (!_inEntryMode) return;
        
        _inEntryMode = false;
        Text.Animating = false;
        Text.Mode = TextMode.Default;

        if (InputMode != EntryMode.Digits) return;
        
        if (string.IsNullOrWhiteSpace(Text.Text)) Text.Text = "0";
    }

    protected override void OnMouseLeftDown(MouseEventArgs e)
    {
        _inEntryMode = !_inEntryMode;
        Text.Mode = _inEntryMode ? EntryTextMode : TextMode.Default;
        
        Text.Animating = _inEntryMode;
        
        base.OnMouseLeftDown(e);
    }

    protected override void OnLostFocus(MouseEventArgs e)
    {
        ExitEntryMode();
        
        base.OnLostFocus(e);
    }

    // protected override void Resize()
    // {
    //     var minSize = new Coord(MaxTextLenght + 1 + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    //
    //     Size = Size.ExpandTo(minSize);
    // }

    private bool CheckIfAllowed(char symbol)
    {
        return InputMode switch
        {
            EntryMode.Alphanumeric => char.IsLetterOrDigit(symbol),
            EntryMode.Letters => char.IsLetter(symbol),
            EntryMode.Digits => char.IsDigit(symbol),
            _ => !char.IsControl(symbol)
        };
    }
}

public enum EntryMode
{
    All,
    Alphanumeric,
    Letters,
    Digits,
}
