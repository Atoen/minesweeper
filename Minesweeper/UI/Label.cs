using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Label : Widget
{
    public required IText Text { get; init; }
    public Coord TextOffset = Coord.Zero;

    public Label(Frame parent) : base(parent) { }

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

        Display.DrawRect(Anchor + Offset, Size, Color);

        Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            background: Text.Background ?? Color, mode: Text.Mode);
    }

    public override void Clear()
    {
        var textStart = Center + TextOffset + Coord.Left * (Text.Length / 2);
        
        Display.ClearRect(textStart, (Text.Length, 1));

        base.Clear();
    }
    
    protected override void Resize()
    {
        if (Text == UString.Empty) return;
        
        var minSize = new Coord(Text.Length + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }
}