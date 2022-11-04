using System.Text;

namespace Minesweeper.UI;

internal class AnsiDisplay : IDisplayProvider
{
    public AnsiDisplay()
    {
        // var stdout = Console.OpenStandardOutput();
        // var con = new StreamWriter(stdout, Encoding.ASCII);
        // con.AutoFlush = true;
        // Console.SetOut(con);
    }
    
    public void Draw(Coord pos, TileDisplay tileDisplay)
    {

    }

    public void Draw(int posX, int posY, TileDisplay tileDisplay)
    {

    }

    public void Draw(int posX, int posY, char symbol, ConsoleColor foreground, ConsoleColor background)
    {

    }

    public void Draw2(int posX, int posY, string symbol, ConsoleColor foreground, ConsoleColor background)
    {
        var f = (int) foreground;
        
        Console.WriteLine($"\x1b[{f}m{symbol}");
    }

    public void ClearAt(Coord pos)
    {

    }

    public void ClearAt(int posX, int posY)
    {

    }

    public void Print(int posX, int posY, string text, ConsoleColor foreground, ConsoleColor background, Alignment alignment)
    {

    }

    public void AddToRenderList(IRenderable renderable)
    {

    }
}