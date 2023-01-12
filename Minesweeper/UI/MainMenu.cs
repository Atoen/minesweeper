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
            Size = (10, 10),
            ShowBorder = true,
            BorderStyle = BorderStyle.Dotted,
            BorderColor = Color.Red
        };

        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());

        var label = new Label
        {
            DefaultColor = Color.Orange,
            Text = new Text("Label with border"),
            ShowBorder = true,
            BorderColor = Color.Black,
            BorderStyle = BorderStyle.Dotted
        };
        
        label.MouseLeftDown += delegate
        {
            var style = (int) label.BorderStyle;
            style++;
            if (style == 5) style = 0;

            label.BorderStyle = (BorderStyle) style;

            label.Text.String = $"label with {label.BorderStyle} border";
        };
        
        label.MouseRightDown += delegate
        {
            label.ShowBorder = !label.ShowBorder;
        };

        grid.SetColumnAndRow(label, 0, 0);
    }
}
