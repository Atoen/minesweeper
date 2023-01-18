using Minesweeper.ConsoleDisplay;
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
            Size = (10, 10),
            
            DefaultColor = Color.Gray,
            GridLinesColor = Color.DarkGray.Brighter(10),
            ShowGridLines = true
        };
        
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        
        var label = new Label
        {
            Color = Color.Orange,
            Text = new Text("MINESWEEPER"),
            
            BorderColor = Color.Black,
            BorderStyle = BorderStyle.Rounded,
            ShowBorder = true,
            
            InnerPadding = (2, 1)
        };
        
        label.MouseRightDown += delegate
        {
            label.ShowBorder = !label.ShowBorder;
        };
        
        var button = new Button
        {
            Text = new Text("PLAY"),
            Color = Color.Aquamarine,
            
            InnerPadding = (2, 1)
        };

        var variable = new Variable();
        var gradient = Colors.Gradient(Color.Green, Color.Orange, 3).ToList();
        
        var easyButton = new RadioButton(variable, 0)
        {
            Text = new Text("Easy"),
            Color = gradient[0]
        };
        
        var mediumButton = new RadioButton(variable, 1)
        {
            Text = new Text("Medium"),
            Color = gradient[1]
        };
        
        var hardButton = new RadioButton(variable, 2)
        {
            Text = new Text("Hard"),
            Color = gradient[2]
        };
        
        grid.SetColumnAndRow(label, 1, 0);
        grid.SetColumnAndRow(easyButton, 0, 1);
        grid.SetColumnAndRow(mediumButton, 0, 2);
        grid.SetColumnAndRow(hardButton, 0, 3);
        grid.SetColumnAndRow(button, 1, 4);
    }
}
