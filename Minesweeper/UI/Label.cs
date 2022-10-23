namespace Minesweeper.UI;

public class Label : IRenderable
{
    public Coord Pos;
    public Coord Size;
    
    public ConsoleColor DefaultColor = ConsoleColor.Blue;
    public Alignment Alignment;

    public string Text;

    public Label(string text, Alignment alignment = Alignment.Center)
    {
        Text = text;
        Alignment = alignment;
        
        Display.AddToRenderList(this);
    }

    public void Render()
    {
        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            Display.Draw(x, y, ' ', ConsoleColor.White, DefaultColor);
        }
        
        RenderText();
    }

    public void Destroy()
    {
        Display.RemoveFromRenderList(this);

        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            Display.Draw(x, y, ' ', ConsoleColor.White, ConsoleColor.Black);
        }
    }

    private void RenderText()
    {
        var centerX = Pos.X + Size.X / 2;
        var centerY = Pos.Y + Size.Y / 2;
        
        Display.Print(centerX, centerY, Text, ConsoleColor.Black, DefaultColor, Alignment);
    }
}