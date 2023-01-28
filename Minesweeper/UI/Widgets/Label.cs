using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI.Widgets;

public class Label : ContentControl
{
    public Label()
    {
        _text = new Text(nameof(Label)) {Parent = this};
        Focusable = false;
    }

    private Text _text;
    public Text Text
    {
        get => _text;
        set
        {
            _text.Remove();
            _text.Parent = null!;
            
            _text = value;
            _text.Parent = this;
        }
    }

    public override void Remove()
    {
        // Display.RemoveFromRenderList(_text);
        _text.Remove();
        base.Remove();
    }

    public override void Clear()
    {
        base.Clear();
        _text.Clear();
    }

    public override void Resize()
    {
        MinSize = InnerPadding * 2 + _text.Size;

        Size = ResizeMode switch
        {
            ResizeMode.Grow => Size.ExpandTo(MinSize),
            ResizeMode.Stretch => MinSize,
            _ => Size
        };

        base.Resize();
    }
}
