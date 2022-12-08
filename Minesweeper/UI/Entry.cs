using Minesweeper.Display;

namespace Minesweeper.UI;

public class Entry : Widget
{
    public UString Text { get; set; } = new("", Color.Black);
    public Coord TextOffset = Coord.Zero;

    public Color TextBackground { get; set; } = Color.Gray;
    public int MaxTextLenght { get; set; }

    public TextEntryMode InputMode { get; set; } = TextEntryMode.All;

    private bool _inEntryMode;
    
    public Entry(Frame parent) : base(parent)
    {
        Input.MouseLeftClick += LeftClick;
        Input.KeyEvent += KeyEvent;
    }
    
    public override Entry Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        return base.Grid<Entry>(row, column, rowSpan, columnSpan, alignment);
    }

    public override Entry Place(int posX, int posY)
    {
        return base.Grid<Entry>(posX, posY);
    }

    public override void Render()
    {
        if (_inEntryMode) Text.Cycle();
        
        var textStart = Center + TextOffset + Coord.Left * (MaxTextLenght / 2);
        var start = Anchor + Offset;
        
        for (var x = start.X; x < start.X + Size.X; x++)
        for (var y = start.Y; y < start.Y + Size.Y; y++)
        {
            if (y == Center.Y && x >= textStart.X && x < textStart.X + MaxTextLenght + 1) continue;
            
            Display.Display.Draw(x, y, ' ', Color.Black, Color);
        }
        
        Display.Display.DrawRect(textStart, (MaxTextLenght + 1, 1), TextBackground);
        
        Display.Display.Print(
            Center.X + TextOffset.X - MaxTextLenght / 2,
            Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            Text.Background ?? TextBackground, Alignment.Left);
    }

    private void KeyEvent(KeyboardState obj)
    {
        if (!_inEntryMode || !obj.Pressed) return;

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

        if (InputMode != TextEntryMode.Digits) return;
        
        if (string.IsNullOrWhiteSpace(Text.Text)) Text.Text = "0";
    }

    private void LeftClick(MouseState obj)
    {
        if (IsInside(obj.Position))
        {
            _inEntryMode = !_inEntryMode;
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
    
    public override void Remove()
    {
        Input.MouseLeftClick -= LeftClick;
        Input.KeyEvent -= KeyEvent;
        
        base.Remove();
    }
}

public enum TextEntryMode
{
    All,
    Alphanumeric,
    Letters,
    Digits,
}