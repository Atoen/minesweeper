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
    
    public bool ShowBorder { get; set; }

    public int PaddedWidth => Width + OuterPadding.X * 2;
    public int PaddedHeight => Height + OuterPadding.Y * 2;

    public Coord PaddedSize => Size + OuterPadding * 2;

    protected Coord MinSize;
    
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
        
        if (ShowBorder) RenderBorder();
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
    
    public virtual void Resize()
    {
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        Display.ClearRect(GlobalPosition - e.Delta, Size);
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Display.ClearRect(GlobalPosition, e.OldSize);
    }

    private void RenderBorder()
    {
        for (var x = 1; x < Width - 1; x++)
        {
            Display.Draw(GlobalPosition.X + x, GlobalPosition.Y, '═', Color.White, Color);
            Display.Draw(GlobalPosition.X + x, GlobalPosition.Y + Height - 1, '═', Color.White, Color);
        }
        
        for (var y = 1; y < Height - 1; y++)
        {
            Display.Draw(GlobalPosition.X, GlobalPosition.Y + y, '║', Color.White, Color);
            Display.Draw(GlobalPosition.X + Width - 1, GlobalPosition.Y + y, '║', Color.White, Color);
        }
        
        Display.Draw(GlobalPosition.X, GlobalPosition.Y, '╔', Color.White, Color);
        Display.Draw(GlobalPosition.X + Width - 1, GlobalPosition.Y, '╗', Color.White, Color);
        Display.Draw(GlobalPosition.X, GlobalPosition.Y + Height - 1, '╚', Color.White, Color);
        Display.Draw(GlobalPosition.X + Width - 1, GlobalPosition.Y + Height - 1, '╝', Color.White, Color);
    }
}

public enum BorderStyle
{
    Single,
    Double,
    Rounded
}
