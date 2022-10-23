namespace Minesweeper.UI;

public sealed class Button : IRenderable
{
    public Coord Pos;
    public Coord Size;

    public ConsoleColor DefaultColor = ConsoleColor.Blue;
    public ConsoleColor HighlightedColor = ConsoleColor.Cyan;
    public ConsoleColor PressedColor = ConsoleColor.White;

    public Action? ClickAction;

    public string Text;
    public Alignment Alignment;

    private ConsoleColor _currentColor;
    private ButtonState _state;
    private Coord _center;

    public Button(string text = "", Alignment alignment = Alignment.Center)
    {
        // Pos = pos;
        // Size = size;
        // _center.X =  (short) (Pos.X + Size.X / 2);
        // _center.Y =  (short) (Pos.Y + Size.Y / 2);

        Text = text;
        Alignment = alignment;

        Display.AddToRenderList(this);

        Input.MouseLeftClick += Click;
        Input.MouseEvent += CursorMove;
    }

    public void Render()
    {
        _currentColor = _state switch
        {
            ButtonState.Pressed => PressedColor,
            ButtonState.Highlighted => HighlightedColor,
            _ => DefaultColor
        };

        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            Display.Draw(x, y, ' ', ConsoleColor.White, _currentColor);
        }
        
        RenderText();
    }

    public void Destroy()
    {
        Display.RemoveFromRenderList(this);
        Input.MouseLeftClick -= Click;
        Input.MouseEvent -= CursorMove;

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
        
        Display.Print(centerX, centerY, Text, ConsoleColor.Black, _currentColor, Alignment);
        
        // Display.Print(_center.X, _center.Y, Text, ConsoleColor.Black, _currentColor, Alignment);
    }
    
    private bool IsOver(Coord pos)
    {
        return pos.X >= Pos.X && pos.X < Pos.X + Size.X &&
               pos.Y >= Pos.Y && pos.Y < Pos.Y + Size.Y;
    }

    public void Click(MouseState state)
    {
        if (!IsOver(state.Position)) return;
        
        _state = ButtonState.Pressed;
        ClickAction?.Invoke();
    }

    public void CursorMove(MouseState state)
    {
        if (IsOver(state.Position))
        { 
            if (state.Buttons == 0) _state = ButtonState.Highlighted;
            return;
        }
        
        _state = ButtonState.Default;
    }
}

public enum ButtonState
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