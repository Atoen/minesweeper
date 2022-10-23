using System.Diagnostics;

namespace Minesweeper;

public static class Game
{
    public static void Start(short bombs, short width, short height)
    {
        Input.MouseLeftClick += InputOnMouseClick;
        Input.MouseRightClick += InputOnMouseClick;

        Grid.Generate(width, height);
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
