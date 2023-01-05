using Minesweeper.ConsoleDisplay;
using Minesweeper.Game;
using Minesweeper.UI.Events;

namespace Minesweeper.UI;

public static class MainMenu
{
    private static readonly List<DifficultyPreset> Presets = new()
    {
        new DifficultyPreset("Easy", 10, 12, 8),
        new DifficultyPreset("Medium", 20, 24, 12),
        new DifficultyPreset("Hard", 50, 30, 15),
    };

    public static void Show()
    {
        var grid = new Grid
        {
            Width = 10,
            Height = 8,
            Position = new Coord(5, 5),
            ShowGridLines = true
        };
        
        grid.Columns.Add(new Column());
        grid.Rows.Add(new Row());

        var label = new Label
        {
            DefaultColor = Color.Orange
        };

        var button = new Button
        {
            DefaultColor = Color.Orchid
        };

        label.MouseLeftDown += delegate
        {
            var pos = grid.Position;
            pos.X++;

            grid.Position = pos;
        };
        
        label.MouseRightDown += delegate
        {
            label.Content = null;
        };

        label.Content = button;
        var label2 = new Label();
        label.Content = label2;
        
        grid.SetColumnAndRow(label, 0, 0);

    }
    
    //     var frame = new Frame(Presets.Count + 4, 4)
    //     {
    //         Pos = (1, 1)
    //     };
    //
    //     var labelsRowOffset = 2;
    //
    //     // Title label
    //     new Label(frame)
    //     {
    //         Text = new UString("MINESWEEPER", Color.DarkBlue)
    //         {
    //             Mode = TextMode.Bold
    //         },
    //         
    //         DefaultColor = Color.PaleGoldenrod,
    //     }.Grid(0, 1, columnSpan: 2);
    //     
    //     // Titles for preset values
    //     new Label(frame)
    //     {
    //         Text = new UString("Bombs", Color.Black),
    //         DefaultColor = Color.DarkGray,
    //         Fill = FillMode.Horizontal
    //     }.Grid(1, 1);
    //     
    //     new Label(frame)
    //     {
    //         Text = new UString("Width", Color.Black),
    //         DefaultColor = Color.DarkGray,
    //         Fill = FillMode.Horizontal
    //     }.Grid(1, 2);
    //     
    //     new Label(frame)
    //     {
    //         Text = new UString("Height", Color.Black),
    //         DefaultColor = Color.DarkGray,
    //         Fill = FillMode.Horizontal,
    //     }.Grid(1, 3);
    //     
    //     var bombsEntry = new Entry(frame)
    //     {
    //         DefaultColor = Color.Pink,
    //         MaxTextLenght = 4,
    //         TextBackground = Color.Gray,
    //         Text = "50",
    //         
    //         Fill = FillMode.Horizontal,
    //         InputMode = TextEntryMode.Digits
    //     
    //     }.Grid(5, 1);
    //     
    //     var widthEntry = new Entry(frame)
    //     {
    //         DefaultColor = Color.DarkGray,
    //         MaxTextLenght = 4,
    //         TextBackground = Color.Gray,
    //         Text = "30",
    //     
    //         Fill = FillMode.Horizontal,
    //         InputMode = TextEntryMode.Digits
    //     }.Grid(5, 2);
    //     
    //     var heightEntry = new Entry(frame)
    //     {
    //         DefaultColor = Color.DarkGray,
    //         MaxTextLenght = 4,
    //         TextBackground = Color.Gray,
    //         Text = "20",
    //     
    //         Fill = FillMode.Horizontal,
    //         InputMode = TextEntryMode.Digits
    //     }.Grid(5, 3);
    //     
    //     // Presets values
    //     var gradient = Colors.Gradient(Color.Green, Color.Orange, Presets.Count).ToList();
    //     var radioVariable = new Variable(1);
    //     
    //     for (var i = 0; i < Presets.Count; i++)
    //     {
    //         var preset = Presets[i];
    //     
    //         new RadioButton(frame, radioVariable, i)
    //         {
    //             Text = new UString(preset.Name, Color.Black),
    //             DefaultColor = gradient[i],
    //             HighlightedColor = gradient[i].Brighter(),
    //             PressedColor = gradient[i].Brighter(50),
    //             Fill = FillMode.Horizontal,
    //             
    //             OnClick = () => SetCustomEntryEnabled(false)
    //         }.Grid(i + labelsRowOffset, 0);
    //         
    //         new Label(frame)
    //         {
    //             Text = new UString(preset.BombAmount.ToString(), Color.Black),
    //             DefaultColor = Color.DarkGray,
    //             Fill = FillMode.Horizontal
    //         }.Grid(i + labelsRowOffset, 1);
    //         
    //         new Label(frame)
    //         {
    //             Text = new UString(preset.GridWidth.ToString(), Color.Black),
    //             DefaultColor = Color.DarkGray,
    //             Fill = FillMode.Horizontal
    //         }.Grid(i + labelsRowOffset, 2);
    //         
    //         new Label(frame)
    //         {
    //             Text =  new UString(preset.GridHeight.ToString(), Color.Black),
    //             DefaultColor = Color.DarkGray,
    //             Fill = FillMode.Horizontal
    //         }.Grid(i + labelsRowOffset, 3);
    //     }
    //     
    //     new RadioButton(frame, radioVariable, Presets.Count)
    //     {
    //         Text = new UString("Custom", Color.Black),
    //         DefaultColor = Color.CornflowerBlue,
    //         HighlightedColor = Color.CornflowerBlue.Brighter(),
    //         PressedColor = Color.CornflowerBlue.Brighter(50),
    //         
    //         OnClick = () => SetCustomEntryEnabled(true),
    //         SelectedTextMode = TextMode.Italic | TextMode.Underline
    //     }.Grid(5, 0);
    //     
    //     SetCustomEntryEnabled(radioVariable.Val == Presets.Count);
    //     
    //     void SetCustomEntryEnabled(bool enabled)
    //     {
    //         bombsEntry.SetEnabled(enabled);
    //         widthEntry.SetEnabled(enabled);
    //         heightEntry.SetEnabled(enabled);
    //     
    //         bombsEntry.Text.Text = "50";
    //         widthEntry.Text.Text = "30";
    //         heightEntry.Text.Text = "20";
    //     }
    //
    //     // Play button
    //     new Button(frame)
    //     {
    //         Text = new UString("PLAY", Color.Black),
    //         DefaultColor = Color.White,
    //         HighlightedColor = Color.Yellow,
    //         PressedColor = Color.Lime,
    //
    //         OnClick = () =>
    //         {
    //             frame.Clear();
    //             StartGame(GetSettings());
    //         }
    //     }.Grid(labelsRowOffset + Presets.Count + 1, 1, columnSpan: 2);
    //
    //     new Button(frame)
    //     {
    //         Text = new UString("X", Color.Red),
    //         OnClick = ShowDialog,
    //     }.Grid(labelsRowOffset + Presets.Count + 1, 3);
    //
    //     // Background for difficulty presets
    //     new Background(frame)
    //     {
    //         DefaultColor = Color.Gray,
    //     }.Grid(1, 0, rowSpan: Presets.Count + 2, columnSpan: 4);
    //     
    //     DifficultyPreset GetSettings()
    //     {
    //         var selected = radioVariable.Val;
    //         
    //         if (selected >= 0 && selected < Presets.Count)
    //             return Presets[selected];
    //         
    //         if (selected == Presets.Count)
    //         {
    //             return new DifficultyPreset
    //             {
    //                 BombAmount = int.Parse(bombsEntry.Text.Text),
    //                 GridWidth = int.Parse(widthEntry.Text.Text),
    //                 GridHeight = int.Parse(heightEntry.Text.Text)
    //             };
    //         }
    //     
    //         return new DifficultyPreset();
    //     }
    // }
    //
    // private static void StartGame(DifficultyPreset preset)
    // {
    //     Game.Game.Start(preset);
    // }
    //
    // private static void ShowDialog()
    // {
    //     var dialog = new DialogWindow();
    //     
    //     Display.AddToRenderList(dialog);
    // }
}
