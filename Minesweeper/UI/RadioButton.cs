using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public class RadioButton : Button
{
    public bool IsSelected => _variable.Val == _value;

    public TextMode SelectedTextMode { get; set; } = TextMode.Underline | TextMode.Overline;
    
    private readonly Variable _variable;
    private readonly int _value;

    public RadioButton(Variable variable, int value)
    {
        _variable = variable;
        _value = value;
        
        _variable.ValueChanged += OnVariableChange;

        if (IsSelected) State = State.Highlighted;
    }

    private void OnVariableChange(object? sender, EventArgs eventArgs)
    {
        if (!IsSelected && Enabled) State = State.Default;
    }

    public override void OnMouseLeftDown(MouseEventArgs e)
    {
        _variable.Val = _value;
        
        base.OnMouseLeftDown(e);
    }

    public override void Render()
    {
        // AnimatedText.Mode = IsSelected ? SelectedTextMode : TextMode.Default;

        if (IsSelected && State == State.Default) State = State.Highlighted;
        else if (!IsSelected && !IsMouseOver) State = State.Default;

        base.Render();
    }
}

public sealed class Variable
{
    private int _val;

    public int Val
    {
        get => _val;
        set
        {
            if (_val != value) ValueChanged?.Invoke(this, EventArgs.Empty);
            _val = value;
        }
    }

    public Variable(int val = 0) => Val = val;

    public event EventHandler? ValueChanged;
}
