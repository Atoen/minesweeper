namespace Minesweeper.UI;

public interface IRenderable
{
    void Render();

    void Clear();
    
    bool ShouldRemove { get; }
}