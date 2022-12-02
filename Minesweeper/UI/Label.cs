namespace Minesweeper.UI;

public class Label : Widget
{
    public UString Text { get; set; }
    public Coord TextOffset = Coord.Zero;

    public Label(Frame parent, UString text) : base(parent)
    {
        Text = text;
    }

    protected override void Resize()
    {
        if (Text == UString.Empty) return;
        
        var minSize = new Coord(Text.Lenght + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }

    public override void Render()
    {
        if (Text.Animating) Text.Cycle();

        base.Render();

        Display.Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            Text.Background ?? Color);
    }

    public override void Clear()
    {
        var textStart = Center + TextOffset + Coord.Left * (Text.Lenght / 2);
        
        Display.Display.ClearRect(textStart, Coord.Right * Text.Lenght);

        base.Clear();
    }
}
