using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;

namespace Minesweeper.Game;

public static class Game
{
    private static Grid _grid = null!;
    private static bool _gameIsRunning;

    public static void Start(int bombs, int width, int height)
    {
        Input.MouseLeftClick += InputOnMouseClick;
        Input.MouseRightClick += InputOnMouseClick;
        
        _grid = new Grid(bombs, width, height);

        _grid.BombClicked += OnBombClicked;
        
        DisplayInterface();

        _gameIsRunning = true;
    }

    private static void DisplayInterface()
    {
        var frame = new Frame(2, 3)
        {
            Pos = (1, 1)
        };

        new Button(frame, "Menu")
        {
            DefaultColor = Color.DarkGray,
            HighlightedColor = Color.Gray,
            PressedColor = Color.White,
            
            Fill = FillMode.Both,

            OnClick = () => 
            {
                frame.Clear();
                MainMenu();
            }
        }.Grid(0, 0);

       
        new Button(frame,  new UString(":)", Color.Wheat))
        {
            DefaultColor = Color.DarkGreen,
            HighlightedColor = Color.DarkGreen.Dimmer(),
            PressedColor = Color.Green,

            OnClick = () => _grid.GenerateNew()
        }.Grid(0, 1);

        new Label(frame, new UString("Timer", Color.Red))
        {
            DefaultColor = Color.DarkGray
        }.Grid(0, 2);
        
        new Background(frame)
        {
            DefaultColor = Color.Gray,
        }.Grid(0, 0, columnSpan: 3);

        new Canvas(frame, _grid).Grid(1, 0, columnSpan: 3);
    }

    private static void InputOnMouseClick(MouseState state)
    {
        if (!_gameIsRunning) return;
        
        _grid.ClickTile(state.Position - _grid.Offset, state.Buttons);
    }

    private static void OnBombClicked()
    {
        _gameIsRunning = false;
    }

    private static void MainMenu()
    {
        Input.MouseLeftClick -= InputOnMouseClick;
        Input.MouseRightClick -= InputOnMouseClick;

        _grid.BombClicked -= OnBombClicked;
        
        UI.MainMenu.Show();
    }
}
