namespace Minesweeper.UI;

internal interface IRenderable
{
    void Render();

    void Remove();

    bool ShouldRemove { get; }
}