using System.Runtime.CompilerServices;
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
    
    public Coord InnerPadding = new(1, 1);
    public Coord OuterPadding = Coord.Zero;

    public int PaddedWidth => Width + OuterPadding.X * 2;
    public int PaddedHeight => Height + OuterPadding.Y * 2;

    public Coord PaddedSize => Size + OuterPadding * 2;
    
    public Color Color
    {
        get
        {
            if (!Enabled) return DefaultColor.Dimmer();
            
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
        Display.DrawRect(GlobalPosition, Size, Color);
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

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        Display.ClearRect(GlobalPosition - e.Delta, Size);
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Display.ClearRect(GlobalPosition, e.OldSize);
    }
}
