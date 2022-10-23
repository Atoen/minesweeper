using System.Runtime.CompilerServices;
using Minesweeper;
using Minesweeper.UI;

namespace Minesweeper;

public static class MainMenu
{
    private static short GridWidth;
    private static short GridHeight;
    private static short BombAmount;

    private static readonly List<IRenderable> Ui = new();

    public static void Display()
    {   
        var bombLabel = new Label("Bomb Amount")
        {
            Pos = new Coord(4, 2),
            Size = new Coord(13, 2),
            DefaultColor = ConsoleColor.Gray
        };
        Ui.Add(bombLabel);
        
        var widthLabel = new Label("Width", Alignment.Center)
        {
            Pos = new Coord(20, 2),
            Size = new Coord(8, 2),
            DefaultColor = ConsoleColor.Gray
        };
        Ui.Add(widthLabel);
        
        var heightLabel = new Label("Height")
        {
            Pos = new Coord(30, 2),
            Size = new Coord(8, 2),
            DefaultColor = ConsoleColor.Gray
        };
        Ui.Add(heightLabel);
        
        var bombSpinbox = new Spinbox(0, 20, 5)
        {
            Pos = new Coord(7, 5),
            Size = new Coord(6, 1)
        };
        Ui.Add(bombSpinbox);
        
        var gridWidth = new Spinbox(3, 80, 40)
        {
            Pos = new Coord(20, 5),
            Size = new Coord(6, 1)
        };
        Ui.Add(gridWidth);
        
        var gridHeight = new Spinbox(3, 20, 15)
        {
            Pos = new Coord(30, 5),
            Size = new Coord(6, 1)
        };
        Ui.Add(gridHeight);


        var playButton = new Button("PLAY")
        {
            Pos = new Coord(18, 15),
            Size = new Coord(12, 3),
            DefaultColor = ConsoleColor.Gray,
            HighlightedColor = ConsoleColor.Green,
            PressedColor = ConsoleColor.Yellow,
            ClickAction = ClickAction,
        };
        Ui.Add(playButton);
        
        void ClickAction()
        {
            BombAmount = bombSpinbox.CurrentVal;
            GridWidth = gridWidth.CurrentVal;
            GridHeight = gridHeight.CurrentVal;

            StartGame();
        }
    }

    private static void StartGame()
    {
        foreach (var ui in Ui)
        {
            ui.Destroy();
        }
        
        Game.Start(BombAmount, GridWidth, GridHeight);
    }
}
    