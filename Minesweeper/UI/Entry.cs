namespace Minesweeper.UI;

public class Entry : Widget
{
    public UString Text { get; set; } = new("", Color.Black);
    public Coord TextOffset = Coord.Zero;

    public Color TextBackground { get; set; } = Color.Gray;
    public int MaxTextLenght { get; set; }

    public TextEntryMode Mode { get; set; } = TextEntryMode.All;

    private bool _inEntryMode;
    
    public Entry(Frame parent) : base(parent)
    {
        Input.MouseLeftClick += LeftClick;
        Input.KeyEvent += KeyEvent;
    }

    public override void Render()
    {
        if (_inEntryMode) Text.Cycle();
        
        base.Render();
        
        Display.Display.DrawRect(Center + TextOffset + Coord.Left * (MaxTextLenght / 2), (MaxTextLenght + 1, 1), TextBackground);
        
        Display.Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            Text.Background ?? TextBackground);
    }

    private void KeyEvent(KeyboardState obj)
    {
        if (!_inEntryMode || !obj.Pressed) return;

        if (obj.Key == ConsoleKey.Enter)
        {
            _inEntryMode = false;
            Text.Animating = false;
            return;
        }

        var symbol = obj.Char;

        if (CheckIfAllowed(symbol) && Text.Lenght < MaxTextLenght)
        {
            Text.Append(symbol);
        }
        
        if (obj.Key == ConsoleKey.Backspace && Text.Lenght > 0)
        {
            Text.RemoveLast(1);
        }
    }

    private void LeftClick(MouseState obj)
    {
        if (IsCursorOver(obj.Position))
        {
            _inEntryMode = !_inEntryMode;
        }
        else
        {
            _inEntryMode = false;
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
        return Mode switch
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