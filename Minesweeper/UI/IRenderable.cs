using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public interface IRenderable
{
    void Render();

    void Clear();

    public Layer Layer { get; set; }
}