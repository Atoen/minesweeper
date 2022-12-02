namespace Minesweeper.UI;

public class WidgetOld : IRenderable
{
    public Coord Pos;
    public Coord Size;

    public Color DefaultColor;

    public TextAlignment TextAlignment;

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

    public bool ShouldRemove { get; set; }

    protected Color CurrentColor;

    protected WidgetOld(Color color, string text, TextAlignment textAlignment)
    {
        Text = text;
        TextAlignment = textAlignment;

        CurrentColor = color;
        DefaultColor = color;

        Display.Display.AddToRenderList(this);
    }

    public virtual void Render()
    {
        if (!IsTextSetUp) SetUp();

        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            if (x >= TextStart.X && x < TextStop.X && y == TextStart.Y) continue;
            
            Display.Display.Draw(x, y, ' ', Color.Black, CurrentColor);
        }
        
        RenderText();
    }

    public virtual void Remove() => ShouldRemove = true;

    public virtual void Clear() => Display.Display.ClearRect(Pos, Size);

    protected virtual void RenderText()
    {
        Display.Display.Print(TextCenter.X, TextCenter.Y, Text, Color.Black, CurrentColor, TextAlignment);
    }

    protected bool IsCursorOver(Coord pos)
    {
        return pos.X >= Pos.X && pos.X < Pos.X + Size.X &&
               pos.Y >= Pos.Y && pos.Y < Pos.Y + Size.Y;
    }

    protected virtual void SetUp()
    {
        IsTextSetUp = true;
        
        if (Text.Length == 0) return;
        
        TextCenter.X = (short) (Pos.X + Size.X / 2);
        TextCenter.Y = (short) (Pos.Y + Size.Y / 2);

        TextStart.Y = TextCenter.Y;
        TextStart.X = TextAlignment switch
        {
            TextAlignment.Left => (short) (TextCenter.X - Text.Length),
            TextAlignment.Right => TextCenter.X,
            _ => (short) (TextCenter.X - Text.Length / 2)
        };

        TextStop.Y = TextCenter.Y;
        TextStop.X = (short) (TextStart.X + Text.Length);
    }
}
