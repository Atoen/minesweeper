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
        if (Text.Animating) Text.Cycle();

        ConsoleDisplay.Display.DrawRect(Anchor + Offset, Size, Color);

        ConsoleDisplay.Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
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
