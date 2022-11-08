using Minesweeper.UI;

namespace Minesweeper;

public static class MainMenu
{
    private static short _gridWidth;
    private static short _gridHeight;
    private static short _bombAmount;

    private static readonly Frame MenuFrame = new();

    public static void Display()
    {
        var bombLabel = new Label(Color.Gray,"Bomb Amount")
        {
            Pos = new Coord(4, 2),
            Size = new Coord(13, 2),
            DefaultColor = Color.Gray
        };
        
        var widthLabel = new Label(Color.Gray, "Width")
        {
            Pos = new Coord(20, 2),
            Size = new Coord(8, 2),
            DefaultColor = Color.Gray
        };
        
        var heightLabel = new Label(Color.Gray, "Height")
        {
            Pos = new Coord(30, 2),
            Size = new Coord(8, 2),
            DefaultColor = Color.Gray
        };
        
        var bombSpinbox = new Spinbox(Color.Cyan, 0, 1000, 15)
        {
            Pos = new Coord(7, 5),
            Size = new Coord(6, 1),
            DefaultColor = Color.Cyan
        };
        
        var gridWidth = new Spinbox(Color.Cyan, 3, 100, 40)
        {
            Pos = new Coord(20, 5),
            Size = new Coord(6, 1),
        };
        
        var gridHeight = new Spinbox(Color.Cyan, 3, 40, 15)
        {
            Pos = new Coord(30, 5),
            Size = new Coord(6, 1),
        };
        
        var playButton = new Button(Color.White ,"PLAY")
        {
            Pos = new Coord(18, 15),
            Size = new Coord(12, 3),
            HighlightedColor = Color.Orange,
            PressedColor = Color.Green,
            OnClick = ClickAction,
        };
        
        MenuFrame.Add(bombLabel, bombSpinbox, gridHeight, gridWidth, heightLabel, playButton, widthLabel);

        Input.DoubleClick += delegate { StartGame(); };
        
        void ClickAction()
        {
            _bombAmount = bombSpinbox.CurrentVal;
            _gridWidth = gridWidth.CurrentVal;
            _gridHeight = gridHeight.CurrentVal;
        
            StartGame();
        }
    }

    private static void StartGame()
    {
        MenuFrame.Clear();

        Console.Read();
        
        
        Input.Stop();
        Minesweeper.Display.Stop();
        
        Console.Clear();
        
        // Game.Start(_bombAmount, _gridWidth, _gridHeight);
    }
}
    