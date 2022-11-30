namespace Minesweeper.UI;

public class Label : Widget
{
    public UString Text { get; set; }

    public Label(Frame parent, UString text) : base(parent)
    {
        Text = text;
    }

    protected override void Resize()
    {
        var minSize = new Coord(Text.Lenght + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }

    public override void Render()
    {
        if (Text.Animating) Text.Cycle();

        base.Render();
        
        Display.Display.Print(Center.X, Center.Y, Text.Text, Text.Foreground, Text.Background ?? Color);
    }
}
