using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class RadioButton : Button
{
    public bool IsSelected => _variable.Val == _value;
    
    private readonly Variable _variable;
    private readonly int _value;

    public RadioButton(Frame parent, Variable variable, int value) : base(parent)
    {
        _variable = variable;
        _value = value;
    }
    
    public override RadioButton Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        return base.Grid<RadioButton>(row, column, rowSpan, columnSpan, alignment);
    }

    public override RadioButton Place(int posX, int posY)
    {
        return base.Grid<RadioButton>(posX, posY);
    }
    
    protected override void LeftClick(MouseState obj)
    {
        if (!IsInside(obj.Position)) return;

        State = WidgetState.Pressed;
        OnClick?.Invoke();
        
        _variable.Val = _value;
    }
    
    public override void Render()
    {
        Text.Mode = IsSelected ? TextMode.Underline : TextMode.Default;

        base.Render();
    }
}

public sealed class Variable
{
    public int Val { get; set; }

    public Variable(int val = 0) => Val = val;
}
