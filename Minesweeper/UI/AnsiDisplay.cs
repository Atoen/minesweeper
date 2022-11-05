using System.Drawing;
using System.Text;

namespace Minesweeper.UI;

internal class AnsiDisplay
{
    public void DrawRect(Coord pos, Coord size, Color fg, Color bg, char symbol = ' ')
    {
        var colored = new string(symbol, size.X).Color(fg, bg);
        
        for (var i = 0; i < size.Y; i++)
        {
            var str = $"\x1b[{pos.Y + i};{pos.X}f{colored}";
            Console.Write(str);
        }
    }
}