namespace Minesweeper.UI;

public static class MainMenu
{
    private static readonly int[,] DifficultySettings =
    {
        {10, 12, 8},
        {20, 24, 12},
        {50, 30, 15}
    };

    public static void Show()
    {
        var frame = new Frame(5, 3)
        {
            Pos = (1, 1)
        };

        var title = new Label(frame, new UString("MINESWEEPER", Color.DarkBlue))
        {
            InnerPadding = (2, 1),
            DefaultColor = Color.PaleGoldenrod
        }.Grid(0, 1);

        var easyLabel = new Label(frame, "Easy")
        {
            InnerPadding = (2, 1),
            DefaultColor = Color.DarkGreen,
        }.Grid(1, 0);
        
        var mediumLabel = new Label(frame, "Medium")
        {
            DefaultColor = Color.Yellow,
        }.Grid(2, 0);
        
        var hardLabel = new Label(frame, "Hard")
        {
            InnerPadding = (2, 1),
            DefaultColor = Color.Orange
        }.Grid(3, 0);

        new Label(frame, UString.Empty)
        {
            Size = (13, 3),
            DefaultColor = Color.Blue,
        }.Grid(4, 2);

        var playButton = new Button(frame, "PLAY")
        {
            InnerPadding = (5, 1),

            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
            
            OnClick = () => Console.Title = "Playing!"
        }.Grid(4, 1);

        var background = new Background(frame)
        {
            DefaultColor = Color.DarkGray,
        }.Grid(1, 1, rowSpan: 3, columnSpan: 2);
    }
}
