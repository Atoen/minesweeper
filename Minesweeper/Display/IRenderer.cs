using Minesweeper.Game;

namespace Minesweeper.Display;

public interface IRenderer
{
    void Draw(int posX, int posY, char symbol, Color fg, Color bg);

    void Draw(int posX, int posY, TileDisplay tile);

    void ClearAt(int posX, int posY);

    void Draw();

    public bool Modified { get; set; }
}
