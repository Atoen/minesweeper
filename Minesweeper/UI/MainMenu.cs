using Minesweeper.Display;

namespace Minesweeper.UI;

public static class MainMenu
{
    // private static readonly int[][] DifficultySettings =
    // {
    //     new []{10, 12, 8},
    //     new []{20, 24, 12},
    //     new []{50, 30, 15}
    // };

    private static readonly int[,] DifficultySettings =
    {
        {10, 12, 8},
        {20, 24, 12},
        {50, 30, 15}
    };

    // public static void Display()
    // {
    //     void Pack(Widget widget) => MenuFrame.Add(widget);
    //
    //     var titleLabel = new Label(Color.White, "Minesweeper")
    //     {
    //         Pos = new Coord(22, 1),
    //         Size = new Coord(16, 3)
    //     };
    //     
    //     Pack(titleLabel);
    //
    //     var variable = new Variable();
    //
    //     var radio1 = new RadioButton(Color.Gray, "Easy", variable, 0)
    //     {
    //         Pos = new Coord(2, 8),
    //         Size = new Coord(13, 3),
    //         HighlightedColor = Color.Yellow,
    //         PressedColor = Color.DodgerBlue,
    //     };
    //     
    //     Pack(radio1);
    //
    //     var radio2 = new RadioButton(Color.Gray, "Medium", variable, 1)
    //     {
    //         Pos = new Coord(2, 12),
    //         Size = new Coord(13, 3),
    //         HighlightedColor = Color.Yellow,
    //         PressedColor = Color.DodgerBlue,
    //     };
    //     
    //     Pack(radio2);
    //
    //     var radio3 = new RadioButton(Color.Gray, "Hard", variable, 2)
    //     {
    //         Pos = new Coord(2, 16),
    //         Size = new Coord(13, 3),
    //         HighlightedColor = Color.Yellow,
    //         PressedColor = Color.DodgerBlue,
    //     };
    //     
    //     Pack(radio3);
    //     
    //     var radio4 = new RadioButton(Color.Gray, "Custom", variable, 3)
    //     {
    //         Pos = new Coord(2, 20),
    //         Size = new Coord(13, 3),
    //         HighlightedColor = Color.Yellow,
    //         PressedColor = Color.DodgerBlue,
    //     };
    //     
    //     Pack(radio4);
    //     
    //     var background = new Label(Color.Gray, "")
    //     {
    //         Pos = new Coord(17, 5),
    //         Size = new Coord(32, 18)
    //     };
    //     
    //     Pack(background);
    //
    //     var bombLabel = new Label(Color.Gray,"Bomb Amount")
    //     {
    //         Pos = new Coord(17, 5),
    //         Size = new Coord(12, 2),
    //         DefaultColor = Color.Gray
    //     };
    //     
    //     Pack(bombLabel);
    //     
    //     var widthLabel = new Label(Color.Gray, "Width")
    //     {
    //         Pos = new Coord(31, 5),
    //         Size = new Coord(8, 2),
    //         DefaultColor = Color.Gray
    //     };
    //     
    //     Pack(widthLabel);
    //     
    //     var heightLabel = new Label(Color.Gray, "Height")
    //     {
    //         Pos = new Coord(41, 5),
    //         Size = new Coord(8, 2),
    //         DefaultColor = Color.Gray
    //     };
    //     
    //     Pack(heightLabel);
    //
    //     var easyBombs = new Label(Color.Gray, $"{DifficultySettings[0][0]}")
    //     {
    //         Pos = new Coord(22, 9),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(easyBombs);
    //
    //     var easyWidth = new Label(Color.Gray, $"{DifficultySettings[0][1]}")
    //     {
    //         Pos = new Coord(33, 9),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(easyWidth);
    //
    //     var easyHeight = new Label(Color.Gray, $"{DifficultySettings[0][2]}")
    //     {
    //         Pos = new Coord(42, 13),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(easyHeight);
    //     
    //     var mediumBombs = new Label(Color.Gray, $"{DifficultySettings[1][0]}")
    //     {
    //         Pos = new Coord(22, 13),
    //         Size = new Coord(5, 1)
    //     };
    //
    //     Pack(mediumBombs);
    //     
    //     var mediumWidth = new Label(Color.Gray, $"{DifficultySettings[1][1]}")
    //     {
    //         Pos = new Coord(33, 13),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(mediumWidth);
    //     
    //     var mediumHeight = new Label(Color.Gray, $"{DifficultySettings[1][2]}")
    //     {
    //         Pos = new Coord(42, 9),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(mediumHeight);
    //     
    //     var hardBombs = new Label(Color.Gray, $"{DifficultySettings[2][0]}")
    //     {
    //         Pos = new Coord(22, 17),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(hardBombs);
    //
    //     var hardWidth = new Label(Color.Gray, $"{DifficultySettings[2][1]}")
    //     {
    //         Pos = new Coord(33, 17),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(hardWidth);
    //
    //     var hardHeight = new Label(Color.Gray, $"{DifficultySettings[2][2]}")
    //     {
    //         Pos = new Coord(42, 17),
    //         Size = new Coord(5, 1)
    //     };
    //     
    //     Pack(hardHeight);
    //
    //     // var easyLabel = new Label(Color.Gray, "10           12       8")
    //     // {
    //     //     Pos = new Coord(17, 9),
    //     //     Size = new Coord(30, 1)
    //     // };
    //     
    //     // var mediumLabel = new Label(Color.Gray, "20           24      12")
    //     // {
    //     //     Pos = new Coord(17, 13),
    //     //     Size = new Coord(30, 1)
    //     // };
    //     
    //     // var hardLabel = new Label(Color.Gray, "50           30      15")
    //     // {
    //     //     Pos = new Coord(17, 17),
    //     //     Size = new Coord(30, 1)
    //     // };
    //
    //     // var bombSpinbox = new Spinbox(Color.Cyan, 0, 1000, 15)
    //     // {
    //     //     Pos = new Coord(7, 5),
    //     //     Size = new Coord(6, 1),
    //     //     DefaultColor = Color.Cyan
    //     // };
    //     
    //     // var gridWidth = new Spinbox(Color.Cyan, 3, Minesweeper.Display.Display.Width, 40)
    //     // {
    //     //     Pos = new Coord(20, 5),
    //     //     Size = new Coord(6, 1),
    //     // };
    //     
    //     // var gridHeight = new Spinbox(Color.Cyan, 3, Minesweeper.Display.Display.Height, 15)
    //     // {
    //     //     Pos = new Coord(30, 5),
    //     //     Size = new Coord(6, 1),
    //     // };
    //     
    //     var playButton = new Button(Color.Green ,"PLAY")
    //     {
    //         Pos = new Coord(23, 24),
    //         Size = new Coord(12, 3),
    //         HighlightedColor = Color.Lime,
    //         PressedColor = Color.DodgerBlue,
    //         OnClick = ClickAction,
    //     };
    //     
    //     Pack(playButton);
    //
    //     void ClickAction() => StartGame(variable.Val);
    // }

    public static void Show()
    {
        var frame = new NewFrame(2, 2);

        var label1 = new NewLabel(frame, UString.Empty)
        {
            Size = new Coord(15, 5),
            DefaultColor = Color.Orange
        }.Grid(0, 1);
        
        var label2 = new NewLabel(frame, UString.Empty)
        {
            Size = new Coord(20, 3),
            DefaultColor = Color.Blue
        }.Grid(1, 0);

        var label3 = new NewLabel(frame, UString.Empty)
        {
            Size = new Coord(5, 9),
            DefaultColor = Color.Green
        }.Grid(1, 1);
        
        var label4 = new NewLabel(frame, UString.Empty)
        {
            Size = new Coord(5, 3),
            DefaultColor = Color.Red
        }.Grid(0, 0);
    }
}
    