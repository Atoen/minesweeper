using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Entry : Widget
{
    public EntryText Text { get; init; } = new("", Color.Black);
    public Coord TextOffset = Coord.Zero;
    public TextMode EntryTextMode { get; set; } = TextMode.Italic;

    public Color TextBackground { get; set; } = Color.Gray;
    public int MaxTextLenght { get; set; }

    public TextEntryMode InputMode { get; init; } = TextEntryMode.All;

    private bool _inEntryMode;

    public Entry()
    {
        MouseEventMask = MouseEventMask.MouseClick;
    }
    
    // public override Entry Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    // {
    //     return base.Grid<Entry>(row, column, rowSpan, columnSpan, alignment);
    // }
    //
    // public override Entry Place(int posX, int posY)
    // {
    //     return base.Grid<Entry>(posX, posY);
    // }

    public override void Render()
    {
        if (_inEntryMode && State != UI.State.Disabled) Text.Cycle();

        Display.DrawRect(Position, Size, Color);
        
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
        if (!_inEntryMode || !obj.Pressed || State == UI.State.Disabled) return;

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

        if (InputMode != TextEntryMode.Digits) return;
        
        if (string.IsNullOrWhiteSpace(Text.Text)) Text.Text = "0";
    }

    protected override void OnMouseLeftDown()
    {
        _inEntryMode = !_inEntryMode;
        Text.Mode = _inEntryMode ? EntryTextMode : TextMode.Default;
        
        Text.Animating = _inEntryMode;
    }

    protected override void OnLostFocus()
    {
        ExitEntryMode();
    }

    private void LeftClick(MouseState obj)
    {
        if (State == UI.State.Disabled) return;
        
        if (ContainsPoint(obj.Position))
        {
            _inEntryMode = !_inEntryMode;
            Text.Mode = _inEntryMode ? EntryTextMode : TextMode.Default;
        }
        else
        {
            ExitEntryMode();
        }
        
        Text.Animating = _inEntryMode;
    }
    
    protected override void Resize()
    {
        var minSize = new Coord(MaxTextLenght + 1 + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);

        Size = Size.ExpandTo(minSize);
    }

    private bool CheckIfAllowed(char symbol)
    {
        return InputMode switch
        {
            TextEntryMode.Alphanumeric => char.IsLetterOrDigit(symbol),
            TextEntryMode.Letters => char.IsLetter(symbol),
            TextEntryMode.Digits => char.IsDigit(symbol),
            _ => !char.IsControl(symbol)
        };
    }
}

public enum TextEntryMode
{
    All,
    Alphanumeric,
    Letters,
    Digits,
}
