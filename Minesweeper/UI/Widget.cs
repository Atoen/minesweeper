namespace Minesweeper.UI;

public abstract class Widget : IRenderable
{
    public Coord Pos;
    public Coord Size;

    public ConsoleColor DefaultColor;

    public Alignment Alignment;
    public string Text;
    
    public bool ShouldRemove { get; set; }

    protected ConsoleColor CurrentColor;

    protected Widget(ConsoleColor color, string text, Alignment alignment)
    {
        Text = text;
        Alignment = alignment;

        CurrentColor = color;
        DefaultColor = color;
        
        Display.AddToRenderList(this);
    }

    public virtual void Render()
    {
        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            Display.Draw(x, y, ' ', ConsoleColor.White, CurrentColor);
        }
        
        RenderText();
    }

    public virtual void Remove()
    {
        ShouldRemove = true;
        
        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            Display.ClearAt(x, y);
        }
    }

    protected virtual void RenderText()
    {
        var centerX = Pos.X + Size.X / 2;
        var centerY = Pos.Y + Size.Y / 2;
        
        Display.Print(centerX, centerY, Text, ConsoleColor.Black, CurrentColor, Alignment);
    }

    protected bool IsCursorOver(Coord pos)
    {
        return pos.X >= Pos.X && pos.X < Pos.X + Size.X &&
               pos.Y >= Pos.Y && pos.Y < Pos.Y + Size.Y;
    }
}

public enum WidgetState
{
    Default,
    Highlighted,
    Pressed
}

public enum Alignment
{
    Left,
    Right,
    Center
}
