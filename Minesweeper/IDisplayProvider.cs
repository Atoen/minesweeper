using Minesweeper.UI;

namespace Minesweeper;

public interface IDisplayProvider
{
    void Draw(int posX, int posY, char symbol, Color fg, Color bg);

    void DrawRect(Coord pos, Coord size, Color color, char symbol);

    void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment);
    
    void ClearAt(int posX, int posY);

    void ClearRect(Coord pos, Coord size);
    
    void AddToRenderList(IRenderable renderable);

    void Draw();
}