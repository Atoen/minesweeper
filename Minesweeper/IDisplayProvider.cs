using Minesweeper.UI;

namespace Minesweeper;

internal interface IDisplayProvider
{
    void Draw(Coord pos, TileDisplay tileDisplay);
    void Draw(int posX, int posY, TileDisplay tileDisplay);
    void Draw(int posX, int posY, char symbol, ConsoleColor foreground, ConsoleColor background);
    
    void ClearAt(Coord pos);
    void ClearAt(int posX, int posY);

    void Print(int posX, int posY, string text, ConsoleColor foreground, ConsoleColor background, Alignment alignment);
    void AddToRenderList(IRenderable renderable);
}