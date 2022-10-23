using System.Diagnostics;

namespace Minesweeper;

public static class Game
{
    private const int TicksPerSecond = 50;
    private const int TickLenght = 1000 / TicksPerSecond;

    private static bool _isRunning;

    public static void Start(short bombs, short width, short height)
    {
        if (_isRunning) return;
        _isRunning = true;

        Input.MouseLeftClick += InputOnMouseLeftClick;
        Input.MouseWheelEvent += InputOnMouseWheelEvent;
        
        Grid.Generate(width, height);
    }

    private static void InputOnMouseWheelEvent(MouseWheelState state)
    {
        Console.Write($"\r{state}");
    }

    private static void InputOnMouseLeftClick(MouseState state)
    {
        if (state.Buttons != 0) Grid.ClickTile(state.Position, state.Buttons);
    }

    public static void Stop()
    {
        Input.Stop();
        _isRunning = false;
    }
}
