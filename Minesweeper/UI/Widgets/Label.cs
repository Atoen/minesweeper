using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI.Widgets;

public class Label : ContentControl
{
    public Label() => _text = new Text(nameof(Label)) {Parent = this};

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

    public override void Render()
    {
        base.Render();
        
        Display.DrawLine(GlobalPosition, (-1, -1), 20, Color.Red, Color.Red, ' ');
    }

    protected override void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        base.OnPositionChanged(sender, e);
        
        Display.DrawLine(GlobalPosition - e.Delta, (-1, -1), 20, Color.Empty, Color.Empty, ' ');
    }

    public override void Remove()
    {
        Display.RemoveFromRenderList(_text);
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
