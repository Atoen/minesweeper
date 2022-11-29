namespace Minesweeper.UI;

public class Label : Widget
{
    public UString Text { get; set; }
    
    public Label(Frame parent, UString text) : base(parent)
    {
        Text = text;
    }

    public override void Render()
    {
        if (Text.Animating) Text.Cycle();

        base.Render();
        
        Display.Display.Print(Center.X, Center.Y, Text.Text, Text.Foreground, Text.Background ?? Color);
    }
}