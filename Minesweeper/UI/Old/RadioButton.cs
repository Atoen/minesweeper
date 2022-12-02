namespace Minesweeper.UI;

public sealed class RadioButton : WidgetOld
{
    public Color HighlightedColor = Color.Cyan;
    public Color PressedColor = Color.White;
    public Action? OnClick;
    
    private WidgetState _uiState = WidgetState.Default;
    private TextAlignment _buttonTextAlignment;
    private Variable _variable;
    private int _value;
    private Coord _buttonPos;

    public RadioButton(Color color, string text, Variable variable, int value,
        TextAlignment buttonTextAlignment = TextAlignment.Left, TextAlignment textTextAlignment = TextAlignment.Center)
        : base(color, text, textTextAlignment)
    {
        Input.MouseLeftClick += LeftClick;
        Input.MouseEvent += MouseMove;

        _variable = variable;
        _value = value;
        _buttonTextAlignment = buttonTextAlignment;

        if (variable.Val == value) _uiState = WidgetState.Highlighted;
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

        Display.Display.Draw(_buttonPos.X, _buttonPos.Y, ' ', Color.White,
            _variable.Val == _value ? Color.Black : Color.Gray);
    }

    protected override void SetUp()
    {
        IsTextSetUp = true;
        
        if (Text.Length == 0) return;
        
        TextCenter.Y = (short) (Pos.Y + Size.Y / 2);
        TextCenter.X = _buttonTextAlignment switch
        {
            TextAlignment.Left => (short) (Pos.X + (Size.X + Text.Length) / 2),
            TextAlignment.Right => (short) (Pos.X + (Size.X - Text.Length) / 2),
            _ => (short) (Pos.X + Size.X / 2),
        };
        
        TextStart.Y = TextCenter.Y;
        TextStart.X = TextAlignment switch
        {
            TextAlignment.Left => (short) (TextCenter.X - Text.Length),
            TextAlignment.Right => TextCenter.X,
            _ => (short) (TextCenter.X - Text.Length / 2)
        };
        
        TextStop.Y = TextCenter.Y;
        TextStop.X = (short) (TextStart.X + Text.Length);

        _buttonPos.Y = TextCenter.Y;
        _buttonPos.X = _buttonTextAlignment switch
        {
            TextAlignment.Left => (short) (Pos.X + 1),
            TextAlignment.Right => (short) (Pos.X + Size.X - 1),
            _ => TextCenter.X
        };
    }

    public override void Remove()
    {
        Input.MouseLeftClick -= LeftClick;
        Input.MouseEvent -= MouseMove;
        
        base.Remove();
    }
    
    private void MouseMove(MouseState state)
    {
        // if (_variable.Val == _value) return;
        
        if (IsCursorOver(state.Position))
        {
            if (state.Buttons == 0) _uiState = WidgetState.Highlighted;
            return;
        }

        _uiState = _variable.Val == _value ? WidgetState.Highlighted : WidgetState.Default;
    }
    
    private void LeftClick(MouseState state)
    {
        if (!IsCursorOver(state.Position)) return;

        _variable.Val = _value;
        
        _uiState = WidgetState.Pressed;
        OnClick?.Invoke();
    }
}

public sealed class Variable
{
    public int Val { get; set; }
    
    public Variable(int val = 0) => Val = val;
}