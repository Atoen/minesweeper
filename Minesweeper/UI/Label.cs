namespace Minesweeper.UI;

public class Label : Widget
{
    public UString Text { get; set; }
    public Coord TextOffset = Coord.Zero;

    public Label(Frame parent, UString text) : base(parent)
    {
        Text = text;
    }

    public override Label Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        return base.Grid<Label>(row, column, rowSpan, columnSpan, alignment);
    }

    public override Label Place(int posX, int posY)
    {
        return base.Grid<Label>(posX, posY);
    }

    public override void Render()
    {
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

    public override void Clear()
    {
        var textStart = Center + TextOffset + Coord.Left * (Text.Length / 2);
        
        Display.Display.ClearRect(textStart, (Text.Length, 1));

        base.Clear();
    }
    
    protected override void Resize()
    {
        if (Text == UString.Empty) return;
        
        var minSize = new Coord(Text.Length + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }
}
