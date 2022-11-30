namespace Minesweeper.UI;

public class Button : Widget
{
    public Action? OnClick;
    public UString Text { get; set; }

    public Button(Frame parent, UString text) : base(parent)
    {
        Input.MouseLeftClick += LeftClick;
        Input.MouseEvent += MouseMove;

        Text = text;
    }

    public override void Render()
    {
        Color = State switch
        {
            WidgetState.Pressed => PressedColor,
            WidgetState.Highlighted => HighlightedColor,
            _ => DefaultColor
        };
        
        if (Text.Animating) Text.Cycle();
        
        base.Render();
        
        Display.Display.Print(Center.X, Center.Y, Text.Text, Text.Foreground, Text.Background ?? Color);
    }
    
    protected override void Resize()
    {
        var minSize = new Coord(Text.Lenght + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }

    private void LeftClick(MouseState obj)
    {
        if (!IsCursorOver(obj.Position)) return;

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
    
    public override void Remove()
    {
        Input.MouseLeftClick -= LeftClick;
        Input.MouseEvent -= MouseMove;
        
        base.Remove();
    }
}