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
        var frame = new Frame(5, 3);

        var title = new Label(frame, new UString("MINESWEEPER", Color.DarkBlue))
        {
            Size = new Coord(15, 3),
            DefaultColor = Color.PaleGoldenrod
        }.Grid(0, 1, GridAlignment.N);

        var easyLabel = new Label(frame, "Easy")
        {
            Size = new Coord(7, 3),
            DefaultColor = Color.DarkGreen
        }.Grid(1, 0);
        
        var mediumLabel = new Label(frame, "Medium")
        {
            Size = new Coord(7, 3),
            DefaultColor = Color.Yellow
        }.Grid(2, 0);
        
        var hardLabel = new Label(frame, "Hard")
        {
            Size = new Coord(7, 3),
            DefaultColor = Color.Orange
        }.Grid(3, 0);

        var playButton = new Button(frame, "PLAY")
        {
            Size = new Coord(12, 3),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
            
            OnClick = () => Console.Title = "Playing!"
        }.Grid(4, 1);
    }
}
