using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.Game;

public static class Game
{
    private static TileGrid _tileGrid = null!;
    private static int _remainingFlags;

    public static void Start(int width, int height, int bombs)
    {
        _tileGrid = new TileGrid(width, height, bombs);

        _tileGrid.PlacedFlag += ChangeFlagCount;
        _tileGrid.BombClicked += OnBombClicked;
        
        DisplayInterface();
    }

    private static void DisplayInterface()
    {
        var grid = new Grid
        {
            Size = (10, 10),

            Color = Color.RosyBrown,
            ShowGridLines = true
        };
        
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());

        var menuButton = new Button
        {
            Text = new Text("Menu"),
            Color = Color.Crimson,
            
            OnClick = () =>
            {
                grid.Remove();
                Back();
            }
        };

        var restartButton = new Button
        {
            Text = new Text(":)", Color.Yellow),
            Color = Color.RoyalBlue,
            
            InnerPadding = (2, 1),
            
            OnClick = Restart
        };

        grid.SetColumnAndRow(menuButton, 0, 0);
        grid.SetColumnAndRow(restartButton, 1, 1);
        grid.SetColumnAndRow(_tileGrid, 1, 2);
        
        Display.SortRenderables();
    }
    
    private static void Restart()
    {
        _tileGrid.GenerateNew();
        _tileGrid.SetEnabled(true);
    }

    private static void OnBombClicked(object? sender, EventArgs eventArgs)
    {
        _tileGrid.SetEnabled(false);
    }
    
    private static void ChangeFlagCount(object? sender, int change)
    {
        _remainingFlags += change;
    }

    private static void Back()
    {
        _tileGrid.PlacedFlag -= ChangeFlagCount;
        _tileGrid.BombClicked -= OnBombClicked;

        MainMenu.Show();
    }

}