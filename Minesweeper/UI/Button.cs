using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public class Button : ContentControl
{
    public Button()
    {
        _text = new Text("") {Parent = this};
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
    
    public override void Clear()
    {
        base.Clear();
        _text.Clear();
    }

    public override void OnMouseEnter(MouseEventArgs e)
    {
        State = State.Highlighted;
        base.OnMouseEnter(e);
    }

    public override void OnMouseExit(MouseEventArgs e)
    {
        State = State.Default;
        base.OnMouseExit(e);
    }

    public override void OnMouseMove(MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released) State = State.Highlighted;
        base.OnMouseMove(e);
    }

    public override void OnMouseLeftDown(MouseEventArgs e)
    {
        State = State.Pressed;
        OnClick?.Invoke();
        
        base.OnMouseLeftDown(e);
    }
}
