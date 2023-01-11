namespace Minesweeper.UI;

public abstract class ContentControl : Control, IContent
{
    private Control? _content;
    public Control? Content
    {
        get => _content;
        set => SetContent(value);
    }

    public override void Remove()
    {
        if (_content != null)
        {
            _content.Parent = null;
            _content.Remove();
        }
        
        base.Remove();
    }

    public override void Clear()
    {
        base.Clear();
        Content?.Clear();
    }
    
    public override void Resize()
    {
        if (_content == null) return;

        var contentSize = _content.PaddedSize + InnerPadding * 2;
        MinSize = MinSize.ExpandTo(contentSize);

        Size = ResizeMode switch
        {
            ResizeMode.Grow => Size.ExpandTo(MinSize),
            ResizeMode.Stretch => MinSize,
            _ => Size
        };

        _content.Center = Center;
    }
    
    private void SetContent(Control? value)
    {
        if (value == this)
        {
            throw new InvalidOperationException($"Control {value} cannot be its own content.");
        }

        if (_content != null)
        {
            _content.Parent = null;
            _content.Remove();
        }

        _content = value;

        if (_content == null)
        {
            if (ResizeMode != ResizeMode.Manual) Resize();
            return;
        }

        _content.Parent = this;

        if (_content.ResizeMode != ResizeMode.Manual) _content.Resize();
        if (ResizeMode != ResizeMode.Manual) Resize();
    }
}

public interface IContent
{
    public Control? Content { get; set; }
}
