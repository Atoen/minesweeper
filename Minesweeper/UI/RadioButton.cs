using Minesweeper.Display;

namespace Minesweeper.UI;

public class RadioButton : Widget
{
    public Action? OnClick { get; init; }
    public UString Text { get; set; }
    public Coord TextOffset = Coord.Zero;
    public Alignment ButtonAlignment { get; set; } = Alignment.Left;
    public bool IsSelected => _variable.Val == _value;

    private Variable _variable;
    private int _value;

    public RadioButton(Frame parent, UString text, Variable variable, int value) : base(parent)
    {
        Text = text;

        _variable = variable;
        _value = value;

        Input.MouseLeftClick += LeftClick;
        Input.MouseEvent += MouseMove;
    }

    private void LeftClick(MouseState obj)
    {
        if (!IsCursorOver(obj.Position)) return;

        _variable.Val = _value;

        State = WidgetState.Pressed;
        OnClick?.Invoke();
    }
    
    private void MouseMove(MouseState obj)
    {
        if (IsCursorOver(obj.Position))
        {
            if (obj.Buttons == 0) State = WidgetState.Highlighted;
            return;
        }

        State = WidgetState.Default;
    }

    public override void Render()
    {
        Color = State switch
        {
            WidgetState.Highlighted => HighlightedColor,
            WidgetState.Pressed => PressedColor,
            _ => DefaultColor,
        };
        
        base.Render();

        var textBackground = IsSelected ? Color.Brighter(50) : Text.Background ?? Color;
        
        Display.Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
            textBackground);
    }

    protected override void Resize()
    {
        var minSize = new Coord(Text.Lenght + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);

        Size = Size.ExpandTo(minSize);
    }
    
    public override void Remove()
    {
        Input.MouseLeftClick -= LeftClick;
        Input.MouseEvent -= MouseMove;
        
        base.Remove();
    }
}

public sealed class Variable
{
    public int Val { get; set; }

    public Variable(int val = 0) => Val = val;
}