using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Background : Widget
{
    public Background()
    {
        Layer = Layer.Background;
    }
    
    // public override Background Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    // {
    //     return base.Grid<Background>(row, column, rowSpan, columnSpan, alignment);
    // }
    //
    // public override Background Place(int posX, int posY)
    // {
    //     return base.Grid<Background>(posX, posY);
    // }

    // protected override void Resize() { }
}