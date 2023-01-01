using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public abstract class Content : IRenderable
{
    public Coord Size { get; protected set; }

    public Coord Position => Parent.Center + Parent.ContentOffset;
    
    public Color Foreground { get; set; }
    public Color? Background { get; set; }

    public VisualComponent Parent { get; set; } = null!;

    public bool Enabled { get; set; } = true;
    
    public int Width => Size.X;

    public int Height => Size.Y;
    
    public abstract void Render();

    public abstract void Clear();

    public Layer Layer { get; set; }
}