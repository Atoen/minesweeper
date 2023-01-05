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
            return;
        }

        _content = value;
        _content.Parent = this;

        if (_content.AutoResize) _content.Resize();
        if (AutoResize) Resize();
    }

    public override void Render()
    {
        base.Render();
        Content?.Render();
    }

    public override void Clear()
    {
        base.Clear();
        Content?.Clear();
    }
    
    public override void Resize()
    {
        var minSize = InnerPadding * 2;
        if (Content is not null)
        {
            minSize += Content.PaddedSize;
            Content.Position = InnerPadding;
        }
    
        Size = Size.ExpandTo(minSize);
        
        if (Parent is {AutoResize: true}) Parent.Resize();
    }
}

public interface IContent
{
    public Control? Content { get; set; }
}