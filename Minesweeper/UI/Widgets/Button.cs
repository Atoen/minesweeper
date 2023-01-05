using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI.Widgets;

public class Button : ContentControl
{
    public Button()
    {
        _text = new Text(nameof(Button)) {Parent = this};
    }
    
    private Text _text;
    public Text Text
    {
        get => _text;
        set
        {
            _text = value;
            _text.Parent = this;
        }
    }
    
    public Action? OnClick { get; init; }

    public override void Render()
    {
        base.Render();
        _text.Render();
    }

    public override void Resize()
    {
        var minSize = InnerPadding * 2 + _text.Size;
        Size = Size.ExpandTo(minSize);
    }

    public override void Remove()
    {
        Display.RemoveFromRenderList(this);
        Display.RemoveFromRenderList(_text);
    }

    public override void Clear()
    {
        base.Clear();
        _text.Clear();
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        State = State.Highlighted;
        base.OnMouseEnter(e);
    }

    protected override void OnMouseExit(MouseEventArgs e)
    {
        State = State.Default;
        base.OnMouseExit(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released) State = State.Highlighted;
        base.OnMouseMove(e);
    }

    protected override void OnMouseLeftDown(MouseEventArgs e)
    {
        State = State.Pressed;
        OnClick?.Invoke();
        
        base.OnMouseLeftDown(e);
    }
}
