using Minesweeper.Game;
using Minesweeper.UI.Events;
using Minesweeper.UI.Widgets;
using Grid = Minesweeper.UI.Widgets.Grid;

namespace Minesweeper.UI;

public static class MainMenu
{
    public static void Show()
    {
        var grid = new Grid
        {
            Position = new Coord(5, 5),
            ShowGridLines = true,
            Size = (10, 10)
        };

        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());

        grid.Columns.Padding = 2;

        var label = new Label
        {
            DefaultColor = Color.Orange,
            Text = new Text("Label with border")
        };

        label.MouseLeftDown += delegate
        {
            label.ShowBorder = !label.ShowBorder;
        };

        grid.SetColumnAndRow(label, 0, 0);
    }
}
