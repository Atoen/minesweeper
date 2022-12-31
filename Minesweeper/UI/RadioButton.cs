using Minesweeper.ConsoleDisplay;

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

    private void OnVariableChange()
    {
        if (!IsSelected && IsEnabled) State = State.Default;
    }

    // public override RadioButton Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    // {
    //     return base.Grid<RadioButton>(row, column, rowSpan, columnSpan, alignment);
    // }
    //
    // public override RadioButton Place(int posX, int posY)
    // {
    //     return base.Grid<RadioButton>(posX, posY);
    // }
    
    protected override void OnMouseLeftDown()
    {
        _variable.Val = _value;
        
        base.OnMouseLeftDown();
    }

    public override void Render()
    {
        AnimatedText.Mode = IsSelected ? SelectedTextMode : TextMode.Default;

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
            if (_val != value) ValueChanged?.Invoke();
            _val = value;
        }
    }

    public Variable(int val = 0) => Val = val;

    public event Action? ValueChanged;
}
