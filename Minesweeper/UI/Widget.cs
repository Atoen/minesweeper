using System.Runtime.CompilerServices;

namespace Minesweeper.UI;

public class Widget : IRenderable
{
    public Coord Pos;
    public Coord Size;

    public Color DefaultColor;

    public Alignment Alignment;

    private string _text = "";
    public string Text
    {
        get => _text;
        set
        {
            IsTextSetUp = false;
            _text = value;
        }
    }

    protected bool IsTextSetUp;
    
    protected Coord TextStart;
    protected Coord TextStop;
    protected Coord TextCenter;
    protected bool AddedToRenderList;

    public bool ShouldRemove { get; set; }

    protected Color CurrentColor;

    protected Widget(Color color, string text, Alignment alignment)
    {
        Text = text;
        Alignment = alignment;

        CurrentColor = color;
        DefaultColor = color;

        Display.AddToRenderList(this);
    }

    public virtual void Render()
    {
        if (!IsTextSetUp) SetUpText();

        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            if (x >= TextStart.X && x < TextStop.X && y == TextStart.Y) continue;
            
            Display.Draw(x, y, ' ', Color.White, CurrentColor);
        }
        
        RenderText();
    }

    public virtual void Remove() => ShouldRemove = true;

    public virtual void Clear() => Display.ClearRect(Pos, Size);

    protected virtual void RenderText()
    {
        Display.Print(TextCenter.X, TextCenter.Y, Text, Color.Black, CurrentColor, Alignment);
    }

    protected bool IsCursorOver(Coord pos)
    {
        return pos.X >= Pos.X && pos.X < Pos.X + Size.X &&
               pos.Y >= Pos.Y && pos.Y < Pos.Y + Size.Y;
    }

    protected virtual void SetUpText()
    {
        IsTextSetUp = true;
        
        if (Text.Length == 0) return;
        
        TextCenter.X = (short) (Pos.X + Size.X / 2);
        TextCenter.Y = (short) (Pos.Y + Size.Y / 2);

        TextStart.Y = TextCenter.Y;
        TextStart.X = Alignment switch
        {
            Alignment.Left => (short) (TextCenter.X - Text.Length),
            Alignment.Right => TextCenter.X,
            _ => (short) (TextCenter.X - Text.Length / 2)
        };

        TextStop.Y = TextCenter.Y;
        TextStop.X = (short) (TextStart.X + Text.Length);
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
