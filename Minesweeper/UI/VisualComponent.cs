﻿using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public abstract class VisualComponent : Component, IRenderable
{
    protected VisualComponent()
    {
        Display.AddToRenderList(this);

        PositionChanged += OnPositionChanged;
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

    private void OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        Display.ClearRect(Position - e.Delta, Size);
    }
    
    public virtual void Render()
    {
        Display.DrawRect(Position, Size, Color);
    }

    public virtual void Clear()
    {
        Display.ClearRect(Position, Size);
    }

    public virtual void Remove()
    {
        Display.RemoveFromRenderList(this);
    }
}
