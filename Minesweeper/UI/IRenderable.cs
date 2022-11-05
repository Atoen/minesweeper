namespace Minesweeper.UI;

public interface IRenderable
{
    void Render();

    void Remove();

    bool ShouldRemove { get; }
}