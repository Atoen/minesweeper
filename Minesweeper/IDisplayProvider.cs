using Minesweeper.UI;

namespace Minesweeper;

public interface IDisplayProvider
{
    void Draw(int posX, int posY, char symbol, Color foreground, Color background);

    void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ');
    
    void ClearAt(int posX, int posY);
    
    void AddToRenderList(IRenderable renderable);

    void Draw();
}