namespace Minesweeper.UI;

public abstract class ContentControl : Control, IContent
{
    protected ContentControl(bool renderOnItsOwn = false) : base(renderOnItsOwn) { }
    
    private Control? _content;

    public Control? Content
    {
        get => _content;
        set
        {
            if (value is null)
            {
                if (_content != null) _content.Parent = null;
                
                _content?.Remove();
            }
            
            _content = value;
            
            if (_content is null) return;
            
            _content.Parent = this;

            if (_content.AutoResize) _content.Resize();
            if (AutoResize) Resize();
        }
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
        if (Content is not null) minSize += Content.PaddedSize;
    
        Size = Size.ExpandTo(minSize);
    }
}

public interface IContent
{
    public Control? Content { get; set; }
}