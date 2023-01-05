namespace Minesweeper.UI;

public abstract class ContentControl : Control, IContent
{
    private Control? _content;

    public Control? Content
    {
        get => _content;
        set => SetContent(value);
    }

    private void SetContent(Control? value)
    {
        if (value == this)
        {
            throw new InvalidOperationException("Control cannot be its own content.");
        }

        if (_content != null)
        {
            _content.Parent = null;
            _content.Remove();
        }
        
        if (value is null)
        {
            _content = null;
            
            if (ResizeMode == ResizeMode.Stretch) Resize();
            return;
        }

        _content = value;
        _content.Parent = this;
        
        if (_content.ResizeMode != ResizeMode.Manual) _content.Resize();
    }

    public override void Clear()
    {
        base.Clear();
        Content?.Clear();
    }
    
    public override void Resize()
    {
        if (_content == null) return;

        var minSize = _content.PaddedSize + InnerPadding * 2;
        _content.Position = InnerPadding + _content.OuterPadding;

        Size = ResizeMode switch
        {
            ResizeMode.Grow => Size.ExpandTo(minSize),
            ResizeMode.Stretch => minSize,
            _ => Size
        };
    }
}

public interface IContent
{
    public Control? Content { get; set; }
}