namespace Minesweeper.UI;

public class NewLabel : NewWidget
{
    public string Text { get; set; }
    
    public NewLabel(NewFrame parent, string text = "") : base(parent)
    {
        Text = text;
    }
}