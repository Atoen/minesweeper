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
        var label = new Label(frame, new UString("MINESWEEPER", Color.DarkBlue))
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
            Fill = FillMode.Horizontal
        }.Grid(1, 3);
        
        // Presets values
        var gradient = Display.Display.Gradient(Color.Green, Color.Orange, Presets.Count).ToList();
        
        for (var i = 0; i < Presets.Count; i++)
        {
            var preset = Presets[i];
        
            new Label(frame, preset.Name)
            {
                DefaultColor = gradient[i],
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

        new Label(frame, "Custom")
        {
            DefaultColor = Color.LightBlue,
            Fill = FillMode.Horizontal
        }.Grid(5, 0);

        new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "50",
            
            Fill = FillMode.Horizontal

        }.Grid(5, 1);
        
        new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "30",

            Fill = FillMode.Horizontal

        }.Grid(5, 2);
        
        new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "20",

            Fill = FillMode.Horizontal

        }.Grid(5, 3);
        
        // Play button
        new Button(frame, "PLAY")
        {
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
            
            Fill = FillMode.Both,
        
            OnClick = () => Console.Title = "Playing!"
        }.Grid(labelsRowOffset + Presets.Count + 1, 1, columnSpan: 2);
    }
}

