using Minesweeper.Display;

namespace Minesweeper.UI;

public static class MainMenu
{
    private static short _gridWidth;
    private static short _gridHeight;
    private static short _bombAmount;

    private static readonly Frame MenuFrame = new();

    public static void Display()
    {
        var titleLabel = new Label(Color.White, "Minesweeper")
        {
            Pos = new Coord(22, 1),
            Size = new Coord(16, 3)
        };
        
        var variable = new Variable();

        var radio1 = new RadioButton(Color.Gray, "Easy", variable, 0)
        {
            Pos = new Coord(2, 8),
            Size = new Coord(13, 3),
            HighlightedColor = Color.Yellow,
            PressedColor = Color.DodgerBlue,
        };

        var radio2 = new RadioButton(Color.Gray, "Medium", variable, 1)
        {
            Pos = new Coord(2, 12),
            Size = new Coord(13, 3),
            HighlightedColor = Color.Yellow,
            PressedColor = Color.DodgerBlue,
        };

        var radio3 = new RadioButton(Color.Gray, "Hard", variable, 2)
        {
            Pos = new Coord(2, 16),
            Size = new Coord(13, 3),
            HighlightedColor = Color.Yellow,
            PressedColor = Color.DodgerBlue,
        };
        
        var radio4 = new RadioButton(Color.Gray, "Custom", variable, 3)
        {
            Pos = new Coord(2, 20),
            Size = new Coord(13, 3),
            HighlightedColor = Color.Yellow,
            PressedColor = Color.DodgerBlue,
        };
        
        var background = new Label(Color.Gray, "")
        {
            Pos = new Coord(17, 5),
            Size = new Coord(32, 18)
        };

        var bombLabel = new Label(Color.Gray,"Bomb Amount")
        {
            Pos = new Coord(17, 5),
            Size = new Coord(12, 2),
            DefaultColor = Color.Gray
        };
        
        var widthLabel = new Label(Color.Gray, "Width")
        {
            Pos = new Coord(31, 5),
            Size = new Coord(8, 2),
            DefaultColor = Color.Gray
        };
        
        var heightLabel = new Label(Color.Gray, "Height")
        {
            Pos = new Coord(41, 5),
            Size = new Coord(8, 2),
            DefaultColor = Color.Gray
        };

        var easyLabel = new Label(Color.Gray, "10           12       8")
        {
            Pos = new Coord(17, 9),
            Size = new Coord(30, 1)
        };
        
        var mediumLabel = new Label(Color.Gray, "20           24      12")
        {
            Pos = new Coord(17, 13),
            Size = new Coord(30, 1)
        };
        
        var hardLabel = new Label(Color.Gray, "50           30      15")
        {
            Pos = new Coord(17, 17),
            Size = new Coord(30, 1)
        };

        // var bombSpinbox = new Spinbox(Color.Cyan, 0, 1000, 15)
        // {
        //     Pos = new Coord(7, 5),
        //     Size = new Coord(6, 1),
        //     DefaultColor = Color.Cyan
        // };
        //
        // var gridWidth = new Spinbox(Color.Cyan, 3, Minesweeper.Display.Display.Width, 40)
        // {
        //     Pos = new Coord(20, 5),
        //     Size = new Coord(6, 1),
        // };
        //
        // var gridHeight = new Spinbox(Color.Cyan, 3, Minesweeper.Display.Display.Height, 15)
        // {
        //     Pos = new Coord(30, 5),
        //     Size = new Coord(6, 1),
        // };
        //
        var playButton = new Button(Color.Green ,"PLAY")
        {
            Pos = new Coord(23, 24),
            Size = new Coord(12, 3),
            HighlightedColor = Color.Lime,
            PressedColor = Color.DodgerBlue,
            OnClick = ClickAction,
        };

        // MenuFrame.Add(bombLabel, gridHeight, gridWidth, bombSpinbox, heightLabel, widthLabel, playButton);

        void ClickAction()
        {

            StartGame();
        }
    }

    private static void StartGame()
    {
        MenuFrame.Clear();

        Game.Game.Start(_bombAmount, _gridWidth, _gridHeight);
    }
}
    