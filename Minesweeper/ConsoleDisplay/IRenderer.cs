using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.ConsoleDisplay;

public interface IRenderer
{
    void Draw(int posX, int posY, char symbol, Color fg, Color bg);

    void DrawRect(Coord start, Coord end, Color color, char symbol);

    void DrawLine(Coord pos, Coord direction, int length, Color fg, Color bg, char symbol);

    void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode mode);

    void DrawBuffer(Coord start, Coord end, Pixel[,] buffer);

    void DrawBorder(Coord start, Coord end, Color color, BorderStyle style);

    void ClearAt(int posX, int posY);

    void ClearRect(Coord start, Coord end);

    void Draw();

    void ResetStyle();
    
    public bool Modified { get; set; }

    public void Clear();
}
