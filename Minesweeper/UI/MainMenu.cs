﻿using Minesweeper.ConsoleDisplay;
using Minesweeper.Game;
using Minesweeper.UI.Events;
using Minesweeper.UI.Widgets;

namespace Minesweeper.UI;

public static class MainMenu
{
    private static readonly List<GamePreset> GamePresets = new()
    {
        new GamePreset("Easy", 15, 10, 15),
        new GamePreset("Medium", 30, 12, 50),
        new GamePreset("Hard", 40, 20, 90)
    };

    private static EntryText _customWidthText = new("15");
    private static EntryText _customHeightText = new("20");
    private static EntryText _customBombsText = new("100");

    public static void Show2()
    {
        var grid = new Grid
        {
            Color = Color.LightSlateGray,
            ShowGridLines = true
        };
        
        grid.Columns.Add(new Column());
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());

        var label1 = new Label
        {
            Color = Color.Red
        };
        
        var label2 = new Label
        {
            Color = Color.Blue
        };
        
        var label3 = new Label
        {
            Color = Color.Yellow,
            ResizeMode = ResizeMode.Expand,
            Text = new Text("Long Label")
        };
        
        var label4 = new Label
        {
            Color = Color.Green
        };
        
        grid.SetColumnAndRow(label1, 0, 0);
        grid.SetColumnAndRow(label2, 1, 0);
        grid.SetColumnAndRow(label3, 0, 1);
        grid.SetColumnAndRow(label4, 1, 1);

        label1.MouseScroll += delegate(Control sender, MouseScrollEventArgs args)
        {
            if (args.ScrollDirection == ScrollDirection.Down && sender.Width > sender.MinSize.X) sender.Width--;
            else if (args.ScrollDirection == ScrollDirection.Up) sender.Width++;
        };
        
        label2.MouseScroll += delegate(Control sender, MouseScrollEventArgs args)
        {
            if (args.ScrollDirection == ScrollDirection.Down && sender.Height > sender.MinSize.Y) sender.Height--;
            else if (args.ScrollDirection == ScrollDirection.Up) sender.Height++;
        };
        
        label4.MouseMove += delegate(Control sender, MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed) sender.Center = args.CursorPosition;
        };
    }
    
    public static void Show()
    {
        var grid = new Grid
        {
            Color = Color.LightSlateGray,
            
            ShowGridLines = true,
            GridLineStyle = GridLineStyle.Single
        };

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
            Text = new Text("MINE"),
            
            BorderColor = Color.Black,
            BorderStyle = BorderStyle.Rounded,
            ShowBorder = true,
            
            InnerPadding = (2, 1),
            OuterPadding = (1, 0)
        };

        grid.SetColumnAndRow(titleLabel, 1, 0);

        titleLabel.MouseMove += delegate(Control sender, MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed) sender.Center = args.CursorPosition;
        };
        
        titleLabel.MouseScroll += delegate(Control sender, MouseScrollEventArgs args)
        {
            if (args.ScrollDirection == ScrollDirection.Down && sender.Width > sender.MinSize.X) sender.Width--;
            else if (args.ScrollDirection == ScrollDirection.Up) sender.Width++;
        };

        var variable = new Variable();
        var playButton = new Button
        {
            Text = new Text("PLAY ORO VERy"),
            Color = Color.Aquamarine,
            
            ResizeMode = ResizeMode.Expand,
            OuterPadding = (1, 0),
            
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
                Color = gradient[i],
                ResizeMode = ResizeMode.Expand
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
            Color = Color.DarkCyan,
            ResizeMode = ResizeMode.Expand
        };
        
        grid.SetColumnAndRow(customButton, 0, GamePresets.Count + 2);

        _customWidthText = new EntryText("15");
        var widthEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = _customWidthText,
            MaxTextLenght = 3,
            InputMode = EntryMode.Digits
        };
        widthEntry.SetEnabled(false);

        _customHeightText = new EntryText("20");
        var heightEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = _customHeightText,
            MaxTextLenght = 3,
            InputMode = EntryMode.Digits
        };
        heightEntry.SetEnabled(false);

        _customBombsText = new EntryText("100");
        var bombsEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = _customBombsText,
            MaxTextLenght = 4,
            InputMode = EntryMode.Digits
        };
        bombsEntry.SetEnabled(false);

        grid.SetColumnAndRow(widthEntry, 1, 5);
        grid.SetColumnAndRow(heightEntry, 2, 5);
        grid.SetColumnAndRow(bombsEntry, 3, 5);
        
        variable.OnValueChanged += delegate(Variable _, int value)
        {
            var enabled = value == GamePresets.Count;

            widthEntry.SetEnabled(enabled);
            heightEntry.SetEnabled(enabled);
            bombsEntry.SetEnabled(enabled);
        };
    }

    private static void StartGame(int preset)
    {
        if (preset < GamePresets.Count)
        {
            Game.Game.Start(GamePresets[preset]);
            return;
        }
        
        Game.Game.Start(GetCustomPreset());
    }

    private static GamePreset GetCustomPreset() => new()
    {
        Width = int.Parse(_customWidthText.String),
        Height = int.Parse(_customHeightText.String),
        Bombs = int.Parse(_customBombsText.String),
    };
}
