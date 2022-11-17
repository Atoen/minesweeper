using System.Collections;

namespace Minesweeper.UI;

public sealed class Spinbox : Widget
{
    public short CurrentVal;
    public short MaxVal;
    public short MinVal;

    public char TextCycleSymbol = '_';
    
    private string _keyboardText = string.Empty;
    private bool _inKeyboardMode;
    private readonly IEnumerator _textCycle;
    private bool _displayingPlaceholder;

    public Spinbox(Color color, short minVal, short maxVal, short defaultVal,
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

        _textCycle = CycleText();
    }

    public override void Remove()
    {
        Input.MouseEvent -= MouseWheel;
        Input.MouseLeftClick -= LeftClick;
        Input.DoubleClick -= DoubleClick;
        Input.KeyEvent -= KeyEvent;
        
        ExitKeyboardMode();
        
        base.Remove();
    }

    private IEnumerator CycleText()
    {
        var i = 0;

        while (!ShouldRemove)
        {
            if (!_inKeyboardMode) yield return null;
            
            i++;
            if (i > 8)
            {
                _displayingPlaceholder = !_displayingPlaceholder;
                i = 0;
            }

            yield return null;
        }
    }

    public override void Render()
    {
        if (ShouldRemove) return;
        if (!IsTextSetUp) SetUp();

        for (var x = Pos.X; x < Pos.X + Size.X; x++)
        for (var y = Pos.Y; y < Pos.Y + Size.Y; y++)
        {
            if (x >= TextStart.X && x < TextStop.X && y == TextStart.Y) continue;

            if (y == Pos.Y + Size.Y / 2 && x == Pos.X)
            {
                Display.Display.Draw(x, y, '<', Color.Black, CurrentColor);
            }
            else if (y == Pos.Y + Size.Y / 2 && x == Pos.X + Size.X - 1)
            {
                Display.Display.Draw(x, y, '>', Color.Black, CurrentColor);
            }
            else
            {
                Display.Display.Draw(x, y, ' ', Color.White, CurrentColor);
            }
        }
        
        RenderText();
    }

    protected override void RenderText()
    {
        var centerX = Pos.X + Size.X / 2;
        var centerY = Pos.Y + Size.Y / 2;

        _textCycle.MoveNext();
        
        var keyboardText = _displayingPlaceholder ? new string(TextCycleSymbol, _keyboardText.Length) : _keyboardText;
        var text = _inKeyboardMode ? keyboardText : CurrentVal.ToString();
        
        Display.Display.Print(centerX, centerY, text, Color.Black, DefaultColor, Alignment);
    }

    protected override void SetUp()
    {
        base.SetUp();

        TextStart.X = Pos.X;
        TextStop.X = (short) (Pos.X + Size.X);
    }

    private void KeyEvent(KeyboardState state)
    {
        if (!_inKeyboardMode || !state.Pressed) return;

        if (state.Key == ConsoleKey.Enter)
        {
            ExitKeyboardMode();
            return;
        }

        if (char.IsDigit(state.Char) && _keyboardText.Length < 4)
        {
            _keyboardText += state.Char;
            return;
        }

        if (state.Key == ConsoleKey.Backspace && _keyboardText.Length > 0)
        {
            _keyboardText = _keyboardText.Remove(_keyboardText.Length - 1);
        }
    }
    
    private void ExitKeyboardMode()
    {
        var val = CurrentVal;
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
        if (!IsCursorOver(state.Position)) return;
        
        _inKeyboardMode = !_inKeyboardMode;
        if (!_inKeyboardMode) ExitKeyboardMode();
    }

    private void LeftClick(MouseState state)
    {
        if (!IsCursorOver(state.Position)) ExitKeyboardMode();
        
        if (state.Position.Y != Pos.Y + Size.Y / 2) return;
        
        if (state.Position.X == Pos.X) ChangeValue(-1);
        if (state.Position.X == Pos.X + Size.X - 1) ChangeValue(1);
    }

    private void MouseWheel(MouseState state)
    {
        if (!IsCursorOver(state.Position)) return;

        if (state.Wheel is MouseWheelState.Up or MouseWheelState.AsciiUp) ChangeValue(1);
        else if (state.Wheel is MouseWheelState.Down or MouseWheelState.AsciiDown) ChangeValue(-1);

        SetUp();
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
