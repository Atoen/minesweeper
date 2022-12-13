namespace Minesweeper.UI;

public sealed class RadioButtonOld : WidgetOld
{
    public Color HighlightedColor = Color.Cyan;
    public Color PressedColor = Color.White;
    public Action? OnClick;
    
    private WidgetState _uiState = WidgetState.Default;
    private Alignment _buttonAlignment;
    private VariableOld _variableOld;
    private int _value;
    private Coord _buttonPos;

    public RadioButtonOld(Color color, string text, VariableOld variableOld, int value,
        Alignment buttonAlignment = Alignment.Left, Alignment alignment = Alignment.Center)
        : base(color, text, alignment)
    {
        Input.MouseLeftClick += LeftClick;
        Input.MouseEvent += MouseMove;

        _variableOld = variableOld;
        _value = value;
        _buttonAlignment = buttonAlignment;

        if (variableOld.Val == value) _uiState = WidgetState.Highlighted;
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

        ConsoleDisplay.Display.Draw(_buttonPos.X, _buttonPos.Y, ' ', Color.White,
            _variableOld.Val == _value ? Color.Black : Color.Gray);
    }

    protected override void SetUp()
    {
        IsTextSetUp = true;
        
        if (Text.Length == 0) return;
        
        TextCenter.Y = (short) (Pos.Y + Size.Y / 2);
        TextCenter.X = _buttonAlignment switch
        {
            Alignment.Left => (short) (Pos.X + (Size.X + Text.Length) / 2),
            Alignment.Right => (short) (Pos.X + (Size.X - Text.Length) / 2),
            _ => (short) (Pos.X + Size.X / 2),
        };
        
        TextStart.Y = TextCenter.Y;
        TextStart.X = Alignment switch
        {
            Alignment.Left => (short) (TextCenter.X - Text.Length),
            Alignment.Right => TextCenter.X,
            _ => (short) (TextCenter.X - Text.Length / 2)
        };
        
        TextStop.Y = TextCenter.Y;
        TextStop.X = (short) (TextStart.X + Text.Length);

        _buttonPos.Y = TextCenter.Y;
        _buttonPos.X = _buttonAlignment switch
        {
            Alignment.Left => (short) (Pos.X + 1),
            Alignment.Right => (short) (Pos.X + Size.X - 1),
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

        _uiState = _variableOld.Val == _value ? WidgetState.Highlighted : WidgetState.Default;
    }
    
    private void LeftClick(MouseState state)
    {
        if (!IsCursorOver(state.Position)) return;

        _variableOld.Val = _value;
        
        _uiState = WidgetState.Pressed;
        OnClick?.Invoke();
    }
}

public sealed class VariableOld
{
    public int Val { get; set; }
    
    public VariableOld(int val = 0) => Val = val;
}