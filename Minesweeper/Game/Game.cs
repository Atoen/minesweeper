using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.Game;

public static class Game
{
    private static TileGrid _tileGrid = null!;
    private static int _remainingFlags;

    private static GamePreset _preset;

    private static Text _smileText = null!;
    private static Text _flagsText = null!;
    
    public static void Start(GamePreset preset)
    {
        _preset = preset;
        
        _tileGrid = new TileGrid(preset.Width, preset.Height, preset.Bombs);

        _tileGrid.PlacedFlag += ChangeFlagCount;
        _tileGrid.RemovedFlag += ChangeFlagCount;
        _tileGrid.BombClicked += OnBombClicked;
        _tileGrid.ClearedField += OnClearedField;

        _remainingFlags = _tileGrid.Bombs;

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

        _flagsText = new Text(_remainingFlags.ToString());
        var flagsLabel = new Label
        {
            Text = _flagsText,
            ResizeMode = ResizeMode.Grow
        };

        _smileText = new Text(":)", Color.Yellow);

        var restartButton = new Button
        {
            Text = _smileText,
            Color = Color.RoyalBlue,

            InnerPadding = (2, 1),

            OnClick = Restart
        };

        grid.SetColumnAndRow(menuButton, 0, 0);
        grid.SetColumnAndRow(flagsLabel, 0, 1);
        grid.SetColumnAndRow(restartButton, 1, 1);
        grid.SetColumnAndRow(_tileGrid, 1, 2);
    }

    private static void Restart()
    {
        _tileGrid.GenerateNew();
        _tileGrid.SetEnabled(true);
        _smileText.String = ":)";

        _remainingFlags = _preset.Bombs;
        _flagsText.String = _remainingFlags.ToString();
        _flagsText.Background = Color.Transparent;
    }

    private static void OnBombClicked(object? sender, EventArgs eventArgs)
    {
        _tileGrid.SetEnabled(false);
        _smileText.String = ":(";
    }

    private static void ChangeFlagCount(object? sender, int change)
    {
        _remainingFlags -= change;
        _flagsText.String = _remainingFlags.ToString();

        _flagsText.Background = _remainingFlags < 0 ? Color.Tomato : Color.Transparent;
    }
    
    private static void OnClearedField(object? sender, EventArgs e)
    {
        _smileText.String = ":D";
    }

    private static void Back()
    {
        _tileGrid.PlacedFlag -= ChangeFlagCount;
        _tileGrid.RemovedFlag -= ChangeFlagCount;
        _tileGrid.BombClicked -= OnBombClicked;
        _tileGrid.ClearedField -= OnClearedField;

        Trace.WriteLine("Back");
        
        MainMenu.Show();
    }
}

public record struct GamePreset(string Name, int Width, int Height, int Bombs);
