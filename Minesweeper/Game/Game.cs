using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;

namespace Minesweeper.Game;

public static class Game
{
    public static void Start(int bombs, int width, int height)
    {
        Input.MouseLeftClick += InputOnMouseClick;
        Input.MouseRightClick += InputOnMouseClick;
        
        DisplayInterface((width, height), bombs);

        // Thread.Sleep(100);
        //
    }

    private static void DisplayInterface(Coord gridSize, int bombs)
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

            OnClick = () =>
            {
                frame.Clear();
                MainMenu.Show();
            }
        }.Grid(0, 0);

        var buttonText = new UString(":)", Color.Green);
        new Button(frame, buttonText)
        {
            DefaultColor = Color.DarkGray,
            HighlightedColor = Color.DarkGray.Dimmer(),
            PressedColor = Color.DarkGoldenrod,

            OnClick = () => buttonText.Text = ":O"
        }.Grid(0, 1);

        new Label(frame, new UString("Timer", Color.Red))
        {
            DefaultColor = Color.DarkGray
        }.Grid(0, 2);
        
        new Background(frame)
        {
            DefaultColor = Color.Gray,
        }.Grid(0, 0, columnSpan: 3);

        new Background(frame)
        {
            DefaultColor = Color.Red,
            Size = gridSize
        }.Grid(1, 0, columnSpan: 3);
    }

    private static void InputOnMouseClick(MouseState state)
    {
        Grid.ClickTile(state.Position, state.Buttons);
    }

    public static void Stop()
    {
        Input.Stop();
    }
}
