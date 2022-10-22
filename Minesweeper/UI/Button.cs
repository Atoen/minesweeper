namespace Minesweeper.UI;

public sealed class Button : IRenderable, IInteractable
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
    public ButtonState State;

    public Button(string text = "", Alignment alignment = Alignment.Center)
    {
        Text = text;
        Alignment = alignment;
        
        Display.AddToRenderList(this);
        Input.RegisterMouseUiElement(this);
        
        Input.MouseClickEvent += Click;
        Input.MouseEvent += CursorMove;
    }

    public void Render()
    {
        _currentColor = State switch
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

    private void RenderText()
    {
        var centerX = Pos.X + Size.X / 2;
        var centerY = Pos.Y + Size.Y / 2;
        
        Display.Print(centerX, centerY, Text, ConsoleColor.Black, _currentColor, Alignment);
    }

    private bool IsOver(Coord pos)
    {
        return pos.X >= Pos.X && pos.X < Pos.X + Size.X &&
               pos.Y >= Pos.Y && pos.Y < Pos.Y + Size.Y;
    }

    public void Click(MouseState state)
    {
        if (IsOver(state.Position)) State = ButtonState.Pressed;
        ClickAction?.Invoke();
    }

    public void CursorMove(MouseState state)
    {
        if (IsOver(state.Position))
        {
            if (state.Buttons == 0) State = ButtonState.Highlighted;
            return;
        }
        
        State = ButtonState.Default;
    }

    public void Update()
    {
        
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