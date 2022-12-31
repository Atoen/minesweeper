using System.Net.NetworkInformation;
using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public abstract class VisualElement : Control, IRenderable
{
    protected VisualElement(bool renderOnItsOwn = false)
    {
        if (renderOnItsOwn)
        {
            Display.AddToRenderList(this);
        }
    }

    private Content? _content;
    public Content? Content
    {
        get => _content;
        set
        {
            _content = value;
            if (_content == null) return;
            
            _content.Parent = this;
        }
    }
    
    public Coord ContentOffset = Coord.Zero;

    public Color DefaultColor { get; set; } = Color.Aqua;
    public Color HighlightedColor { get; set; } = Color.Blue;
    public Color PressedColor { get; set; } = Color.White;
    
    public Coord InnerPadding = new(1, 1);
    public Coord OuterPadding = Coord.Zero;

    public Coord PaddedSize => Size + OuterPadding * 2;
    
    public Color Color => State switch
    {
        State.Pressed => PressedColor,
        State.Highlighted => HighlightedColor,
        State.Disabled => DefaultColor.Dimmer(),
        _ => DefaultColor
    };
    
    public virtual void Render()
    {
        Display.DrawRect(Position, Size, Color);
    }

    public virtual void Clear()
    {
        Display.ClearRect(Position, Size);
    }

    public override void Remove()
    {
        Display.RemoveFromRenderList(this);

        base.Remove();
    }

    public void Resize()
    {
        var minSize = InnerPadding * 2;
        if (Content is not null) minSize += Content.Size;

        Size = Size.ExpandTo(minSize);
    }
}
