namespace Minesweeper.UI;

public class Spinbox : Widget
{
    public short CurrentVal;
    public short MaxVal;
    public short MinVal;
    
    private string _keyboardText = string.Empty;
    private bool _inKeyboardMode;

    public Spinbox(ConsoleColor color, short minVal, short maxVal, short defaultVal,
        Alignment alignment = Alignment.Center) : base(color, "", alignment)
    {
        if (minVal > maxVal)
        {
            (maxVal, minVal) = (minVal, maxVal);
        }
        
        MinVal = minVal;
        MaxVal = maxVal;

        CurrentVal = defaultVal;
        
        Input.MouseEvent += MouseWheel;
        Input.MouseLeftClick += LeftClick;
        Input.DoubleClick += DoubleClick;
        Input.KeyEvent += KeyEvent;
    }

    public override void Remove()
    {
        Input.MouseEvent -= MouseWheel;
        Input.MouseLeftClick -= LeftClick;
        Input.DoubleClick -= DoubleClick;
        Input.KeyEvent -= KeyEvent;
        
        base.Remove();
    }

    protected override void RenderText()
    {
        var centerX = Pos.X + Size.X / 2;
        var centerY = Pos.Y + Size.Y / 2;
        
        Display.Draw(Pos.X, centerY, '<', ConsoleColor.White, DefaultColor);
        Display.Draw(Pos.X + Size.X - 1, centerY, '>', ConsoleColor.White, DefaultColor);

        var text = _inKeyboardMode ? _keyboardText : CurrentVal.ToString();
        
        Display.Print(centerX, centerY, text, ConsoleColor.Black, DefaultColor, Alignment);
    }
    
    private void KeyEvent(KeyboardState state)
    {
        if (!_inKeyboardMode || !state.Pressed) return;

        if (state.KeyCode == (int) ConsoleKey.Enter)
        {
            ExitKeyboardMode();
            return;
        }

        if (char.IsDigit(state.Char))
        {
            _keyboardText += state.Char;
        }
    }
    
    private void ExitKeyboardMode()
    {
        short val = 0;
        if (!string.IsNullOrWhiteSpace(_keyboardText))
        {
            val = short.Parse(_keyboardText);
        }

        if (val > MaxVal) val = MaxVal;
        else if (val < MinVal) val = MinVal;

        CurrentVal = val;
        _inKeyboardMode = false;
        _keyboardText = string.Empty;
    }
    
    private void DoubleClick(MouseState state)
    {
        if (IsCursorOver(state.Position))
        {
            _inKeyboardMode = !_inKeyboardMode;
        }
    }

    private void LeftClick(MouseState state)
    {
        if (!IsCursorOver(state.Position)) _inKeyboardMode = false;
        
        if (state.Position.Y != Pos.Y + Size.Y / 2) return;
        
        if (state.Position.X == Pos.X) ChangeValue(-1);
        if (state.Position.X == Pos.X + Size.X - 1) ChangeValue(1);
    }

    private void MouseWheel(MouseState state)
    {
        if (!IsCursorOver(state.Position)) return;

        if (state.Wheel == MouseWheelState.Up) ChangeValue(1);
        else if (state.Wheel == MouseWheelState.Down) ChangeValue(-1);
    }
    
    private void ChangeValue(short change)
    {
        CurrentVal += change;

        var sign = Math.Sign(change);
        
        if (sign == -1)
        {
            if (CurrentVal + change < MinVal) CurrentVal = MinVal;
        }
        else if (sign == 1)
        {
            if (CurrentVal + change > MaxVal) CurrentVal = MaxVal;
        }
    }
}