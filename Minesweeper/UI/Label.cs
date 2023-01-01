namespace Minesweeper.UI;

public class Label : VisualComponent
{
    public Label()
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

    public override void Render()
    {
        base.Render();
        _text.Render();
    }

    public override void Clear()
    {
        base.Clear();
        _text.Clear();
    }

    public override void Resize()
    {
        var minSize = InnerPadding * 2 + _text.Size;
        
        Size = Size.ExpandTo(minSize);
    }
}
