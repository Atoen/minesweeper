namespace Minesweeper.UI;

public sealed class Button : Widget
{
    public Color HighlightedColor = Color.Cyan;
    public Color PressedColor = Color.White;
    public Action? OnClick;

    private WidgetState _uiState = WidgetState.Default;
    
    public Button(Color color, string text, Alignment alignment = Alignment.Center) : base(color, text, alignment)
    {
        Input.MouseLeftClick += LeftClick;
        Input.MouseEvent += MouseMove;
    }

    public override void Render()
    {
        CurrentColor = _uiState switch
        {
            WidgetState.Pressed => PressedColor,
            WidgetState.Highlighted => HighlightedColor,
            _ => DefaultColor
        };
        
        base.Render();
    }

    public override void Remove()
    {
        Input.MouseLeftClick -= LeftClick;
        Input.MouseEvent -= MouseMove;
        
        base.Remove();
    }

    private void MouseMove(MouseState state)
    {
        if (IsCursorOver(state.Position))
        {
            if (state.Buttons == 0) _uiState = WidgetState.Highlighted;
            return;
        }

        _uiState = WidgetState.Default;
    }

    private void LeftClick(MouseState state)
    {
        if (!IsCursorOver(state.Position)) return;

        _uiState = WidgetState.Pressed;
        OnClick?.Invoke();
    }
}
