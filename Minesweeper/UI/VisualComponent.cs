using Minesweeper.ConsoleDisplay;
using Minesweeper.Utils;

namespace Minesweeper.UI;

public abstract class VisualComponent : Component, IRenderable
{
    protected VisualComponent()
    {
        Display.AddToRenderList(this);

        PositionChanged += OnPositionChanged;
        SizeChanged += OnSizeChanged;
    }

    public Color DefaultColor { get; set; } = Color.Aqua;
    public Color HighlightedColor { get; set; } = Color.Blue;
    public Color PressedColor { get; set; } = Color.White;
    public Color DisabledColor { get; set; } = Color.DarkSlateGray;
    
    public Color Color
    {
        get => DefaultColor;
        set
        {
            DefaultColor = value;
            HighlightedColor = value.Brighter();
            PressedColor = value.Brighter(50);
            DisabledColor = value.Dimmer();
        }
    }
    
    public Coord InnerPadding = new(1, 1);
    public Coord OuterPadding = Coord.Zero;
    
    public bool ShowBorder { get; set; }
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Single;
    public Color BorderColor { get; set; } = Color.Cyan;

    public bool RenderOnItsOwn { get; set; }

    public bool ShouldRender => RenderOnItsOwn || Parent != null;

    public int PaddedWidth => Width + OuterPadding.X * 2;
    public int PaddedHeight => Height + OuterPadding.Y * 2;

    public Coord PaddedSize => Size + OuterPadding * 2;

    protected Coord MinSize;

    public Color CurrentColor
    {
        get
        {
            if (!Enabled) return DisabledColor;
            
            return State switch
            {
                State.Pressed => PressedColor,
                State.Highlighted => HighlightedColor,
                _ => DefaultColor
            };
        }
    }

    [MethodCall(MethodCallMode.Scheduled)]
    public virtual void Render()
    {
        Display.DrawRect(GlobalPosition, Size, CurrentColor);

        if (ShowBorder)
        {
            Display.DrawBorder(GlobalPosition, Size, BorderColor, BorderStyle);
        }
    }

    [MethodCall(MethodCallMode.OnEvent)]
    public virtual void Clear()
    {
        Display.ClearRect(GlobalPosition, Size);
    }

    [MethodCall(MethodCallMode.Manual)]
    public virtual void Remove()
    {
        Display.RemoveFromRenderList(this);
    }

    protected virtual void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        Display.ClearRect(GlobalPosition - e.Delta, Size);
    }
    
    protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Display.ClearRect(GlobalPosition, e.OldSize);
    }
}
