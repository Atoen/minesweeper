using Minesweeper.Display;

namespace Minesweeper.UI;

public interface IRenderable
{
    void Render();

    void Clear();
    
    bool ShouldRemove { get; }
    
    public Layer Layer { get; set; }

}