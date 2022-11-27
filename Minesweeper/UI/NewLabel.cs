namespace Minesweeper.UI;

public class NewLabel : NewWidget
{
    public UString Text { get; set; }
    
    public NewLabel(NewFrame parent, UString text) : base(parent)
    {
        Text = text;
    }

    public override void Render()
    {
        Text.Cycle();
        
        Debug.WriteLine("rendering");
        
        Display.Display.Print(DrawOffset.X, DrawOffset.Y, Text.Text, Text.Foreground, Text.Background ?? DefaultColor);
        
        base.Render();
    }
}