using Minesweeper.ConsoleDisplay;
using Minesweeper.Game;
using Minesweeper.UI.Events;
using Minesweeper.UI.Widgets;
using Minesweeper.Visuals;
using Minesweeper.Visuals.Figlet;

namespace Minesweeper.UI;

public static class MainMenu
{
    private static readonly List<GamePreset> GamePresets = new()
    {
        new GamePreset("Easy", 15, 10, 15),
        new GamePreset("Medium", 30, 12, 40),
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
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());
        grid.Rows.Add(new Row());

        var red = new Label
        {
            Color = Color.Red,
            ResizeMode = ResizeMode.Expand
        };

        var blue = new Label
        {
            Color = Color.Blue
        };

        var yellow = new Label
        {
            Color = Color.Yellow,
            // ResizeMode = ResizeMode.Expand,
            Text = new Text("Long Label")
        };

        var green = new Label
        {
            Color = Color.Green
        };

        grid.SetColumnAndRow(red, 0, 0);
        grid.SetColumnSpanAndRowSpan(red, 2, 2);

        grid.SetColumnAndRow(blue, 2, 1);
        grid.SetColumnAndRow(yellow, 0, 2);
        grid.SetColumnAndRow(green, 1, 2);

        yellow.MouseScroll += delegate(Control sender, MouseEventArgs args)
        {
            if (args.ScrollDirection == ScrollDirection.Down && sender.Width > sender.MinSize.X) sender.Width--;
            else if (args.ScrollDirection == ScrollDirection.Up) sender.Width++;
        };

        blue.MouseScroll += delegate(Control sender, MouseEventArgs args)
        {
            if (args.ScrollDirection == ScrollDirection.Down && sender.Height > sender.MinSize.Y) sender.Height--;
            else if (args.ScrollDirection == ScrollDirection.Up) sender.Height++;
        };

        green.MouseMove += delegate(Control sender, MouseEventArgs args)
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
            Text = new BigText("MINESWEEPER", Font.BigFig),

            BorderColor = Color.Black,
            BorderStyle = BorderStyle.Rounded,
            ShowBorder = true,

            InnerPadding = (2, 1)
        };

        grid.SetColumnAndRow(titleLabel, 0, 0);
        grid.SetColumnSpanAndRowSpan(titleLabel, 4, 1);

        titleLabel.MouseScroll += delegate(Control sender, MouseEventArgs args)
        {
            if (args.ScrollDirection == ScrollDirection.Down && sender.Width > sender.MinSize.X) sender.Width--;
            else if (args.ScrollDirection == ScrollDirection.Up) sender.Width++;
        };

        titleLabel.MouseDown += delegate(Control sender, MouseEventArgs args)
        {
            if (sender is not Label {Text: BigText bigText}) return;

            bigText.CharacterWidth = args switch
            {
                { LeftButton: MouseButtonState.Pressed } => CharacterWidth.Full,
                { MiddleButton: MouseButtonState.Pressed } => CharacterWidth.Fitted,
                { RightButton: MouseButtonState.Pressed } => CharacterWidth.Smush,
                _ => throw new ArgumentOutOfRangeException(nameof(args), args, null)
            };
        };

        var variable = new Variable();
        var playButton = new Button
        {
            Text = new BigText("play", Font.CalvinS),
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
        grid.SetColumnSpanAndRowSpan(playButton, 2, 1);

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
            var radioButton = new RadioButton(variable, i)
            {
                Text = new Text(preset.Name),
                Color = gradient[i],
                ResizeMode = ResizeMode.Expand
            };

            radioButton.DoubleClick += delegate
            {
                grid.Remove();
                StartGame(variable.Val);
            };

            var width = new Label
            {
                Text = new Text(preset.Width.ToString()),
                Color = Color.DarkGray
            };

            var height = new Label
            {
                Text = new Text(preset.Height.ToString()),
                Color = Color.DarkGray,
                ResizeMode = ResizeMode.Expand
            };

            var bombs = new Label
            {
                Text = new Text(preset.Bombs.ToString()),
                Color = Color.DarkGray,
                ResizeMode = ResizeMode.Expand
            };

            grid.SetColumnAndRow(radioButton, 0, i + 2);
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

        customButton.DoubleClick += delegate
        {
            grid.Remove();
            StartGame(variable.Val);
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
            InputMode = EntryMode.Digits,
            ResizeMode = ResizeMode.Expand
        };
        heightEntry.SetEnabled(false);

        _customBombsText = new EntryText("100");
        var bombsEntry = new Entry
        {
            Color = Color.DarkGray,
            Text = _customBombsText,
            MaxTextLenght = 4,
            InputMode = EntryMode.Digits,
            ResizeMode = ResizeMode.Expand
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
