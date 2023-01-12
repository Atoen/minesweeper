using Minesweeper.Game;
using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.ConsoleDisplay;

public interface IRenderer
{
    void Draw(int posX, int posY, char symbol, Color fg, Color bg);

    void DrawRect(Coord pos, Coord size, Color color, char symbol);

    void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode mode);

    void DrawBuffer(Coord pos, Coord size, Pixel[,] buffer);

    void DrawBorder(Coord pos, Coord size, Color color, BorderStyle style);

    void ClearAt(int posX, int posY);

    void ClearRect(Coord pos, Coord size);

    void Draw();

    void ResetStyle();
}
