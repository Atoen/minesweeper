using Minesweeper.Game;
using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.ConsoleDisplay;

public interface IRenderer
{
    void Draw(int posX, int posY, char symbol, Color fg, Color bg);

    void Draw(int posX, int posY, TileDisplay tile);

    void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode mode);

    void DrawBuffer(Coord pos, Coord size, AnsiDisplay.Pixel[,] buffer);

    void ClearAt(int posX, int posY);

    void Draw();

    void ResetStyle();
}
