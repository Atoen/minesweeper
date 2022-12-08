using Minesweeper.Display;

namespace Minesweeper.UI;

public static class MainMenu
{
    private record struct DifficultyPreset(string Name, int BombAmount, int GridWidth, int GridHeight);
    
    private static readonly List<DifficultyPreset> Presets = new()
    {
        new DifficultyPreset("Easy", 10, 12, 8),
        new DifficultyPreset("Medium", 20, 24, 12),
        new DifficultyPreset("Hard", 50, 30, 15),
    };

    public static void Show()
    {
        var frame = new Frame(Presets.Count + 4, 4)
        {
            Pos = (1, 1)
        };

        var labelsRowOffset = 2;

        // Title label
        new Label(frame, new UString("MINESWEEPER", Color.DarkBlue))
        {
            DefaultColor = Color.PaleGoldenrod,
        }.Grid(0, 1, columnSpan: 2);
        
        // Background for difficulty presets
        new Background(frame)
        {
            DefaultColor = Color.Gray,
            InnerPadding = (1, 1)
        }.Grid(1, 0, rowSpan: Presets.Count + 2, columnSpan: 4);

        // Titles for preset values
        new Label(frame, "Bombs")
        {
            DefaultColor = Color.DarkGray,
            Fill = FillMode.Horizontal
        }.Grid(1, 1);
        
        new Label(frame, "Width")
        {
            DefaultColor = Color.DarkGray,
            Fill = FillMode.Horizontal
        }.Grid(1, 2);
        
        new Label(frame, "Height")
        {
            DefaultColor = Color.DarkGray,
            Fill = FillMode.Horizontal,
        }.Grid(1, 3);
        
        // Presets values
        var gradient = Colors.Gradient(Color.Green, Color.Orange, Presets.Count).ToList();
        var radioVariable = new Variable();
        
        for (var i = 0; i < Presets.Count; i++)
        {
            var preset = Presets[i];
        
            new RadioButton(frame, preset.Name, radioVariable, i)
            {
                DefaultColor = gradient[i],
                HighlightedColor = gradient[i].Dimmer(),
                PressedColor = gradient[i].Brighter(),
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 0);
            
            new Label(frame, preset.BombAmount.ToString())
            {
                DefaultColor = Color.DarkGray,
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 1);
            
            new Label(frame, preset.GridWidth.ToString())
            {
                DefaultColor = Color.DarkGray,
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 2);
            
            new Label(frame, preset.GridHeight.ToString())
            {
                DefaultColor = Color.DarkGray,
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 3);
        }
        
        new RadioButton(frame, "Custom", radioVariable, Presets.Count)
        {
            DefaultColor = Color.CornflowerBlue,
            HighlightedColor = Color.CornflowerBlue.Dimmer(),
            PressedColor = Color.CornflowerBlue.Brighter()
        }.Grid(5, 0);
        
        var bombsEntry = new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "50",
            
            Fill = FillMode.Horizontal,
            InputMode = TextEntryMode.Digits
        
        }.Grid(5, 1);
        
        var widthEntry = new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "30",
        
            Fill = FillMode.Horizontal,
            InputMode = TextEntryMode.Digits
        }.Grid(5, 2);
        
        var heightEntry = new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "20",
        
            Fill = FillMode.Horizontal,
            InputMode = TextEntryMode.Digits
        }.Grid(5, 3);

        // Play button
        new Button(frame, "PLAY")
        {
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,

            OnClick = () =>
            {
                frame.Clear();
                StartGame(GetSettings());
            }
        }.Grid(labelsRowOffset + Presets.Count + 1, 1, columnSpan: 2);

        DifficultyPreset GetSettings()
        {
            var selected = radioVariable.Val;
            
            if (selected >= 0 && selected < Presets.Count)
                return Presets[selected];
            
            if (selected == Presets.Count)
            {
                return new DifficultyPreset()
                {
                    BombAmount = int.Parse(bombsEntry.Text.Text),
                    GridWidth = int.Parse(widthEntry.Text.Text ?? "0"),
                    GridHeight = int.Parse(heightEntry.Text.Text ?? "0")
                };
            }

            return new DifficultyPreset();
        }
    }

    private static void StartGame(DifficultyPreset preset)
    {
        Game.Game.Start(preset.BombAmount, preset.GridWidth, preset.GridHeight);
    }
}

