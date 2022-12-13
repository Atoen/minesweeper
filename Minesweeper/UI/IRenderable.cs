using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public interface IRenderable
{
    void Render();

    void Clear();
    
    bool ShouldRemove { get; }

    bool StateChanged { get; }
    
    public Layer Layer { get; set; }
}