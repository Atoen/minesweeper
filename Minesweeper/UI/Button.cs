namespace Minesweeper.UI;

public class Button : Widget
{
    public Action? OnClick { get; init; }
    public UString Text { get; set; }
    public Coord TextOffset = Coord.Zero;

    public Button(Frame parent, UString text) : base(parent)
    {
        Text = text;
        
        Input.MouseLeftClick += LeftClick;
        Input.MouseEvent += MouseMove;
    }
    
    public override Button Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        return base.Grid<Button>(row, column, rowSpan, columnSpan, alignment);
    }

    public override Button Place(int posX, int posY)
    {
        return base.Grid<Button>(posX, posY);
    }

    public override void Render()
    {
        Color = State switch
        {
            WidgetState.Pressed => PressedColor,
            WidgetState.Highlighted => HighlightedColor,
            _ => DefaultColor
        };
        
        if (Text.Animating) Text.Cycle();
        
        var textStart = Center + TextOffset + Coord.Left * (Text.Length / 2);
        var start = Anchor + Offset;
        
        for (var x = start.X; x < start.X + Size.X; x++)
        for (var y = start.Y; y < start.Y + Size.Y; y++)
        {
            if (y == Center.Y && x >= textStart.X && x < textStart.X + Text.Length) continue;
            
            Display.Display.Draw(x, y, ' ', Color.Black, Color);
        }

        Display.Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            Text.Background ?? Color);
    }
    
    protected override void Resize()
    {
        var minSize = new Coord(Text.Length + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }

    protected virtual void LeftClick(MouseState obj)
    {
        if (!IsInside(obj.Position)) return;

        State = WidgetState.Pressed;
        OnClick?.Invoke();
    }

    private void MouseMove(MouseState obj)
    {
        if (IsInside(obj.Position))
        {
            if (obj.Buttons == 0) State = WidgetState.Highlighted;
            return;
        }

        State = WidgetState.Default;
    }
    
    public override void Remove()
    {
        Input.MouseLeftClick -= LeftClick;
        Input.MouseEvent -= MouseMove;
        
        base.Remove();
    }
}
