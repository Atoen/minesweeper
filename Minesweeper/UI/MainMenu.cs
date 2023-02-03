using Minesweeper.ConsoleDisplay;
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
            Size = (10, 10),
            
            Color = Color.LightSlateGray,
            
            ShowGridLines = true,
            GridLineStyle = GridLineStyle.SingleBold
        };

        grid.Position = (4, 4);
        
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        
        var titleLabel = new Label
        {
            Color = Color.Orange,
            Text = new Text("MINESWEEPER"),
            
            BorderColor = Color.Black,
            BorderStyle = BorderStyle.Rounded,
            ShowBorder = true,
            
            InnerPadding = (2, 1)
        };
        
        grid.SetColumnAndRow(titleLabel, 1, 0);

        titleLabel.MouseMove += delegate(object _, MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed) args.OriginalSource.Center = args.CursorPosition;
        };

        var variable = new Variable();
        var playButton = new Button
        {
            Text = new Text("PLAY"),
            Color = Color.Aquamarine,
            
            InnerPadding = (2, 1),
            
            OnClick = () =>
            {
                grid.Remove();
                StartGame(variable.Val);
            }
        };
        
        grid.SetColumnAndRow(playButton, 1, 6);

        var widthLabel = new Label
        {
            Text = new Text("Width"),
            Color = Color.DarkGray
        };
        
        var heightLabel = new Label
        {
            Text = new Text("Height"),
            Color = Color.DarkGray
        };
        
        var bombsLabel = new Label
        {
            Text = new Text("Bombs"),
            Color = Color.DarkGray
        };
        
        grid.SetColumnAndRow(widthLabel, 1, 1);
        grid.SetColumnAndRow(heightLabel, 2, 1);
        grid.SetColumnAndRow(bombsLabel, 3, 1);

        var gradient = Colors.Gradient(Color.Green, Color.Orange, GamePresets.Count).ToList();
        
        for (var i = 0; i < GamePresets.Count; i++)
        {
            var preset = GamePresets[i];
            var button = new RadioButton(variable, i)
            {
                Text = new Text(preset.Name),
                Color = gradient[i]
            };

            var width = new Label
            {
                Text = new Text(preset.Width.ToString()),
                Color = Color.DarkGray
            };
            
            var height = new Label
            {
                Text = new Text(preset.Height.ToString()),
                Color = Color.DarkGray
            };
            
            var bombs = new Label
            {
                Text = new Text(preset.Bombs.ToString()),
                Color = Color.DarkGray
            };
            
            grid.SetColumnAndRow(button, 0, i + 2);
            grid.SetColumnAndRow(width, 1, i + 2);
            grid.SetColumnAndRow(height, 2, i + 2);
            grid.SetColumnAndRow(bombs, 3, i + 2);
        }

        var customButton = new RadioButton(variable, GamePresets.Count)
        {
            Text = new Text("Custom"),
            Color = Color.DarkCyan
        };
        
        grid.SetColumnAndRow(customButton, 0, GamePresets.Count + 2);

        var widthEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = new EntryText("15"),
            MaxTextLenght = 3,
            InputMode = EntryMode.Digits
        };

        var heightEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = new EntryText("20"),
            MaxTextLenght = 3,
            InputMode = EntryMode.Digits
        };

        var bombsEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = new EntryText("100"),
            MaxTextLenght = 4,
            InputMode = EntryMode.Digits
        };
        
        grid.SetColumnAndRow(widthEntry, 1, 5);
        grid.SetColumnAndRow(heightEntry, 2, 5);
        grid.SetColumnAndRow(bombsEntry, 3, 5);
        
        Display.SortRenderables();
    }

    private static void StartGame(int preset)
    {
        if (preset < GamePresets.Count)
        {
            Game.Game.Start(GamePresets[preset]);
        }
        
        
    }

    private static readonly List<GamePreset> GamePresets = new()
    {
        new GamePreset("Easy", 15, 10, 20),
        new GamePreset("Medium", 30, 12, 50),
        new GamePreset("Hard", 40, 20, 120)
    };
}
