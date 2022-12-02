namespace Minesweeper.UI;

public static class MainMenu
{
    private static readonly List<DifficultyPreset> Presets = new()
    {
        new DifficultyPreset("Easy", 10, 12, 8),
        new DifficultyPreset("Medium", 20, 24, 12),
        new DifficultyPreset("Hard", 50, 30, 15)
    };

    public static void Show()
    {
        var frame = new Frame(Presets.Count + 3, 4)
        {
            Pos = (1, 1)
        };

        var labelsRowOffset = 2;

        // Title label
        new Label(frame, new UString("MINESWEEPER", Color.DarkBlue))
        {
            DefaultColor = Color.PaleGoldenrod,
        }.Grid(0, 1, columnSpan: 2);
        
        // Play button
        new Button(frame, "PLAY")
        {
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,

            OnClick = () => Console.Title = "Playing!"
        }.Grid(labelsRowOffset + Presets.Count, 1, columnSpan: 2);

        // Background for difficulty presets
        new Background(frame)
        {
            DefaultColor = Color.Gray,
            InnerPadding = (1, 1)
        }.Grid(1, 0, rowSpan: 4, columnSpan: 4);
        
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
    }

    private record struct DifficultyPreset(string Name, int BombAmount, int GridWidth, int GridHeight);
    
    // Difficulty labels
        // new Label(frame, "Easy")
        // {
        //     InnerPadding = (2, 1),
        //     DefaultColor = Color.DarkGreen,
        // }.Grid(1, 0);
        //
        // new Label(frame, "Medium")
        // {
        //     DefaultColor = Color.Yellow,
        // }.Grid(2, 0);
        //
        // new Label(frame, "Hard")
        // {
        //     InnerPadding = (2, 1),
        //     DefaultColor = Color.Orange
        // }.Grid(3, 0);

    
}

