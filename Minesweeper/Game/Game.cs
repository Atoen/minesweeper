namespace Minesweeper.Game;

public static class Game
{
    public static void Start(int bombs, int width, int height)
    {
        Input.MouseLeftClick += InputOnMouseClick;
        Input.MouseRightClick += InputOnMouseClick;

        Thread.Sleep(100);
        
        Grid.Generate(bombs, width, height);
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
