namespace Minesweeper.UI;

public interface IRenderable
{
    void Render();

    void Clear();
    
    int ZIndex { get; }
}